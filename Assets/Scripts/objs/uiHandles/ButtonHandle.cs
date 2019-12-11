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
	
	private CanvasGroup _canvasGroup;
	private bool _isPointerDown;
	
	protected override void Start(){
		base.Start();
		_canvasGroup=gameObject.AddComponent<CanvasGroup>();
		_canvasGroup.alpha=idleAlpha;
	}

	public void OnPointerDown(PointerEventData eventData){
		_isPointerDown=true;
		_canvasGroup.alpha=activeAlpha;
		
		onPointerDown?.Invoke(eventData);
	}

    public void OnPointerUp(PointerEventData eventData){
		_isPointerDown=false;
		_canvasGroup.alpha=idleAlpha;
		
		onPointerUp?.Invoke(eventData);
    }
	
	protected override void OnDestroy(){
		if(onPointerDown!=null)onPointerDown.RemoveAllListeners();
		if(onPointerUp!=null)onPointerUp.RemoveAllListeners();
		base.OnDestroy();
	}
	
	public bool isPointerDown{ get=>_isPointerDown; }
    
}