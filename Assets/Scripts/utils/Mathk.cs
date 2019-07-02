
using UnityEngine;

public class Mathk{
	/**将任意角度转换为[-180°,180°]，并返回转换后的角度*/
	public static float getRotationTo180(float rotation){
		rotation%=360.0f;
		if     (rotation>180.0f)rotation-=360.0f;
		else if(rotation<-180.0f)rotation+=360.0f;
		return rotation;
	}
		
	/**计算出任意两个角度的差(取到达目标角度的最近旋转方向),并返回这个差*/
	public static float getFlashRotationOffset(float rotation,float targetRotation){
		rotation=getRotationTo360(rotation);
		targetRotation=getRotationTo360(targetRotation);
		float offset=targetRotation-rotation;
		if(Mathf.Abs(offset)>180.0f){
			float reDir=offset>=0?-1:1;
			offset=reDir*(360.0f-Mathf.Abs(offset));
		}
		return offset;
	}
		
	/**将任意角度转换为[0°,360°]的值,并返回转换后的值*/
	public static float getRotationTo360(float rotation){
		rotation=getRotationTo180(rotation);
		if(rotation<0) rotation+=360;
		return rotation;
	}
}
