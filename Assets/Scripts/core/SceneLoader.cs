using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class SceneLoader:BaseMonoBehaviour{
	[Tooltip("进度条滑块")]
	public Image imageMid;
	[Tooltip("百分比文本框")]
	public Text txt;

	private AsyncOperation _asyncOperation;

	protected override void Awake() {
		base.Awake();
		gameObject.SetActive(false);
	}

	public void load(string sceneName){
		gameObject.SetActive(true);
		imageMid.fillAmount=0;
		StartCoroutine(loadSceneAsync(sceneName));
	}

	IEnumerator loadSceneAsync(string sceneName){
		_asyncOperation=SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
		_asyncOperation.completed+=onComplete;
		_asyncOperation.allowSceneActivation=false;
		while(!_asyncOperation.isDone){
			float progress=_asyncOperation.progress;
			if(progress>=0.9f){
				_asyncOperation.allowSceneActivation=true;
				imageMid.fillAmount=1.0f;
				txt.text="loading 100%...";
			}else{
				imageMid.fillAmount=progress;
				txt.text="loading "+Mathf.FloorToInt(progress*100)+"%...";
			}
			yield return null;
		}
	}

	private void onComplete(AsyncOperation asyncOperation){
		gameObject.SetActive(false);
		_asyncOperation.completed-=onComplete;
		_asyncOperation=null;
	}

	protected override void OnDestroy(){
		if(_asyncOperation!=null){
			_asyncOperation.completed-=onComplete;
		}
		base.OnDestroy();
	}

}
