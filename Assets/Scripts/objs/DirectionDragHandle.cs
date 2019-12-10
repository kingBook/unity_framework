using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 方向手柄(拖动中心的滑块控制方向)
/// <br>通过angleNormal属性,获取方向角度力，x、y值范围[-1,1]</br>
/// </summary>
public class DirectionDragHandle:BaseMonoBehaviour{
	[System.Serializable]
	public class MyEvent:UnityEvent<PointerEventData>{ }

	[Tooltip("滑块")]
	public RectTransform handleRect;
	[Tooltip("滑块的父级")]
	public GameObject handleParent;
	
	public MyEvent onEndDragEvent;
	
	private readonly float idleAlpha=0.5f;
	private readonly float activeAlpha=1.0f;
	/// <summary>当鼠标按下/接触开始时是否允许操作手柄在小范围内移动到鼠标/接触点位置</summary>
	private bool _isMoveHandleOnTouchBegin=false;
	private float _radius=0f;
	private Vector2 _angleNormal=Vector2.zero;
	private Vector2 _initPos;
	private int _fingerId=-1;
	private RectTransform _rectTransform;
	private CanvasGroup _canvasGroup;
	private ScrollRect _scrollRect;

	protected override void Awake() {
		base.Awake();
		//添加ScrollRect组件，必须在添加EventTrigger之前，否则无法限制滑块拖动范围
		_scrollRect=handleParent.AddComponent<ScrollRect>();
		//侦听onBeginDrag、onDrag、onEndDrag
		EventTrigger eventTrigger=handleParent.AddComponent<EventTrigger>();
		//onBeginDrag
		EventTrigger.Entry entry=new EventTrigger.Entry();
		entry.eventID=EventTriggerType.BeginDrag;
		entry.callback.AddListener((eventData)=>{onBeginDrag((PointerEventData)eventData);});
		eventTrigger.triggers.Add(entry);
		//onDrag
		entry=new EventTrigger.Entry();
		entry.eventID=EventTriggerType.Drag;
		entry.callback.AddListener((eventData)=>{onDrag((PointerEventData)eventData);});
		eventTrigger.triggers.Add(entry);
		//onEndDrag
		entry=new EventTrigger.Entry();
		entry.eventID=EventTriggerType.EndDrag;
		entry.callback.AddListener((eventData)=>{onEndDrag((PointerEventData)eventData);});
		eventTrigger.triggers.Add(entry);
		//
		_rectTransform=handleParent.transform as RectTransform;
		_canvasGroup=handleParent.AddComponent<CanvasGroup>();
	}

	protected override void Start(){
		base.Start();
		//计算摇杆块的半径
		_radius=_rectTransform.sizeDelta.x*0.5f;
		_initPos=_rectTransform.anchoredPosition;
		
		_scrollRect.content=handleRect;
		_canvasGroup.alpha=idleAlpha;
    }

	private void onBeginDrag(PointerEventData eventData){
		_canvasGroup.alpha=activeAlpha;
	}

	private void onDrag(PointerEventData eventData){
		var contentPostion=_scrollRect.content.anchoredPosition;
		//限制滑块拖动的半径范围
		if (contentPostion.magnitude>_radius){
			contentPostion=contentPostion.normalized*_radius;
			_scrollRect.content.anchoredPosition=contentPostion;
		}
		_angleNormal.Set(contentPostion.x/_radius,contentPostion.y/_radius);
    }

	private void onEndDrag(PointerEventData eventData){
		_angleNormal=Vector2.zero;
		_canvasGroup.alpha=idleAlpha;
		//
		onEndDragEvent?.Invoke(eventData);
	}

	protected override void Update2(){
		base.Update2();
		if(Input.touchSupported){
			Touch[] touchs=Input.touches;
			foreach(Touch touch in touchs){
				if(_fingerId==-1){
					if(RectTransformUtility.RectangleContainsScreenPoint(_rectTransform,touch.position)){
						if(touch.phase==TouchPhase.Began){
							if(touch.position.x>_initPos.x&&touch.position.y>_initPos.y){
								if(_isMoveHandleOnTouchBegin)moveHandleToPos(touch.position);
								_canvasGroup.alpha=activeAlpha;
								_fingerId=touch.fingerId;
							}
						}
					}
				}else if(touch.fingerId==_fingerId){
					if(touch.phase==TouchPhase.Ended){
						_fingerId=-1;
						_rectTransform.anchoredPosition=_initPos;
						_canvasGroup.alpha=idleAlpha;
					}
				}
			}
		}else{
			if(Input.GetMouseButtonDown(0)){
				Vector2 mousePos=Input.mousePosition;
				if(RectTransformUtility.RectangleContainsScreenPoint(_rectTransform,mousePos)){
					if(mousePos.x>_initPos.x&&mousePos.y>_initPos.y){
						if(_isMoveHandleOnTouchBegin)moveHandleToPos(mousePos);
						_canvasGroup.alpha=activeAlpha;
					}
				}
			}else if(Input.GetMouseButtonUp(0)){
				_rectTransform.anchoredPosition=_initPos;
				_canvasGroup.alpha=idleAlpha;
			}
		}
	}

	private void moveHandleToPos(Vector2 pos){
		CanvasScaler canvasScaler=GetComponentInParent<CanvasScaler>();

		//屏幕分辨率与设计分辨率的缩放因子
		float scaleX=Screen.width/canvasScaler.referenceResolution.x;
		float scaleY=Screen.height/canvasScaler.referenceResolution.y;

		//加权平均值
		float averageValue=scaleX*(1-canvasScaler.matchWidthOrHeight)+scaleY*(canvasScaler.matchWidthOrHeight);

		pos/=averageValue;

		pos-=_rectTransform.sizeDelta*0.5f;
		Vector2 offset=pos-_rectTransform.offsetMin;

		_rectTransform.offsetMin=pos;
		_rectTransform.offsetMax=_rectTransform.offsetMax+offset;
	}

	protected override void OnDestroy() {
		if(onEndDragEvent!=null){
			onEndDragEvent.RemoveAllListeners();
		}
		base.OnDestroy();
	}

	public Vector2 angleNormal{ get=>_angleNormal;}

}
