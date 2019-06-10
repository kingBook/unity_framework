namespace framework{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public enum Language{AUTO,CN,EN}
	
	/// <summary>
	/// <para>整个应用程序的单例类</para>
	/// </summary>
	public abstract class BaseApp<T>:BaseBehaviour where T:class,new(){
		
		[Tooltip("AUTO:运行时根据系统语言决定是CN/EN \nCN:中文 \nEN:英文")]
		[SerializeField]
		protected Language _language=Language.AUTO;

		protected static T _instance;
		public static T getInstance(){ return _instance; }

		//禁止子类重写
		sealed protected override void Awake(){
			base.Awake();
			//再次加载场景时，如果已有实例则删除
			if(_instance==null){
				_instance=this as T;
				DontDestroyOnLoad(gameObject);
				init();
			}else{
				Destroy(gameObject);
			}
		}

		//禁止子类重写
		sealed protected override void Start(){
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
		}

		private void initLanguage(){
			bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
			isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
			isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
			_language=isCN?Language.CN:Language.EN;
		}

		protected override void OnDestroy(){
			if(_instance!=null){
				if(_instance.Equals(this)){
					_instance=null;
				}
			}
			base.OnDestroy();
		}
	}
}
