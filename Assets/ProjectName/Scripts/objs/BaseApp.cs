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

		protected override void Awake(){
			base.Awake();
			_instance=this as T;
			DontDestroyOnLoad(gameObject);
			
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
			_instance=null;
			base.OnDestroy();
		}
	}
}
