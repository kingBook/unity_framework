using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveToOnCanvas:MonoBehaviour{
	public float delayOnStart;		 //间隔一指定的秒数才开始
	public float duration=2.0f;		 //持续时间
	public Vector2 position;		 //设置分辨率下的AnchorPosition
	public Vector3 scale=Vector3.one;//目标缩放比例
	public bool isDestroyOnComplete;
	
	private Canvas m_cavans;
	private CanvasScaler m_cavansScaler;
	private RectTransform m_rectTransform;
	
	private void Awake(){
		m_cavans=GetComponentInParent<Canvas>();
		m_cavansScaler=GetComponentInParent<CanvasScaler>();
		m_rectTransform=GetComponent<RectTransform>();
	}
	
	private void Start(){
		if(delayOnStart>0){
			Invoke(nameof(StartTween),delayOnStart);
		}else{
			StartTween();
		}
	}
	
	private void StartTween(){
		float scaleFactor=m_cavans.scaleFactor;
		Vector2 screenSize=new Vector2(Screen.width,Screen.height);
		
		Vector2 realSize=screenSize/scaleFactor;//Canvas实际大小(像素为单位)
		Vector2 realScale=realSize/m_cavansScaler.referenceResolution;//计算出实际缩放比
		position*=realScale;
		
		m_rectTransform.DOScale(scale,duration);
		m_rectTransform.DOAnchorPos(position,duration).OnComplete(OnComplete);
	}
	
	
	private void OnComplete(){
		if(isDestroyOnComplete){
			Destroy(gameObject);
		}
	}
}