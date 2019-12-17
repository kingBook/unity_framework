using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 方向手柄(拖动中心的滑块控制方向)
/// <br>通过angleNormal属性,获取方向角度力，x、y值范围[-1,1]</br>
/// </summary>
public class DirectionDragHandle:BaseMonoBehaviour{
	public float idleAlpha=0.5f;
	public float activeAlpha=1.0f;

	[Tooltip("滑块")]
	public RectTransform handleRect;
	[Tooltip("滑块的父级")]
	public GameObject handleParent;
	
	[System.Serializable]
	public class MyEvent:UnityEvent<PointerEventData>{ }
	public MyEvent onEndDragEvent;
	
	/// <summary>当鼠标按下/接触开始时是否允许操作手柄在小范围内移动到鼠标/接触点位置</summary>
	private bool m_isMoveHandleOnTouchBegin=false;
	private float m_radius=0f;
	private Vector2 m_angleNormal=Vector2.zero;
	private Vector2 m_initPos;
	private int m_fingerId=-1;
	private RectTransform m_rectTransform;
	private CanvasGroup m_canvasGroup;
	private ScrollRect m_scrollRect;

	public Vector2 angleNormal{ get=>m_angleNormal;}

	protected override void Awake() {
		base.Awake();
		//添加ScrollRect组件，必须在添加EventTrigger之前，否则无法限制滑块拖动范围
		m_scrollRect=handleParent.AddComponent<ScrollRect>();
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
		entry.callback.AddListener((eventData)=>{OnDrag((PointerEventData)eventData);});
		eventTrigger.triggers.Add(entry);
		//onEndDrag
		entry=new EventTrigger.Entry();
		entry.eventID=EventTriggerType.EndDrag;
		entry.callback.AddListener((eventData)=>{OnEndDrag((PointerEventData)eventData);});
		eventTrigger.triggers.Add(entry);
		//
		m_rectTransform=handleParent.transform as RectTransform;
		m_canvasGroup=handleParent.AddComponent<CanvasGroup>();
	}

	protected override void Start(){
		base.Start();
		//计算摇杆块的半径
		m_radius=m_rectTransform.sizeDelta.x*0.5f;
		m_initPos=m_rectTransform.anchoredPosition;
		
		m_scrollRect.content=handleRect;
		m_canvasGroup.alpha=idleAlpha;
    }

	private void onBeginDrag(PointerEventData eventData){
		m_canvasGroup.alpha=activeAlpha;
	}

	private void OnDrag(PointerEventData eventData){
		var contentPostion=m_scrollRect.content.anchoredPosition;
		//限制滑块拖动的半径范围
		if (contentPostion.magnitude>m_radius){
			contentPostion=contentPostion.normalized*m_radius;
			m_scrollRect.content.anchoredPosition=contentPostion;
		}
		m_angleNormal.Set(contentPostion.x/m_radius,contentPostion.y/m_radius);
    }

	private void OnEndDrag(PointerEventData eventData){
		m_angleNormal=Vector2.zero;
		m_canvasGroup.alpha=idleAlpha;
		//
		onEndDragEvent?.Invoke(eventData);
	}

	protected override void Update2(){
		base.Update2();
		if(Input.touchSupported){
			Touch[] touchs=Input.touches;
			foreach(Touch touch in touchs){
				if(m_fingerId==-1){
					if(RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform,touch.position)){
						if(touch.phase==TouchPhase.Began){
							if(touch.position.x>m_initPos.x&&touch.position.y>m_initPos.y){
								if(m_isMoveHandleOnTouchBegin)MoveHandleToPos(touch.position);
								m_canvasGroup.alpha=activeAlpha;
								m_fingerId=touch.fingerId;
							}
						}
					}
				}else if(touch.fingerId==m_fingerId){
					if(touch.phase==TouchPhase.Ended){
						m_fingerId=-1;
						m_rectTransform.anchoredPosition=m_initPos;
						m_canvasGroup.alpha=idleAlpha;
					}
				}
			}
		}else{
			if(Input.GetMouseButtonDown(0)){
				Vector2 mousePos=Input.mousePosition;
				if(RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform,mousePos)){
					if(mousePos.x>m_initPos.x&&mousePos.y>m_initPos.y){
						if(m_isMoveHandleOnTouchBegin)MoveHandleToPos(mousePos);
						m_canvasGroup.alpha=activeAlpha;
					}
				}
			}else if(Input.GetMouseButtonUp(0)){
				m_rectTransform.anchoredPosition=m_initPos;
				m_canvasGroup.alpha=idleAlpha;
			}
		}
	}

	private void MoveHandleToPos(Vector2 pos){
		CanvasScaler canvasScaler=GetComponentInParent<CanvasScaler>();

		//屏幕分辨率与设计分辨率的缩放因子
		float scaleX=Screen.width/canvasScaler.referenceResolution.x;
		float scaleY=Screen.height/canvasScaler.referenceResolution.y;

		//加权平均值
		float averageValue=scaleX*(1-canvasScaler.matchWidthOrHeight)+scaleY*(canvasScaler.matchWidthOrHeight);

		pos/=averageValue;

		pos-=m_rectTransform.sizeDelta*0.5f;
		Vector2 offset=pos-m_rectTransform.offsetMin;

		m_rectTransform.offsetMin=pos;
		m_rectTransform.offsetMax=m_rectTransform.offsetMax+offset;
	}

	protected override void OnDestroy() {
		if(onEndDragEvent!=null){
			onEndDragEvent.RemoveAllListeners();
		}
		base.OnDestroy();
	}

}
