Unity Framework



=================== Framework start ===================
Framework 待完善的功能: 
* App.instance.Delay()需要重新加回来，参考racingTravel游戏，因为需要在执行一些延时操作时，需要在游戏暂停时不计算时间，游戏恢复暂停时继续计算延时，App.instance.Delay()实现了这一功能
* RandomUtil.cs 中许多方法会分配内存，在运行时可能较慢
* ctrl+shift+d 复制关联预制件的对象时，会失去关联
* 扩展 Export Package 功能，使其能导出 Project Settings
* 解压 .fbx 材质时， 场景中引用此 .fbx 的对象被 "Unpack Prefab" 时，材质、网格等会失去关联
* 物理投射盒添加
* EditorReplaceDDSTextures.cs 现在需要解压 .fbx 的材质才能替换成功
* 在 Hierarchy 与 Project 面板中，按数字或字母时像 Windows 系统那样快速定位文件或对象
* PhysicsUtil.IsTouching(Collider collider1, Collider collider2);
* 人物刚体限制旋转角度
* AnimationCallbacks.cs
* jsfl导出动画空白帧时有错误
* jsfl导出的UI的Image动画图片大小不一样播放会出错
* jsfl导出是MaxRects无效与及在图集大于2048x2048时的MaxRects如何设置
* FbxAnimationSpliter 已切分的动画如果有被引用，再次切分时会丢失
* SceneLoader 场景加载器实现后台加载，进度条美化
* SceneLoader 需实现后台加载场景，能获取场景对象合并到其他对象
* SceneLoader 需发出加载完成并激活事件，能访问加载后场景的对象，如：场景中有多个Panel能实现在加载后立即显示哪个Panel隐藏哪此Panel

Spine运行库修改记录：
Spine\Runtime\spine-csharp\SkeletonJson.cs
Spine\Runtime\spine-csharp\SkeletonBinary.cs
在Spine中使用Linear颜色空间会出现不不兼容的情况，所以使用Gamma颜色空间（Player Settings->Player->Other Settings）
SpineSharderURP导入警告：
Shader warning in 'Universal Render Pipeline/2D/Spine/Sprite': Keyword PIXELSNAP_ON declared as global and local. It will be treated as local keyword.
=================== Framework end ===================