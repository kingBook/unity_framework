#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportProjectPackage : Editor {

    [MenuItem("Assets/Export Package (Assets And ProjectSettings)", false, 20)]
    private static void Execute () {
        Debug.Log("Export Package (Assets And ProjectSettings)");

        List<string> assetPathNames = new List<string>();
        string[] paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0, length = paths.Length; i < length; i++) {
            string path = paths[i];
            if (path.IndexOf("ProjectSettings") == 0) {
                assetPathNames.Add(path);
            } else if (path.IndexOf("Assets") == 0) {
                assetPathNames.Add(path);
            } else if (path.IndexOf("Packages") == 0) {

            } else if (path.IndexOf("Library") == 0) {
                
            } else {
                
            }
        }
        AssetDatabase.ExportPackage(assetPathNames.ToArray(), "Assets.unitypackage", ExportPackageOptions.Interactive);
    }

}
#endif
