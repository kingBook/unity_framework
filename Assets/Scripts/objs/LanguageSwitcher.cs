using UnityEngine;
using System.Collections;

public class LanguageSwitcher:BaseMonoBehaviour{
	[Tooltip("根据语言决定是否激活该对象，" +
	 "\n注意：只能是CN、EN，" +
	 "\n不可以设置为AUTO")
	]
	[SerializeField]
	protected Language _language=Language.EN;

	protected override void Start() {
		base.Start();
		bool isActive=_language==App.getInstance().language;
		//gameObject.SetActive(isActive);
	}

#if UNITY_EDITOR
	protected override void OnValidate() {
		base.OnValidate();
		if(_language==Language.AUTO)Debug.LogError("LanguageSwitcher中，language只能设置为EN/CN不能为AUTO");
	}
#endif
}
