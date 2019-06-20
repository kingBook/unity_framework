using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language{AUTO,CN,EN}
	
/// <summary>
/// <para>整个应用程序的单例类</para>
/// </summary>
public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
	
	protected static T _instance;
	public static T getInstance(){
		return _instance; 
	}

	[Tooltip("AUTO:运行时根据系统语言决定是CN/EN \nCN:中文 \nEN:英文")]
	[SerializeField]
	protected Language _language=Language.AUTO;
	private UpdateManager _updateMgr;



	//禁止子类重写
	sealed protected override void Awake() {
		base.Awake();
		//再次加载场景时，如果已有实例则删除
		if(_instance==null){
			_instance=this as T;
			init();
		}else{
			Destroy(gameObject);
		}
	}

	//禁止子类重写
	sealed protected override void Start() {
		base.Start();
	}

	//禁止子类重写
	sealed protected override void init(Dictionary<string,object> info){
		base.init(info);
	}

	virtual protected void init(){
		if(_language==Language.AUTO){
			initLanguage();
		}
		_updateMgr=new UpdateManager();
	}

	private void initLanguage(){
		bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
		_language=isCN?Language.CN:Language.EN;
	}

	#region IUpdate Manager
	private void FixedUpdate(){
		_updateMgr.fixedUpdate();
	}
	private void Update(){
		_updateMgr.update();
	}
	private void LateUpdate(){
		_updateMgr.lateUpdate();
	}
	private void OnGUI(){
		_updateMgr.onGUI();
	}
	private void OnRenderObject(){
		_updateMgr.onRenderObject();
	}
	#endregion

	protected override void OnDestroy(){
		if(_instance!=null){
			if(_instance.Equals(this)){
				_instance=null;
			}
		}
		base.OnDestroy();
	}

	public UpdateManager updateMgr { get => _updateMgr; }
}
