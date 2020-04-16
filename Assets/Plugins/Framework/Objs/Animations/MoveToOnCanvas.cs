using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveToOnCanvas:BaseMonoBehaviour{
	public float duration=2.0f;
	[Tooltip("设置分辨率下的AnchorPosition")]
	public Vector2 position;
	public Vector3 scale=Vector3.one;
	public bool isDestroyOnComplete;
	
	private Canvas m_cavans;
	private CanvasScaler m_cavansScaler;
	private RectTransform m_rectTransform;
	
	protected override void Awake(){
		base.Awake();
		m_cavans=GetComponentInParent<Canvas>();
		m_cavansScaler=GetComponentInParent<CanvasScaler>();
		m_rectTransform=GetComponent<RectTransform>();
	}
	
	protected override void Start(){
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