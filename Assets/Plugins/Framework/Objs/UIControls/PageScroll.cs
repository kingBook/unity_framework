#pragma warning disable 0649
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// 页滚动控制
/// </summary>
public class PageScroll:BaseMonoBehaviour,IPointerDownHandler,IPointerUpHandler{
	[SerializeField]
	private float m_smoothTime=0.2f;
	[SerializeField]
	private float m_enableControlSpeedLength=500f;
	[SerializeField]
	private ScrollRect m_scrollRect;
	private Scrollbar m_scrollbar;
	private int m_pageCount;
	private float m_scrollBarTargetValue=-1f;
	private float m_currentVelocity=0f;
	private bool m_isPointerDowning=false;

#if UNITY_EDITOR
	protected override void Reset() {
		base.Reset();
		m_scrollRect=GetComponent<ScrollRect>();
	}
#endif

	protected override void Awake(){
		base.Awake();
		m_pageCount=m_scrollRect.content.childCount;
		if(m_scrollRect.horizontal){
			m_scrollbar=m_scrollRect.horizontalScrollbar;
		}else if(m_scrollRect.vertical){
			m_scrollbar=m_scrollRect.verticalScrollbar;
		}
		m_scrollBarTargetValue=m_scrollbar.value;
	}

	protected override void Update2(){
		base.Update2();
		if(!m_isPointerDowning){
			if(m_scrollBarTargetValue<0){
				float speedLength=m_scrollRect.velocity.magnitude;
				if(speedLength<m_enableControlSpeedLength){
					int closetPageIndex=GetClosestPageIndex();
					m_scrollBarTargetValue=closetPageIndex*(1f/(m_pageCount-1));
				}
			}
			if(m_scrollBarTargetValue>=0){
				m_scrollbar.value=Mathf.SmoothDamp(m_scrollbar.value,m_scrollBarTargetValue,ref m_currentVelocity,m_smoothTime);
			}
		}
	}

	private int GetClosestPageIndex(){
		int pageIndex=0;
		float minDistance=float.MaxValue;
		for(int i=0;i<m_pageCount;i++){
			float scrollTargetValue=i * (1f/(m_pageCount-1));
			float distance=Mathf.Abs(m_scrollbar.value-scrollTargetValue);
			if(distance<minDistance){
				minDistance=distance;
				pageIndex=i;
			}
		}
		return pageIndex;
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData){
		m_isPointerDowning=true;
		m_scrollBarTargetValue=-1f;
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData){
		m_isPointerDowning=false;
	}
	
}
