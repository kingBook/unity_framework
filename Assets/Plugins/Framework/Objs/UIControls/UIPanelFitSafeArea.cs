using UnityEngine;
/// <summary>
/// 此脚本为了UI元素不被刘海遮挡而设计。
/// 使用此类需要遵循以下布局：
/// 1.Canvas下创建一个Panel（此Panel的Image将全屏显示，一般作为半透明的全屏底图）。
/// 2.Panel下再建一个子Panel，在子Panel添加此脚本组件，子Panel的Image组件一般不用则移除
/// （子Panel的Image及它的所有子元素都将显示在屏幕安全区内，所以需要显示在屏幕安全区内的所有UI元素都要放在子Panel内）。
/// </summary>
public class UIPanelFitSafeArea:BaseMonoBehaviour{
	
	[Tooltip("如果true，将截取屏幕的宽度/高度的95%进行刘海屏模拟测试")]
	[SerializeField,SetProperty(nameof(isTest))]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
	private bool m_isTest;

	private Rect m_safeArea;
    private Rect m_lastSafeArea;
    private RectTransform m_panel;

	protected override void Awake(){
		base.Awake();
		m_panel=GetComponent<RectTransform>();
		SetSafeArea();
    }

	private void SetSafeArea(){
#if UNITY_EDITOR
		if(m_isTest){
			float width=Screen.width;
			float height=Screen.height;
			bool isPortrait=Screen.width<Screen.height;
			if(isPortrait)height*=0.95f;
			else width*=0.95f;
			m_safeArea=new Rect(0.0f,0.0f,width,height);
		}else{
			m_safeArea=Screen.safeArea;
		}
#else
		m_safeArea=Screen.safeArea;
#endif
		Refresh(m_safeArea);
	}

	protected override void Start(){
		base.Start();
		Refresh(m_safeArea);
    }

	protected override void Update2() {
		base.Update2();
		Refresh(m_safeArea);
    }

    private void Refresh(Rect r){
        if(m_lastSafeArea==r)return;
        m_lastSafeArea=r;
        //
        //Debug.LogFormat("safeArea.position:{0}, safeArea.size:{1}",r.position,r.size);
        //Debug.LogFormat("anchorMin:{0},anchorMax:{1}",m_panel.anchorMin,m_panel.anchorMax);
        Vector2 anchorMin=r.position;
        Vector2 anchorMax=r.position+r.size;
        //anchorMin(左上角)、anchorMax(右下角)表示在屏幕上的百分比位置,在屏幕内的取值范围是[0,1]
        anchorMin.x/=Screen.width;
        anchorMin.y/=Screen.height;
        anchorMax.x/=Screen.width;
        anchorMax.y/=Screen.height;
        m_panel.anchorMin=anchorMin;
        m_panel.anchorMax=anchorMax;
		//Debug.LogFormat("anchorMin:{0},anchorMax:{1}",m_panel.anchorMin,m_panel.anchorMax);
        //Debug.Log("=====================================================================");
    }

	public bool isTest{
		get => m_isTest;
		set{
			m_isTest=value;
			if(Application.isPlaying){
				SetSafeArea();
			}
		}
	}
}