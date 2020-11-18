using UnityEngine;

/// <summary>
/// 控制相机跟随目标
/// </summary>
public class DriftCamera:BaseMonoBehaviour{
	
	[System.Serializable]
	public class RangeFloat{
		public Vector3 min=new Vector3(float.MinValue,float.MinValue,float.MinValue);
		public Vector3 max=new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
		public RangeFloat(Vector3 minValue,Vector3 maxValue){
			min=minValue;
			max=maxValue;
		}
	}
	
	[System.Serializable]
	public class AdvancedOptions{
		public bool updateCameraInFixedUpdate;							//是否FixedUpdate函数中更新相机
		public bool updateCameraInUpdate;								//是否在Update函数中更新相机
		public bool updateCameraInLateUpdate=true;  					//是否在LateUpdate函数中更新相机
		public bool isLookToTargetOnStart;								//是否在Start函数中，立即设置相机位置并旋转朝向目标
		public bool isLookTargetRotation;								//是否锁定旋转到目标（当目标发生旋转时，相机也绕着目标旋转）
		public RangeFloat positionRange=new RangeFloat(					//相机移动的位置范围
			new Vector3(float.MinValue,float.MinValue,float.MinValue),
			new Vector3(float.MaxValue,float.MaxValue,float.MaxValue));
		[Space]
		public bool isLockEulerAngleX;
		public float lockEulerValueX;
		public bool isLockEulerAngleY;
		public float lockEulerValueY;
		public bool isLockEulerAngleZ;
		public float lockEulerValueZ;
		[Space]
		public bool isCheckCrossObs=true;								//是否检测穿过遮挡物并处理														
		public LayerMask obsLayerMask=-1;								//遮挡物LayerMask
	}
	
	[System.Serializable]
	public enum PositionMode{
		Top,//0
		TopLeftForward,TopForward,TopRightForward,TopRight,TopRightBack,TopBack,TopLeftBack,TopLeft,//8
		LeftForward,Forward,RightForward,Right,RightBack,Back,LeftBack,Left,//16
		Bottom,//17
		BottomLeftForward,BottomForward,BottomRightForward,BottomRight,BottomRightBack,BottomBack,BottomLeftBack,BottomLeft//25
	}
	private static readonly Vector3[] s_positionModeVerties=new Vector3[]{
		new Vector3(0,1,0),//0
		new Vector3(-1,1,1),new Vector3(0,1,1),new Vector3(1,1,1),new Vector3(1,1,0),new Vector3(1,1,-1),new Vector3(0,1,-1),new Vector3(-1,1,-1),new Vector3(-1,1,0),//8
		new Vector3(-1,0,1),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0),new Vector3(1,0,-1),new Vector3(0,0,-1),new Vector3(-1,0,-1),new Vector3(-1,0,0),//16
		new Vector3(0,-1,0),//17
		new Vector3(-1,-1,1),new Vector3(0,-1,1),new Vector3(1,-1,1),new Vector3(1,-1,0),new Vector3(1,-1,-1),new Vector3(0,-1,-1),new Vector3(-1,-1,-1),new Vector3(-1,-1,0)//25
	};

	public float smoothing=6.0f;											//更新相机时每秒移动的距离
	public Transform targetTransform;										//相机朝向的目标点
	public Vector3 originPositionNormalized=new Vector3(0.2f,0.68f,-1.0f);	//相机相对于目标点的单位化位置
	public float distance=4.0f;												//相机与目标点的距离
	public AdvancedOptions advancedOptions;
	
	protected override void Start(){
		base.Start();
		if(advancedOptions.isLookToTargetOnStart){
			//立即移动相机并且并旋转朝向目标
			UpdateCamera(false,false);
		}
	}

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		if(advancedOptions.updateCameraInFixedUpdate){
			UpdateCamera(advancedOptions.isCheckCrossObs,true);
		}
	}

	protected override void Update2(){
		base.Update2();
		if(advancedOptions.updateCameraInUpdate){
			UpdateCamera(advancedOptions.isCheckCrossObs,true);
		}
	}

	protected override void LateUpdate2(){
		base.LateUpdate2();
		if(advancedOptions.updateCameraInLateUpdate){
			UpdateCamera(advancedOptions.isCheckCrossObs,true);
		}
	}
	
	private void UpdateCamera(bool isCheckCrossObs,bool isLearp){
		if(targetTransform==null)return;
		Vector3 positionTarget=GetPositionTarget();
        //遮挡检测
		if(isCheckCrossObs){
			CheckCrossObsViewField(ref positionTarget);
		}
        //移动相机
		if(isLearp){
			positionTarget=Vector3.Lerp(transform.position,positionTarget,Time.deltaTime*smoothing);
		}
		//根据要求限制位置范围
		Vector3 tempPosition=transform.position;
		tempPosition.x=Mathf.Clamp(positionTarget.x,advancedOptions.positionRange.min.x,advancedOptions.positionRange.max.x);
		tempPosition.y=Mathf.Clamp(positionTarget.y,advancedOptions.positionRange.min.y,advancedOptions.positionRange.max.y);
		tempPosition.z=Mathf.Clamp(positionTarget.z,advancedOptions.positionRange.min.z,advancedOptions.positionRange.max.z);
		transform.position=tempPosition;
        //旋转相机朝向
		transform.LookAt(targetTransform);
		//根据要求锁定旋转
		var eulerAngles=transform.eulerAngles;
		if(advancedOptions.isLockEulerAngleX)eulerAngles.x=advancedOptions.lockEulerValueX;
		if(advancedOptions.isLockEulerAngleY)eulerAngles.y=advancedOptions.lockEulerValueY;
		if(advancedOptions.isLockEulerAngleZ)eulerAngles.z=advancedOptions.lockEulerValueZ;
		transform.eulerAngles=eulerAngles;
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
			Vector3 normalized=s_positionModeVerties[i];
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
	    Vector3 position=targetTransform.position;
	    Ray ray=new Ray(positionTarget,position-positionTarget);
		float maxDistance=Vector3.Distance(position,positionTarget);
		
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