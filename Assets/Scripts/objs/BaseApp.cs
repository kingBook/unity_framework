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

	[Tooltip("决定在Awake中是否调用init函数，" +
	 "\n用于调试其它场景需要调用该脚本，" +
	 "\n在Hierarchy中拖入该脚本所在的.unity文件时，" +
	 "\n不执行init(),不载入其他场景，不创建其他对象，" +
	 "\n发布工程时必须为true。")
	]
	[SerializeField]
	private bool _isCallInitOnAwake=true;

	[Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
	 "\nCN:中文 " +
	 "\nEN:英文")
	]
	[SerializeField]
	protected Language _language=Language.AUTO;
	private UpdateManager _updateManager;



	//禁止子类重写
	sealed protected override void Awake() {
		base.Awake();
		//再次加载场景时，如果已有实例则删除
		if(_instance==null){
			_instance=this as T;
			initSelf();
			if(_isCallInitOnAwake)init();
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

	//用于初始App和其它全局成员
	private void initSelf(){
		if(_language==Language.AUTO){
			initLanguage();
		}
		_updateManager=new UpdateManager();
	}
	private void initLanguage(){
		bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
		_language=isCN?Language.CN:Language.EN;
	}

	//仅供子类实现
	virtual protected void init(){}

	#region IUpdate Manager
	private void FixedUpdate(){
		_updateManager.fixedUpdate();
	}
	private void Update(){
		_updateManager.update();
	}
	private void LateUpdate(){
		_updateManager.lateUpdate();
	}
	private void OnGUI(){
		_updateManager.onGUI();
	}
	private void OnRenderObject(){
		_updateManager.onRenderObject();
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

	public UpdateManager updateManager { get => _updateManager; }
}
