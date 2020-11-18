
using UnityEngine;
/// <summary>
/// 数学类
/// </summary>
public class Mathk{
	
	/// <summary>
	/// 将任意角度转换为[-180°,180°]，并返回转换后的角度
	/// </summary>
	/// <param name="rotation">需要转换的角度</param>
	/// <returns></returns>
	public static float GetRotationTo180(float rotation){
		rotation%=360.0f;
		if     (rotation>180.0f)rotation-=360.0f;
		else if(rotation<-180.0f)rotation+=360.0f;
		return rotation;
	}
		
	/// <summary>
	/// 计算出目标角减当前角的差（取到达目标角度的最近旋转方向）,并返回这个差
	/// </summary>
	/// <param name="rotation">当前角度</param>
	/// <param name="targetRotation">目标角度</param>
	/// <returns></returns>
	public static float GetRotationDifference(float rotation,float targetRotation){
		rotation=GetRotationTo360(rotation);
		targetRotation=GetRotationTo360(targetRotation);
		float offset=targetRotation-rotation;
		if(Mathf.Abs(offset)>180.0f){
			float reDir=offset>=0?-1:1;
			offset=reDir*(360.0f-Mathf.Abs(offset));
		}
		return offset;
	}
		
	/// <summary>
	/// 将任意角度转换为[0°,360°]的值,并返回转换后的值
	/// </summary>
	/// <param name="rotation">需要转换的角度</param>
	/// <returns></returns>
	public static float GetRotationTo360(float rotation){
		rotation=GetRotationTo180(rotation);
		if(rotation<0) rotation+=360;
		return rotation;
	}

	/// <summary> 获取点在线段的垂足（垂足会超出线段） </summary>
	public static Vector3 GetPerpendicularPoint(Vector3 point,Vector3 lineStart,Vector3 lineEnd){
		Vector3 rhs=point-lineStart;
		Vector3 vector=lineEnd-lineStart;
		float magnitude=vector.magnitude;
		Vector3 vector2=vector;
		if(magnitude>1E-06f){
			vector2/=magnitude;
		}
		float value=Vector3.Dot(vector2,rhs);
		return lineStart+vector2*value;
	}

	/// <summary> 点到线段的最小距离点（垂足不超出线段） </summary>
	public static Vector3 ProjectPointLine(Vector3 point,Vector3 lineStart,Vector3 lineEnd){
		Vector3 rhs=point-lineStart;
		Vector3 vector=lineEnd-lineStart;
		float magnitude=vector.magnitude;
		Vector3 vector2=vector;
		if(magnitude>1E-06f){
			vector2/=magnitude;
		}
		float value=Vector3.Dot(vector2,rhs);
		value=Mathf.Clamp(value,0f,magnitude);
		return lineStart+vector2*value;
	}

	/// <summary> 点到线段的最小距离（垂足不超出线段） </summary>
	public static float DistancePointLine(Vector3 point,Vector3 lineStart,Vector3 lineEnd){
		return Vector3.Magnitude(ProjectPointLine(point,lineStart,lineEnd)-point);
	}
	
	/// <summary>
	/// 获取点距离顶点列表的最近的线段的索引（顺时针方向查找），当顶点列表长度小于2时返回(-1,-1)
	/// </summary>
	/// <param name="point">点</param>
	/// <param name="vertices">顶点列表（顺时针方向）</param>
	/// <param name="isClosed">顶点列表是否闭合</param>
	/// <returns></returns>
	public static (int startIndex, int endIndex) GetClosestPolyLineToPoint(Vector3 point,Vector3[] vertices,bool isClosed){
		var result=(-1,-1);
		float minDistance=float.MaxValue;
		for(int i=0,len=vertices.Length;i<len;i++){
			int lineStartIndex=i;
			int lineEndIndex=(i>=len-1&&isClosed)?0:i+1;
			if(lineEndIndex>=len)break;
			Vector3 lineStart=vertices[lineStartIndex];
			Vector3 lineEnd=vertices[lineEndIndex];

			float distance=DistancePointLine(point,lineStart,lineEnd);
			if(distance<minDistance){
				minDistance=distance;
				result=(lineStartIndex,lineEndIndex);
			}
		}
		return result;
	}
}
