using UnityEngine;
/// <summary>
/// 控制相机跟随目标
/// </summary>
public class DriftCamera:BaseMonoBehaviour{
	
	[System.Serializable]
	public class AdvancedOptions{
		public bool updateCameraInFixedUpdate=true;
		public bool updateCameraInUpdate;
		public bool updateCameraInLateUpdate;
		[Tooltip("Start()时，立即设置相机位置并旋转朝向目标")]
		public bool isLookToTargetOnStart;
		[Tooltip("当目标发生旋转时，相机也绕着目标旋转")]
		public bool isLookTargetRotation;
        [Tooltip("是否检测穿过遮挡物并处理")]
		public bool isCheckCrossObs=true;
        [Tooltip("遮挡物LayerMask")]
		public LayerMask obsLayerMask=-1;
	}
	
	[System.Serializable]
	public enum PositionMode{
		Top,//0
		TopLeftForward,TopForward,TopRightForward,TopRight,TopRightBack,TopBack,TopLeftBack,TopLeft,//8
		LeftForward,Forward,RightForward,Right,RightBack,Back,LeftBack,Left,//16
		Bottom,//17
		BottomLeftForward,BottomForward,BottomRightForward,BottomRight,BottomRightBack,BottomBack,BottomLeftBack,BottomLeft//25
	}
	public static readonly Vector3[] positionModeVerties=new Vector3[]{
		new Vector3(0,1,0),//0
		new Vector3(-1,1,1),new Vector3(0,1,1),new Vector3(1,1,1),new Vector3(1,1,0),new Vector3(1,1,-1),new Vector3(0,1,-1),new Vector3(-1,1,-1),new Vector3(-1,1,0),//8
		new Vector3(-1,0,1),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0),new Vector3(1,0,-1),new Vector3(0,0,-1),new Vector3(-1,0,-1),new Vector3(-1,0,0),//16
		new Vector3(0,-1,0),//17
		new Vector3(-1,-1,1),new Vector3(0,-1,1),new Vector3(1,-1,1),new Vector3(1,-1,0),new Vector3(1,-1,-1),new Vector3(0,-1,-1),new Vector3(-1,-1,-1),new Vector3(-1,-1,0)//25
	};

	public float smoothing=6.0f;
	[Tooltip("相机朝向的目标点")]
	public Transform targetTransform;
	[Tooltip("相机相对于目标点的单位化位置")]
	public Vector3 originPositionNormalized=new Vector3(0.2f,0.68f,-1.0f);
	[Tooltip("相机与目标点的距离")]
	public float distance=4.0f;
	public AdvancedOptions advancedOptions;
	
	protected override void Start(){
		base.Start();
		if(advancedOptions.isLookToTargetOnStart){
			LookToTarget();
		}
	}

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		if(advancedOptions.updateCameraInFixedUpdate){
			UpdateCamera();
		}
	}

	protected override void Update2(){
		base.Update2();
		if(advancedOptions.updateCameraInUpdate){
			UpdateCamera();
		}
	}

	protected override void LateUpdate2(){
		base.LateUpdate2();
		if(advancedOptions.updateCameraInLateUpdate){
			UpdateCamera();
		}
	}
	
	/// <summary>
	/// 立即移动相机并且并旋转朝向目标
	/// </summary>
	private void LookToTarget(){
		if(targetTransform==null)return;
		Vector3 positionTarget=GetPositionTarget();
        //移动相机
		transform.position=positionTarget;
        //旋转相机朝向
		transform.LookAt(targetTransform);
	}
	
	private void UpdateCamera(){
		if(targetTransform==null)return;
		Vector3 positionTarget=GetPositionTarget();
        //遮挡检测
		if(advancedOptions.isCheckCrossObs){
			CheckCrossObsViewField(ref positionTarget);
		}
        //移动相机
		transform.position=Vector3.Lerp(transform.position,positionTarget,Time.deltaTime*smoothing);
        //旋转相机朝向
		transform.LookAt(targetTransform);
	}
	
	private Vector3 GetPositionTarget(){
		Vector3 offset=originPositionNormalized*distance;
		if(advancedOptions.isLookTargetRotation){
			offset=targetTransform.rotation*offset;
		}
		Vector3 positionTarget=targetTransform.position+offset;
		return positionTarget;
	}
    
    /// <summary>
    /// 检测遮挡并处理
    /// </summary>
	private void CheckCrossObsViewField(ref Vector3 positionTarget){
		if(!IsCrossObs(positionTarget))return;
		for(int i=0;i<17;i++){
            //取一个相机测试点检测是否遮挡
			Vector3 normalized=positionModeVerties[i];
			Vector3 offset=normalized*distance;
			offset=targetTransform.rotation*offset;
			Vector3 testPosTarget=targetTransform.position+offset;
            //球形插值运算取测试点检测是否遮挡
			float t=0.0f;
			for(int j=0;j<5;j++){
				t+=0.2f;
				Vector3 checkPos=Vector3.Slerp(positionTarget,testPosTarget,t);
				if(!IsCrossObs(checkPos)){
                    //没有被遮挡，返回该测试点
					positionTarget=checkPos;
					return;
				}
			}
			
		}
	}
	
    /// <summary>
    /// 是否被遮挡
    /// </summary>
	private bool IsCrossObs(Vector3 positionTarget){
		Ray ray=new Ray(positionTarget,targetTransform.position-positionTarget);
		float maxDistance=Vector3.Distance(targetTransform.position,positionTarget);
		
		const int bufferLen=50;
		RaycastHit[] buffer=new RaycastHit[bufferLen];
		Physics.RaycastNonAlloc(ray,buffer,maxDistance,advancedOptions.obsLayerMask);
		
		for(int i=0;i<bufferLen;i++){
			RaycastHit raycastHit=buffer[i];
			if(raycastHit.collider!=null){
				return true;
			}
		}
		return false;
	}
	
}