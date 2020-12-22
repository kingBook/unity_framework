#pragma warning disable 0649
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PanelLogoScreen:MonoBehaviour{
	
	public event System.Action onFadeOutEvent;
	
	[SerializeField] private CanvasGroup m_canvasGroup;

	private void OnEnable(){
		m_canvasGroup.alpha=1f;
		Invoke(nameof(StartFadeOut),2f);
		
	}

	private void StartFadeOut(){
		m_canvasGroup.DOFade(0.0f,1f).OnComplete(OnFadeOut);
	}

	private void OnFadeOut(){
		gameObject.SetActive(false);
		onFadeOutEvent?.Invoke();
	}

	public void Active(){
		gameObject.SetActive(true);
	}
}
