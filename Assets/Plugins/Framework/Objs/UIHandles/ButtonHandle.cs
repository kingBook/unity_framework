using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/// <summary>
/// 按钮手柄
/// </summary>
public class ButtonHandle:BaseMonoBehaviour,IPointerDownHandler,IPointerUpHandler{
	
	public float idleAlpha=0.5f;
	public float activeAlpha=1.0f;
	
	[System.Serializable]
	public class MyEvent:UnityEvent<PointerEventData>{}
	public MyEvent onPointerDown;
	public MyEvent onPointerUp;
	
	private CanvasGroup m_canvasGroup;

	public bool isPointerDown{ get; private set;}
	
	protected override void Start(){
		base.Start();
		m_canvasGroup=gameObject.AddComponent<CanvasGroup>();
		m_canvasGroup.alpha=idleAlpha;
	}

	public void OnPointerDown(PointerEventData eventData){
		isPointerDown=true;
		m_canvasGroup.alpha=activeAlpha;
		
		onPointerDown?.Invoke(eventData);
	}

    public void OnPointerUp(PointerEventData eventData){
		isPointerDown=false;
		m_canvasGroup.alpha=idleAlpha;
		
		onPointerUp?.Invoke(eventData);
    }
	
	protected override void OnDestroy(){
		if(onPointerDown!=null)onPointerDown.RemoveAllListeners();
		if(onPointerUp!=null)onPointerUp.RemoveAllListeners();
		base.OnDestroy();
	}
	
	
    
}