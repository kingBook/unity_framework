#pragma warning disable 0649
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasWidthOrHeightAdapter:BaseMonoBehaviour{
	[SerializeField]
	private CanvasScaler m_canvasScaler;

	private float m_referenceScaleFactor;

#if UNITY_EDITOR
	protected override void Reset(){
		base.Reset();
		if(!m_canvasScaler){
			m_canvasScaler=GetComponent<CanvasScaler>();
		}
	}
#endif

	protected override void Start(){
		base.Start();
		m_referenceScaleFactor=m_canvasScaler.referenceResolution.x/m_canvasScaler.referenceResolution.y;
	}

	protected override void Update2(){
		base.Update2();
		float scaleFactor=(float)Screen.width/Screen.height;
		if(scaleFactor>m_referenceScaleFactor){
			m_canvasScaler.matchWidthOrHeight=1f;
		}else if(scaleFactor<m_referenceScaleFactor){
			m_canvasScaler.matchWidthOrHeight=0f;
		}
	}
}
