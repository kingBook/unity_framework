using UnityEngine;
using System.Collections;
/// <summary>
/// 当App的语言和该脚本设置的一致则激活绑定该脚本的Gameobject,否则吊销
/// </summary>
public class LanguageSwitcher:BaseMonoBehaviour{
	[Tooltip("根据语言决定是否激活该对象，" +
	 "\n注意：只能是CN、EN，" +
	 "\n不可以设置为AUTO")
	]
	[SerializeField]
	protected Language _language=Language.EN;

	protected override void Start() {
		base.Start();
		bool isActive=_language==App.instance.language;
		gameObject.SetActive(isActive);
	}

#if UNITY_EDITOR
	protected override void OnValidate() {
		base.OnValidate();
		if(_language==Language.AUTO){
			Debug.LogError("在LanguageSwitcher中，language只能设置为EN/CN不能为AUTO");
		}
	}
#endif
}
