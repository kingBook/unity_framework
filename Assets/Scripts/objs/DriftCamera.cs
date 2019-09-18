using System;
using UnityEngine;

public class DriftCamera:BaseMonoBehaviour{
    [Serializable]
    public class AdvancedOptions{
        public bool updateCameraInUpdate;
        public bool updateCameraInFixedUpdate = true;
        public bool updateCameraInLateUpdate;
        public KeyCode switchViewKey = KeyCode.X;
    }
    public bool m_ShowingSideView;

    public float smoothing = 6f;
    public Transform lookAtTarget;
    public Transform positionTarget;
    public Transform sideView;
    public AdvancedOptions advancedOptions;

	protected override void FixedUpdate2() {
		base.FixedUpdate2();
		if(advancedOptions.updateCameraInFixedUpdate)
            updateCamera ();
	}

	protected override void Update2() {
		base.Update2();
		if (Input.GetKeyDown (advancedOptions.switchViewKey))
            m_ShowingSideView = !m_ShowingSideView;

        if(advancedOptions.updateCameraInUpdate)
            updateCamera ();
	}

	protected override void LateUpdate2() {
		base.LateUpdate2();
		 if(advancedOptions.updateCameraInLateUpdate)
            updateCamera ();
	}

    private void updateCamera (){
		if(lookAtTarget==null)return;
        if (m_ShowingSideView){
			if(sideView){
				transform.rotation = sideView.rotation;
				transform.position = Vector3.Lerp(transform.position, sideView.position, Time.deltaTime * smoothing);
				transform.LookAt(lookAtTarget);
			}
        }else{
			if(positionTarget){
				transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
				transform.LookAt(lookAtTarget);
			}
        }
    }
}
