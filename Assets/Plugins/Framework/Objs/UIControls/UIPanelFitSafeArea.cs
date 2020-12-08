#pragma warning disable 0649
using UnityEngine;
/// <summary>
/// 此脚本为了UI元素不被刘海遮挡而设计。
/// 使用此类需要遵循以下布局：
/// 1.Canvas下创建一个Panel（此Panel的Image将全屏显示，一般作为半透明的全屏底图）。
/// 2.Panel下再建一个子Panel，在子Panel添加此脚本组件，子Panel的Image组件一般不用则移除
/// （子Panel的Image及它的所有子元素都将显示在屏幕安全区内，所以需要显示在屏幕安全区内的所有UI元素都要放在子Panel内）。
/// </summary>
[DisallowMultipleComponent]
public class UIPanelFitSafeArea:BaseMonoBehaviour{

	[SerializeField,Tooltip("如果true，将截取屏幕的宽度/高度的95%进行刘海屏模拟测试")]
	private bool m_isTest;

	private RectTransform m_panel;
	private float m_time;

	protected override void Awake(){
		base.Awake();
		m_panel=GetComponent<RectTransform>();
		m_time=Time.time;
		MatchSafeArea();
	}
	
	protected override void Start(){
		base.Start();
		MatchSafeArea();
	}

	protected override void Update2() {
		base.Update2();
		if(Time.time-m_time>0.3f){//限制刷新频率
			m_time=Time.time;
			MatchSafeArea();
		}
	}

	private void MatchSafeArea(){
		Rect safeArea=Screen.safeArea;
		float screenWidth=Screen.width;
		float screenHeight=Screen.height;
		//在 Unity 编辑器时，如果 m_isTest 为 true 时，截取屏幕进行测试
#if UNITY_EDITOR
		if(m_isTest){
			bool isPortraitGameView=screenWidth<screenHeight;
			if(isPortraitGameView)safeArea.height*=0.95f;
			else safeArea.width*=0.95f;
		}
#elif UNITY_IOS||UNITY_ANDROID
		//根据屏幕旋转重新设置正确的 screenWidth 和 screenHeight，长的为 screenWidth ，短的为 screenHeight。
		bool isPortrait=Screen.orientation==ScreenOrientation.Portrait||Screen.orientation==ScreenOrientation.PortraitUpsideDown;
		float minScreenValue=Mathf.Min(screenWidth,screenHeight);
		float maxScreenValue=Mathf.Max(screenWidth,screenHeight);
		screenWidth=isPortrait?minScreenValue:maxScreenValue;
		screenHeight=isPortrait?maxScreenValue:minScreenValue;
#endif
		//计算 anchorMin、anchorMax
		Vector2 anchorMin=safeArea.position;
		Vector2 anchorMax=safeArea.position+safeArea.size;
		anchorMin.x/=screenWidth;
		anchorMin.y/=screenHeight;
		anchorMax.x/=screenWidth;
		anchorMax.y/=screenHeight;
		m_panel.anchorMin=anchorMin;
		m_panel.anchorMax=anchorMax;
	}

	public bool isTest{
		get => m_isTest;
	}
}