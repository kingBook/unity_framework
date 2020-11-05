using UnityEngine;
using System.Collections;
/// <summary>
/// 将一个游戏对象的 Rotation 设置与相机一样
/// </summary>
public class RotationAlignCamera:BaseMonoBehaviour{

	public bool isUpdate;
	private Transform m_cameraTransform;

	protected override void Start() {
		base.Start();
		m_cameraTransform=Camera.main.transform;
		Align();
	}

	protected override void Update2() {
		base.Update2();
		if(isUpdate){
			Align();
		}
	}

	private void Align(){
		transform.rotation=m_cameraTransform.rotation;
	}

}
