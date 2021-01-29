#pragma warning disable 0649
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PanelLogoScreen:MonoBehaviour{
	
	public event System.Action onFadeOutEvent;
	
	[SerializeField]
	private CanvasGroup m_canvasGroup;

	private void OnEnable(){
		m_canvasGroup.alpha=1f;
		SceneManager.sceneLoaded+=OnSceneLoaded;
	}

	private void OnDisable(){
		SceneManager.sceneLoaded-=OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene,LoadSceneMode mode){
		if(scene.path!=gameObject.scene.path){
			if(gameObject.activeInHierarchy){
				SceneManager.sceneLoaded-=OnSceneLoaded;
				StartFadeOut();
			}
		}
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
