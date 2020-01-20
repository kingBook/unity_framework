using UnityEngine;
/// <summary>
/// 屏幕上打印Log信息
/// </summary>
public class ScreenLog:BaseMonoBehaviour{
	
	public bool isStackTrace=false;

	private string m_output="";
	private Vector2 m_scrollPos;
	private bool m_isPause;
	
	protected override void OnEnable(){
		base.OnEnable();
		Application.logMessageReceivedThreaded+=LogHandler;
	}
	
	protected override void OnGUI2(){
		base.OnGUI2();
		float width=Screen.width*0.3f;
		float height=Screen.height;
		GUILayout.BeginVertical();
			//滚动的文本
			m_scrollPos=GUILayout.BeginScrollView(m_scrollPos,GUILayout.Height(height-30f));
			GUILayout.TextArea(m_output,GUILayout.MaxWidth(width),GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();
			
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				//暂停/恢复按钮
				string pauseResumeText=m_isPause?"Resume":"Pause";
				if(GUILayout.Button(pauseResumeText)){
					m_isPause=!m_isPause;
				}
				//清除按钮
				if(GUILayout.Button("Clear")){
					m_output="";
				}
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	private void LogHandler(string logString, string stackTrace, LogType type){
		if(m_isPause)return;
		m_output+=logString+'\n';
		if(isStackTrace){
			m_output+=stackTrace+'\n';
		}
	}
	
	protected override void OnDisable(){
		Application.logMessageReceivedThreaded-=LogHandler;
		base.OnDisable();
	}
	
}