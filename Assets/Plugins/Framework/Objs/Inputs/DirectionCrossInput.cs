using UnityEngine;
using System.Collections;

#pragma warning disable 0649

/// <summary>
/// 方向跨平台输入(正式发布时，DirectionDragHandle所在GameObject必须吊销)
/// </summary>
public class DirectionCrossInput:BaseMonoBehaviour{
	[SerializeField]private DirectionDragHandle m_directionDragHandle;

	private bool m_isMoblieInput;
	private Vector2 m_directionSize;

	public Vector2 directionSize=>m_directionSize;
	public Vector2 directionNormalized=>m_directionSize.normalized;

	protected override void Awake() {
		base.Awake();
		if(Input.touchSupported){
			m_directionDragHandle.gameObject.SetActive(true);
		}
		if(m_directionDragHandle.gameObject.activeSelf){
			m_isMoblieInput=true;
		}
	}

	protected override void OnEnable() {
		base.OnEnable();
		m_directionSize.Set(0,0);
		if(m_isMoblieInput){
			m_directionDragHandle.gameObject.SetActive(true);
		}
	}

	protected override void Update2() {
		base.Update2();
		if(enabled){
			if(m_isMoblieInput){
				m_directionSize=m_directionDragHandle.directionSize;
			}else{
				float x=Input.GetAxis("Horizontal");
				float y=Input.GetAxis("Vertical");
				m_directionSize.Set(x,y);
			}
		}
	}

	protected override void OnDisable() {
		base.OnDisable();
		if(m_isMoblieInput){
			if(m_directionDragHandle!=null&&m_directionDragHandle.gameObject!=null){
				m_directionDragHandle.gameObject.SetActive(false);
			}
		}
	}

}
