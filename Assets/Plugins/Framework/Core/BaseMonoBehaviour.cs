﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 所有GameObject脚本的抽象类(基类)
/// <br>子类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public abstract class BaseMonoBehaviour:MonoBehaviour,IUpdate{
	
	/// <summary>是否已经添加到<see cref="UpdateManager"/></summary>
	private bool m_hasAddToUpdateManager=false;

	/// <summary>表示是否已销毁</summary>
	public bool isDestroyed{ get; private set; }

	private void AddToUpdateManager(){
		if(m_hasAddToUpdateManager)return;
		m_hasAddToUpdateManager=true;
		
		App.instance.updateManager.Add(this);
	}
		

	#region Event Function
	/// <summary>
	/// 始终在任何 Start 函数之前并在实例化预制件之后调用此函数。
	/// 如果游戏对象在启动期间处于非活动状态，则在激活之后才会调用 Awake。）
	/// </summary>
	protected virtual void Awake(){
		
	}

	/// <summary>
	/// （仅在对象处于激活状态时调用）在启用对象后立即调用此函数。
	/// 在创建 MonoBehaviour 实例时（例如加载关卡或实例化具有脚本组件的游戏对象时）会执行此调用。
	/// </summary>
	protected virtual void OnEnable(){
		
	}

	/// <summary>
	/// 仅当启用脚本实例后，才会在第一次帧更新之前调用 Start。
	/// </summary>
	protected virtual void Start(){
		AddToUpdateManager();
	}

	/// <summary>
	/// 调用 FixedUpdate 的频度常常超过 Update。
	/// 如果帧率很低，可以每帧调用该函数多次；
	/// 如果帧率很高，可能在帧之间完全不调用该函数。
	/// 在 FixedUpdate 之后将立即进行所有物理计算和更新。
	/// 在 FixedUpdate 内应用运动计算时，无需将值乘以 Time.deltaTime。
	/// 这是因为 FixedUpdate 的调用基于可靠的计时器（独立于帧率）。
	/// </summary>
	//virtual protected void FixedUpdate(){}
	protected virtual void FixedUpdate2(){ }
	void IUpdate.FixedUpdate(){
		if(!gameObject.activeInHierarchy||!gameObject.activeSelf||!enabled)return;
		FixedUpdate2();
	}

	/// <summary>
	/// 如果 MonoBehaviour 已启用，则在每一帧都调用 Update
	/// </summary>
	//virtual protected void Update(){}
	protected virtual void Update2(){ }
	void IUpdate.Update(){
		if(!gameObject.activeInHierarchy||!gameObject.activeSelf||!enabled)return;
		Update2();
	}

	/// <summary>
	/// 每帧调用一次 LateUpdate()（在 Update()完成后）。
	/// LateUpdate 开始时，在 Update 中执行的所有计算便已完成。
	/// LateUpdate 的常见用途是跟随第三人称摄像机。
	/// 如果在 Update 内让角色移动和转向，可以在 LateUpdate 中执行所有摄像机移动和旋转计算。
	/// 这样可以确保角色在摄像机跟踪其位置之前已完全移动。
	/// </summary>
	//virtual protected void LateUpdate(){}
	protected virtual void LateUpdate2(){ }
	void IUpdate.LateUpdate(){
		if(!gameObject.activeInHierarchy||!gameObject.activeSelf||!enabled)return;
		LateUpdate2();
	}

	/// <summary>
	/// 当此碰撞器/刚体开始接触另一个刚体/碰撞器时，调用 OnCollisionEnter
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionEnter(Collision collision){}
		
	/// <summary>
	/// 当此碰撞器 2D/刚体 2D 开始接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionEnter2D (仅限 2D 物理)
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionEnter2D(Collision2D collision){}

	/// <summary>
	/// 当此碰撞器/刚体停止接触另一刚体/碰撞器时调用 OnCollisionExit
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionExit(Collision collision){}

	/// <summary>
	/// 当此碰撞器 2D/刚体 2D 停止接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionExit2D (仅限 2D 物理)
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionExit2D(Collision2D collision) {}

	/// <summary>
	/// 每当此碰撞器/刚体接触到刚体/碰撞器时，OnCollisionStay 将在每一帧被调用一次
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionStay(Collision collision){}

	/// <summary>
	/// 每当碰撞器 2D/刚体 2D 接触到刚体 2D/碰撞器 2D 时，OnCollisionStay2D 将在每一帧被调用一次(仅限 2D 物理)
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionStay2D(Collision2D collision){}

	/// <summary>
	/// 渲染和处理 GUI 事件时调用 OnGUI
	/// </summary>
	//virtual protected void OnGUI(){}
	protected virtual void OnGUI2(){}
	void IUpdate.OnGUI(){
		if(!gameObject.activeInHierarchy||!gameObject.activeSelf||!enabled)return;
		OnGUI2();
	}
	
	/// <summary>
	/// 当行为被禁用或处于非活动状态时调用此函数
	/// </summary>
	protected virtual void OnDisable(){}

#if UNITY_STANDALONE||UNITY_WEBGL
	/// <summary>
	/// 当用户在 GUIElement 或碰撞器上按鼠标按钮时调用 OnMouseDown
	/// </summary>
	protected virtual void OnMouseDown(){}

	/// <summary>
	/// 当用户在 GUIElement 或碰撞器上单击鼠标并保持按住鼠标时调用 OnMouseDrag
	/// </summary>
	protected virtual void OnMouseDrag(){}

	/// <summary>
	/// 当鼠标进入 GUIElement 或碰撞器时调用 OnMouseEnter
	/// </summary>
	protected virtual void OnMouseEnter(){}

	/// <summary>
	/// 当鼠标不再停留在 GUIElement 或碰撞器上时调用 OnMouseExit
	/// </summary>
	protected virtual void OnMouseExit(){}

	/// <summary>
	/// 当鼠标停留在 GUIElement 或碰撞器上时每帧都调用 OnMouseOver
	/// </summary>
	protected virtual void OnMouseOver(){}

	/// <summary>
	/// 当用户松开鼠标按钮时调用 OnMouseUp
	/// </summary>
	protected virtual void OnMouseUp(){}

	/// <summary>
	/// 仅当在同一 GUIElement 或碰撞器上按下鼠标，在松开时调用 OnMouseUpAsButton
	/// </summary>
	protected virtual void OnMouseUpAsButton(){}
#endif

	/// <summary>
	/// 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
	/// </summary>
	/// <param name="other"></param>
	protected virtual void OnTriggerEnter(Collider other){}

	/// <summary>
	/// 如果另一个碰撞器 2D 进入了触发器，则调用 OnTriggerEnter2D (仅限 2D 物理)
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnTriggerEnter2D(Collider2D collision){}

	/// <summary>
	/// 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
	/// </summary>
	/// <param name="other"></param>
	protected virtual void OnTriggerExit(Collider other){}

	/// <summary>
	/// 如果另一个碰撞器 2D 停止接触触发器，则调用 OnTriggerExit2D (仅限 2D 物理)
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnTriggerExit2D(Collider2D collision){}

	/// <summary>
	/// 对于触动触发器的所有“另一个碰撞器”，OnTriggerStay 将在每一帧被调用一次
	/// </summary>
	/// <param name="other"></param>
	protected virtual void OnTriggerStay(Collider other){}

	/// <summary>
	/// 如果其他每个碰撞器 2D 接触触发器，OnTriggerStay2D 将在每一帧被调用一次(仅限 2D 物理)
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnTriggerStay2D(Collider2D collision){}

	/// <summary>
	/// 设置动画 IK 的回叫(反向运动学)
	/// </summary>
	/// <param name="layerIndex"></param>
	protected virtual void OnAnimatorIK(int layerIndex){}

	/// <summary>
	/// 在此状态计算机和动画均已求值后但在 OnAnimatorIK 之前，将在每一帧都调用此回叫
	/// </summary>
	protected virtual void OnAnimatorMove(){}

	/// <summary>
	/// 当玩家获得或失去焦点时发送给所有游戏对象
	/// </summary>
	/// <param name="focus"></param>
	protected virtual void OnApplicationFocus(bool focus){}

	/// <summary>
	/// 当玩家暂停时发送给所有游戏对象
	/// </summary>
	/// <param name="pause"></param>
	protected virtual void OnApplicationPause(bool pause){}

	/// <summary>
	/// 应用程序退出前发送给所有游戏对象
	/// </summary>
	protected virtual void OnApplicationQuit(){}

	/// <summary>
	/// 如果已实现 OnAudioFilterRead，则 Unity 将向音频 DSP 链插入自定义筛选器
	/// </summary>
	/// <param name="data"></param>
	/// <param name="channels"></param>
	protected virtual void OnAudioFilterRead(float[] data,int channels){}

	/// <summary>
	/// 当呈现器在任何照相机上都不可见时调用 OnBecameInvisible
	/// </summary>
	protected virtual void OnBecameInvisible(){}

	/// <summary>
	/// 当呈现器在任何照相机上可见时调用 OnBecameVisible
	/// </summary>
	protected virtual void OnBecameVisible(){}

	/// <summary>
	/// 转换父级更改发生前将回叫发送到图形
	/// </summary>
	protected virtual void OnBeforeTransformParentChanged(){}

	/// <summary>
	/// 如果更改了画布组合，则发送回叫
	/// </summary>
	protected virtual void OnCanvasGroupChanged(){}

	/// <summary>
	/// 当成功连接到服务器时，在客户端调用
	/// </summary>
	protected virtual void OnConnectedToServer(){}

	/// <summary>
	/// 如果控制器在执行移动时击中碰撞器，则调用 OnControllerColliderHit
	/// </summary>
	/// <param name="hit"></param>
	protected virtual void OnControllerColliderHit(ControllerColliderHit hit){}

	/// <summary>
	/// 如果你希望仅在对象被选中时绘制 gizmos，请实现此 OnDrawGizmosSelected
	/// </summary>
	protected virtual void OnDrawGizmos(){}

	/// <summary>
	/// 如果你想要绘制可选取并始终绘制的 gizmos，请实现此 OnDrawGizmos
	/// </summary>
	protected virtual void OnDrawGizmosSelected(){}

	/// <summary>
	/// 当附加到同一游戏对象的联接断开时调用
	/// </summary>
	/// <param name="breakForce"></param>
	protected virtual void OnJointBreak(float breakForce){}

	/// <summary>
	/// 当附加到同一游戏对象的 Joint2D 断开时调用(仅限 2D 物理)
	/// </summary>
	/// <param name="joint"></param>
	protected virtual void OnJointBreak2D(Joint2D joint){}

	/// <summary>
	/// 当粒子击中碰撞器时调用 OnParticleCollision
	/// </summary>
	/// <param name="other"></param>
	protected virtual void OnParticleCollision(GameObject other){}

	/// <summary>
	/// 当粒子系统中的任意粒子满足触发模块的条件时调用
	/// </summary>
	protected virtual void OnParticleTrigger(){}

	/// <summary>
	/// 照相机完成场景渲染后调用 OnPostRender
	/// </summary>
	protected virtual void OnPostRender(){}

	/// <summary>
	/// 在照相机裁剪场景前调用 OnPreCull
	/// </summary>
	protected virtual void OnPreCull(){}

	/// <summary>
	/// 在照相机开始渲染场景前调用 OnPreRender
	/// </summary>
	protected virtual void OnPreRender(){}

	/// <summary>
	/// 如果关联的 RectTransform 的维度发生更改，则发送回叫
	/// </summary>
	protected virtual void OnRectTransformDimensionsChange(){}

	/// <summary>
	/// 如果关联的 RectTransform 被删除，则发送回叫
	/// </summary>
	protected virtual void OnRectTransformRemoved(){}

	/// <summary>
	/// OnRenderImage 在所有渲染完成后被调用，以对图片进行额外渲染
	/// 注意：当脚本绑定到Camera时，如果注册该函数且不实现任何操作会导致Game视图黑屏
	/// </summary>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	//protected virtual void OnRenderImage(RenderTexture source,RenderTexture destination){}

	/// <summary>
	/// 照相机渲染场景后调用 OnRenderObject
	/// </summary>
	//virtual protected void OnRenderObject(){}
	protected virtual void OnRenderObject2(){}
	void IUpdate.OnRenderObject(){
		if(!gameObject.activeInHierarchy||!gameObject.activeSelf||!enabled)return;
		OnRenderObject2();
	}

	/// <summary>
	/// 无论何时调用并完成 Network.InitializeServer，都在该服务器上调用
	/// </summary>
	protected virtual void OnServerInitialized(){}

	/// <summary>
	/// 回叫在转换子级发生更改后发送到图形
	/// </summary>
	protected virtual void OnTransformChildrenChanged(){}

	/// <summary>
	/// 回叫在转换父级发生更改后发送到图形
	/// </summary>
	protected virtual void OnTransformParentChanged(){}

	/// <summary>
	/// 当该脚本被加载或检视面板的值被修改时调用此函数(仅在编辑器中调用)
	/// </summary>
	protected virtual void OnValidate(){}

	/// <summary>
	/// 如果对象可见，则每个照相机都会调用一次 OnWillRenderObject
	/// </summary>
	protected virtual void OnWillRenderObject(){}

	/// <summary>
	/// 重置为默认值
	/// </summary>
	protected virtual void Reset(){}

	/// <summary>
	/// 当 MonoBehaviour 将被销毁时调用此函数
	/// </summary>
	protected virtual void OnDestroy(){
		isDestroyed=true;
		App.instance.updateManager.Remove(this);
	}
	#endregion

	
}