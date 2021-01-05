#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class DirectionInput:MonoBehaviour{
		
	public enum Mode{ Handle, Automatic }

	[SerializeField] private float m_disableAlpha=0.5f;
	[SerializeField] private float m_enableAlpha=1.0f;
	[SerializeField] private RectTransform m_touchableArea;
	[SerializeField] private RectTransform m_slidingArea;
	[SerializeField] private RectTransform m_handle;
	[SerializeField] private Mode m_mode=Mode.Automatic;

	private Vector2 m_directionSize;
	private float m_slidingRadiusOnStart;
	private bool m_enableHandle;
	private int m_fingerIdRecord=-1;
	private bool m_inTouchableAreaTouchDown;
	private Canvas m_canvas;
	private CanvasGroup m_canvasGroupSliginArea;
	private float m_screenScaleFactorOnStart;

	/// <summary> 输入的方向的大小，值区间[-1,1]，表示输入的方向、大小（滑块离中心点越远越大） </summary>
	public Vector2 directionSize=>m_directionSize;
	/// <summary> 输入的方向单位化向量，值区间[-1,1]，表示输入的方向，不表示大小 </summary>
	public Vector2 directionNormalized=>m_directionSize.normalized;

	private void Awake(){
		m_canvas=GetComponentInParent<Canvas>();
		m_canvasGroupSliginArea=m_slidingArea.GetComponent<CanvasGroup>();
		//隐藏可触摸区域 Image
		Image imageTouchableArea=m_touchableArea.GetComponent<Image>();
		Color color=imageTouchableArea.color;
		color.a=0f;
		imageTouchableArea.color=color;
		//根据模式设置 m_enableHandle
		if(m_mode==Mode.Automatic){
			m_enableHandle=Input.touchSupported;
		}else if(m_mode==Mode.Handle){
			m_enableHandle=true;
		}
		//设置默认的透明度
		m_canvasGroupSliginArea.alpha=m_disableAlpha;
		//根据 m_enabledHandle 显示/隐藏手柄
		m_touchableArea.gameObject.SetActive(m_enableHandle);
		m_canvasGroupSliginArea.gameObject.SetActive(m_enableHandle);
	}

	private void Start(){
		m_screenScaleFactorOnStart=m_canvas.scaleFactor;
		//记录可滑动的半径(必须在 Awake 之后记录，否则 Canvas 未计算适配会出错)
		m_slidingRadiusOnStart=m_slidingArea.sizeDelta.x*0.5f*m_canvas.scaleFactor;
	}

	private void Update(){
		if(m_enableHandle){
			//屏幕大小发生变化时
			if(m_canvas.scaleFactor!=m_screenScaleFactorOnStart){
				OnScreenSizeChanged();
			}
			//检测UI手柄输入
			CheckUiHandleInput();
			//释放手柄后回到中心
			if(!m_inTouchableAreaTouchDown){
				MoveHandleToScreenPoint(Vector3.Lerp(m_handle.position,m_slidingArea.position,0.6f));
					
			}
		}else{
			//检测pc键盘等输入
			CheckAxisInput();
		}
	}

	#region UI Handle

	private void MoveHandleToScreenPoint(Vector3 screenPoint){
		Vector3 relative=screenPoint-m_slidingArea.position;
		relative=Vector3.ClampMagnitude(relative,m_slidingRadiusOnStart);
		relative.z=0f;
		m_handle.position=m_slidingArea.position+relative;
		//计算输出方向的大小
		m_directionSize=relative/m_slidingRadiusOnStart;
	}

	private void OnScreenSizeChanged(){
		//重新计算并记录可滑动半径范围
		float scale=m_canvas.scaleFactor/m_screenScaleFactorOnStart;
		m_screenScaleFactorOnStart=m_canvas.scaleFactor;
		m_slidingRadiusOnStart*=scale;
	}

	private void CheckUiHandleInput(){
		if(Input.touchSupported){
			if(m_inTouchableAreaTouchDown){
				//触摸按下过程中...
				Touch touch=InputUtil.GetTouchWithFingerId(m_fingerIdRecord,false,TouchPhase.Moved,TouchPhase.Stationary);
				if(touch.fingerId>-1){ 
					OnUiInputTouchMoved(touch.position);
				｝
				//判断触摸释放
				touch=InputUtil.GetTouchWithFingerId(m_fingerIdRecord,false,TouchPhase.Canceled,TouchPhase.Ended);
				if(touch.fingerId>-1){
					OnUiInputTouchEnded(touch.position);
				}
			}else{
				//判断触摸按下
				Touch touch=InputUtil.GetFirstTouch(TouchPhase.Began,false);
				if(touch.fingerId>-1){
					bool inTouchableArea=RectTransformUtility.RectangleContainsScreenPoint(m_touchableArea,touch.position);
					if(inTouchableArea){
						m_fingerIdRecord=touch.fingerId;
						OnUiInputTouchBegan(touch.position);
					}
				}
			}
		}else{
			if(m_inTouchableAreaTouchDown){
				//鼠标按下过程中...
				OnUiInputTouchMoved(Input.mousePosition);
				//判断鼠标释放
				if(Input.GetMouseButtonUp(0)){
					OnUiInputTouchEnded(Input.mousePosition);
				}
			}else{
				//判断鼠标按下
				if(Input.GetMouseButtonDown(0)){
					bool inTouchableArea=RectTransformUtility.RectangleContainsScreenPoint(m_touchableArea,Input.mousePosition);
					if(inTouchableArea){
						OnUiInputTouchBegan(Input.mousePosition);
					}
				}
			}
		}
	}

	private void OnUiInputTouchBegan(Vector2 screenPoint){
		m_inTouchableAreaTouchDown=true;
		m_canvasGroupSliginArea.alpha=m_enableAlpha;
		MoveHandleToScreenPoint(screenPoint);
	}

	private void OnUiInputTouchMoved(Vector2 screenPoint){
		bool inTouchableArea=RectTransformUtility.RectangleContainsScreenPoint(m_touchableArea,screenPoint);
		if(inTouchableArea){
			MoveHandleToScreenPoint(screenPoint);
		}else{
			//触摸移动超出可触摸区域时强制释放
			OnUiInputTouchEnded(screenPoint);
		}
	}

	private void OnUiInputTouchEnded(Vector2 screenPoint){
		m_inTouchableAreaTouchDown=false;
		m_fingerIdRecord=-1;
		m_canvasGroupSliginArea.alpha=m_disableAlpha;
	}
		
	#endregion 

	private void CheckAxisInput(){
		m_directionSize.x=Input.GetAxis("Horizontal");
		m_directionSize.y=Input.GetAxis("Vertical");
	}
}
