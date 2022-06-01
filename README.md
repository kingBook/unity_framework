Unity Framework

SpineSharderURP导入警告：
Shader warning in 'Universal Render Pipeline/2D/Spine/Sprite': Keyword PIXELSNAP_ON declared as global and local. It will be treated as local keyword.

待完善的功能: 
* ctrl+shift+d 复制关联预制件的对象时，会失去关联
* 扩展 Export Package 功能，使其能导出 Project Settings
* 解压 .fbx 材质时， 场景中引用此 .fbx 的对象被 "Unpack Prefab" 时，材质、网格等会失去关联
* 物理投射盒添加
* 场景加载器实现后台加载，进度条美化

* EditorReplaceDDSTextures.cs 现在需要解压 .fbx 的材质才能替换成功
* 在 Hierarchy 与 Project 面板中，按数字或字母时像 Windows 系统那样快速定位文件或对象
* PhysicsUtil.IsTouching(Collider collider1, Collider collider2);
* 人物刚体限制旋转角度
* AnimationCallbacks.cs
* jsfl导出动画空白帧时有错误
* jsfl导出的UI的Image动画图片大小不一样播放会出错
* jsfl导出是MaxRects无效与及在图集大于2048x2048时的MaxRects如何设置
* FbxAnimationSpliter 已切分的动画如果有被引用，再次切分时会丢失


Spine运行库修改记录：
Spine\Runtime\spine-csharp\SkeletonJson.cs
Spine\Runtime\spine-csharp\SkeletonBinary.cs

在Spine中使用Linear颜色空间会出现不不兼容的情况，所以使用Gamma颜色空间（Player Settings->Player->Other Settings）