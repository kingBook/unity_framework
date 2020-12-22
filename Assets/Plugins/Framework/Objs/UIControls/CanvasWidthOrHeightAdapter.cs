#pragma warning disable 0649
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasWidthOrHeightAdapter:MonoBehaviour{
	
	[SerializeField] private CanvasScaler m_canvasScaler;

	private float m_referenceScaleFactor;

#if UNITY_EDITOR
	private void Reset(){
		if(!m_canvasScaler){
			m_canvasScaler=GetComponent<CanvasScaler>();
		}
	}
#endif

	private void Start(){
		m_referenceScaleFactor=m_canvasScaler.referenceResolution.x/m_canvasScaler.referenceResolution.y;
	}

	private void Update(){
		float scaleFactor=(float)Screen.width/Screen.height;
		if(scaleFactor>m_referenceScaleFactor){
			//匹配高度
			m_canvasScaler.matchWidthOrHeight=1f;
		}else if(scaleFactor<m_referenceScaleFactor){
			//匹配宽度
			m_canvasScaler.matchWidthOrHeight=0f;
		}
	}
}
