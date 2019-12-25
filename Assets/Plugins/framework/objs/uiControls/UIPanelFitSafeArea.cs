using UnityEngine;
/// <summary>
/// Canvas下的Panel适应刘海屏
/// </summary>
public class UIPanelFitSafeArea:BaseMonoBehaviour{
	[Tooltip("如果true，将取屏幕的宽度的0.9进行测试")]
	[SerializeField,SetProperty("isTest")]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
	private bool m_isTest;

	private Rect m_safeArea;
    private Rect m_lastSafeArea;
    private RectTransform m_panel;

	protected override void Awake(){
		base.Awake();
		m_panel=GetComponent<RectTransform>();
		setSafeArea();
    }

	private void setSafeArea(){
		if(m_isTest){
			m_safeArea=new Rect(0.0f,0.0f,Screen.width*0.9f,Screen.height);//测试：取屏幕宽度的0.9
		}else{
			m_safeArea=Screen.safeArea;
		}
		refresh(m_safeArea);
	}

	protected override void Start(){
		base.Start();
		refresh(m_safeArea);
    }

	protected override void Update2() {
		base.Update2();
		refresh(m_safeArea);
    }

    private void refresh(Rect r){
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
				setSafeArea();
			}
		}
	}
}