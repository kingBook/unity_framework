#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental;
using UnityEngine;

namespace PresetImportPerFolder {
#if UNITY_2020_1_OR_NEWER
    /// <summary>
    /// 此示例类将预设自动应用于包含预设的文件夹以及任何子文件夹中的资源。
    /// 代码划分为三个部分，用于设置导入器依赖关系以确保所有资源的导入器保持确定性。
    ///
    /// OnPreprocessAsset:
    /// 对于导入的资源，此方法从根文件夹向下进入资源文件夹，
    /// 并向每个文件夹注册 CustomDependency，以防以后添加/移除预设。
    /// 它随后从该文件夹加载所有预设并尝试将它们应用于资源导入器。
    /// 如果进行应用，则该方法会向每个预设添加直接依赖关系，以便可以在更改预设值时重新导入资源。
    /// </summary>
    public class EnforcePresetPostProcessor : AssetPostprocessor {
        void OnPreprocessAsset () {
            // if(assetPath....) 行可确保资源路径以"Assets/"开头，以便 AssetPostprocessor 不会应用于包中的资源。
            // 资源扩展名不能以 .cs 结尾，以避免在每次创建或移除预设时触发代码编译。
            // 资源扩展名不能以 .preset 结尾，以便预设不依赖于自己（这会导致无限的导入循环）。
            // 根据项目，可能要在此处添加更多例外情况。
            if (assetPath.StartsWith("Assets/") && !assetPath.EndsWith(".cs") && !assetPath.EndsWith(".preset")) {
                var path = Path.GetDirectoryName(assetPath);
                ApplyPresetsFromFolderRecursively(path);
            }
        }

        void ApplyPresetsFromFolderRecursively (string folder) {
            // 按照从父文件夹开始到资源的顺序来应用预设，以便距离资源最近的预设最后进行应用。
            var parentFolder = Path.GetDirectoryName(folder);
            if (!string.IsNullOrEmpty(parentFolder))
                ApplyPresetsFromFolderRecursively(parentFolder);

            // 向文件夹预设自定义键添加依赖关系，
            // 以便在每次对此文件夹添加或移除预设时，都会重新导入资源。
            context.DependsOnCustomDependency($"PresetPostProcessor_{folder}");

            // 查找此文件夹中的所有预设资源。使用 System.Directory 方法而不是 AssetDatabase
            // 因为导入可能会在单独进程中运行，这会阻止 AssetDatabase 执行全局搜索。
            var presetPaths =
                Directory.EnumerateFiles(folder, "*.preset", SearchOption.TopDirectoryOnly)
                    .OrderBy(a => a);

            foreach (var presetPath in presetPaths) {
                // 加载预设并尝试将它应用于导入器。
                var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);

                // 脚本会在两种情况下向资源添加预设依赖关系：
                //1 如果资源在预设之前导入，则预设不会加载，因为它尚未导入。
                //通过在资源与预设之间添加依赖关系可以重新导入资源，以便 Unity 加载
                //分配的预设并且可以尝试应用其值。
                //2 如果预设成功加载，则在预设应用于此资源的导入设置后，ApplyTo 方法会返回 true。
                //将预设作为依赖关系添加到资源可确保预设值的任何更改都会使用新值重新导入资源。
                if (preset == null || preset.ApplyTo(assetImporter)) {
                    // 在此处使用 DependsOnArtifact，因为预设是本机资源，使用 DependsOnSourceAsset 会不起作用。
                    context.DependsOnArtifact(presetPath);
                }
            }
        }
    }

    /// <summary>
    /// InitPresetDependencies:
    /// 在加载项目时会调用此方法。它会在项目中查找导入的每个预设。
    /// 对于每个包含预设的文件夹，通过文件夹名称创建 CustomDependency，并通过文件夹中预设名称和类型的列表创建哈希。
    ///
    /// OnAssetsModified:
    /// 每当对文件夹添加、移除或移动预设时，此文件夹的 CustomDependency 都需要更新
    /// 因此会重新导入可能依赖于这些预设的资源。
    ///
    /// TODO：理想情况下，每个 CustomDependency 还应依赖于 PresetType，
    /// 因此不会通过在文件夹中添加新 FBXImporterPreset 来重新导入纹理。
    /// 这使 InitPresetDependencies 和 OnPostprocessAllAssets 方法对于本示例而言太过复杂。
    /// Unity 建议让 CustomDependency 遵循"Preset_{presetType}_{folder}"的形式，
    /// 并且哈希仅包含该文件夹中给定 presetType 的预设。
    /// </summary>
    public class UpdateFolderPresetDependency : AssetsModifiedProcessor {
        /// <summary>
        /// 每次加载项目或编译代码时，都会调用包含 InitializeOnLoadMethod 的此方法。
        /// 在启动时正确设置所有哈希十分重要，
        /// 因为 Unity 不会将 OnPostprocessAllAssets 方法应用于以前导入的预设
        /// CustomDependencies 不会在会话之间保存，需要每次重新构建。
        /// </summary>
        [InitializeOnLoadMethod]
        static void InitPresetDependencies () {
            // AssetDatabase.FindAssets 使用 glob 过滤器避免导入项目中的所有对象。
            // 此 glob 搜索仅查找 .preset 文件。
            var allPaths = AssetDatabase.FindAssets("glob:\"**.preset\"")
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(a => a)
                .ToList();

            bool atLeastOnUpdate = false;
            string previousPath = string.Empty;
            Hash128 hash = new Hash128();
            for (var index = 0; index < allPaths.Count; index++) {
                var path = allPaths[index];
                var folder = Path.GetDirectoryName(path);
                if (folder != previousPath) {
                    // When a new folder is found, create a new CustomDependency with the Preset name and the Preset type.
                    if (previousPath != string.Empty) {

                        AssetDatabase.RegisterCustomDependency($"PresetPostProcessor_{previousPath}", hash);
                        atLeastOnUpdate = true;
                    }

                    hash = new Hash128();
                    previousPath = folder;
                }

                // Append both path and Preset type to make sure Assets get re-imported whenever a Preset type is changed.
                hash.Append(path);
                hash.Append(AssetDatabase.LoadAssetAtPath<Preset>(path).GetTargetFullTypeName());
            }

            // 注册最后一个路径。
            if (previousPath != string.Empty) {
                AssetDatabase.RegisterCustomDependency($"PresetPostProcessor_{previousPath}", hash);
                atLeastOnUpdate = true;
            }

            // 仅当此处至少更新了一个依赖关系时，才会触发刷新。
            if (atLeastOnUpdate)
                AssetDatabase.Refresh();
        }

        /// <summary>
        /// 每当在项目中更改资源时，都会调用 OnAssetsModified 方法。
        /// 此方法确定是否添加、移除或移动了任何预设
        /// 并更新与更改的文件夹相关的 CustomDependency。
        /// </summary>
        protected override void OnAssetsModified (string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets) {
            HashSet<string> folders = new HashSet<string>();
            foreach (var asset in changedAssets) {
                // 预设已更改，因此必须更新此文件夹的依赖关系，以防更改了预设类型。
                if (asset.EndsWith(".preset")) {
                    folders.Add(Path.GetDirectoryName(asset));
                }
            }

            foreach (var asset in addedAssets) {
                // 添加了新预设，因此必须更新此文件夹的依赖关系。
                if (asset.EndsWith(".preset")) {
                    folders.Add(Path.GetDirectoryName(asset));
                }
            }

            foreach (var asset in deletedAssets) {
                // 移除了预设，因此必须更新此文件夹的依赖关系。
                if (asset.EndsWith(".preset")) {
                    folders.Add(Path.GetDirectoryName(asset));
                }
            }

            foreach (var movedAsset in movedAssets) {
                // 移动了预设，因此必须更新上一个和新文件夹的依赖关系。
                if (movedAsset.destinationAssetPath.EndsWith(".preset")) {
                    folders.Add(Path.GetDirectoryName(movedAsset.destinationAssetPath));
                }

                if (movedAsset.sourceAssetPath.EndsWith(".preset")) {
                    folders.Add(Path.GetDirectoryName(movedAsset.sourceAssetPath));
                }
            }

            // 请勿无缘无故地添加依赖关系更新。
            if (folders.Count != 0) {
                // 依赖关系需要在 AssetPostprocessor 调用外部进行更新。
                // 向下一次编辑器更新注册方法。
                EditorApplication.delayCall += () => {
                    DelayedDependencyRegistration(folders);
                };
            }
        }

        /// <summary>
        /// 此方法会加载每个给定文件夹路径中的所有预设
        /// 并基于该文件夹中的当前预设更新 CustomDependency 哈希。
        /// </summary>
        static void DelayedDependencyRegistration (HashSet<string> folders) {
            foreach (var folder in folders) {
                var presetPaths =
                    AssetDatabase.FindAssets("glob:\"**.preset\"", new[] { folder })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Where(presetPath => Path.GetDirectoryName(presetPath) == folder)
                        .OrderBy(a => a);

                Hash128 hash = new Hash128();
                foreach (var presetPath in presetPaths) {
                    // 附加路径和预设类型以确保每当更改预设类型时都会重新导入资源。
                    hash.Append(presetPath);
                    hash.Append(AssetDatabase.LoadAssetAtPath<Preset>(presetPath).GetTargetFullTypeName());
                }

                AssetDatabase.RegisterCustomDependency($"PresetPostProcessor_{folder}", hash);
            }

            // 手动触发刷新
            // 以便 AssetDatabase 对更新的文件夹哈希触发依赖关系检查。
            AssetDatabase.Refresh();
        }
    }
#else
    public class EnforcePresetPostProcessor : AssetPostprocessor {
        void OnPreprocessAsset () {
            // 确保我们在第一次导入资源时应用预设。
            if (assetImporter.importSettingsMissing) {
                // 获取当前导入的资源文件夹。
                var path = Path.GetDirectoryName(assetPath);
                if (!string.IsNullOrEmpty(path)) {
                    // 是否为 Assets 文件夹下的资源
                    bool isAssetFolder = path.IndexOf("Assets/") > -1;
                    if (isAssetFolder) {
                        while (!string.IsNullOrEmpty(path)) {
                            // 查找此文件夹中的所有预设资源。
                            var presetGuids = AssetDatabase.FindAssets("t:Preset", new[] { path });
                            foreach (var presetGuid in presetGuids) {
                                // 确保不是在子文件夹中测试预设。
                                string presetPath = AssetDatabase.GUIDToAssetPath(presetGuid);
                                if (Path.GetDirectoryName(presetPath) == path) {
                                    //加载预设，然后尝试将其应用于导入器。
                                    var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
                                    if (preset.ApplyTo(assetImporter))
                                        return;
                                }
                            }
                            //在父文件夹中重试。
                            path = Path.GetDirectoryName(path);
                        }
                    }
                }
            }
        }
    }
}
#endif
#endif