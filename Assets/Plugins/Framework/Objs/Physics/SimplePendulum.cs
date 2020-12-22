﻿using UnityEngine;

/**单摆*/
public class SimplePendulum:MonoBehaviour{
	
	public float gravity=-9.81f;
	public Vector2 originPosition;
	public Transform targetTransform;

	private float m_deltaTime;
	private float m_currentAngle;
	private float m_len;//摆长
	private bool m_isPause;
	private bool m_isInited;
	private Vector2 m_velocity;

	/**返回单摆当前欧拉角度*/
	public float eulerAngle=>LocalAngleToWorld(m_currentAngle)*Mathf.Rad2Deg;
	public Vector2 velocity=>m_velocity;
	public float w { get; private set; }

	private void Init(){
		m_deltaTime=Time.fixedDeltaTime;
		
		Vector2 relative=(Vector2)targetTransform.position-originPosition;
		
		m_len=relative.magnitude;
		
		float angle=Mathf.Atan2(relative.y,relative.x);
		m_currentAngle=WorldAngleToLocal(angle);
		
		w=0f;
	}

	private void FixedUpdate(){
		if(m_isPause)return;

		if(!m_isInited){
			m_isInited=true;
			Init();
		}

		float k1,k2,k3,k4;
		float l1,l2,l3,l4;
		{
			k1=w;
			l1=(gravity/m_len)*Mathf.Sin(m_currentAngle);
			
			k2=w+m_deltaTime*l1/2f;
			l2=(gravity/m_len)*Mathf.Sin(m_currentAngle+m_deltaTime*k1/2f);
			
			k3=w+m_deltaTime*l2/2f;
			l3=(gravity/m_len)*Mathf.Sin(m_currentAngle+m_deltaTime*k2/2f);
			
			k4=w+m_deltaTime*l3;
			l4=(gravity/m_len)*Mathf.Sin(m_currentAngle*m_deltaTime*k3);
			
			m_currentAngle+=m_deltaTime*(k1+2f*k2+2f*k3+k4)/(6f/*2f*Math.PI*/);
			w+=m_deltaTime*(l1+2f*l2+2f*l3+l4)/(6f/*2f*Math.PI*/);
		}

		float newX=originPosition.x+Mathf.Sin(m_currentAngle)*m_len;
		float newY=originPosition.y-Mathf.Cos(m_currentAngle)*m_len;

		Vector3 targetPos=targetTransform.position;

		m_velocity.x=newX-targetPos.x;
		m_velocity.y=newY-targetPos.y;
		
		targetPos.x=newX;
		targetPos.y=newY;
		targetTransform.position=targetPos;
	}

	private float WorldAngleToLocal(float worldAngle){
		return worldAngle+Mathf.PI*0.5f;
	}

	private float LocalAngleToWorld(float localAngle){
		return localAngle-Mathf.PI*0.5f;
	}

	public void SetPause(bool value){
		m_isPause=value;
	}

	private void OnDisable(){
		m_isInited=false;
	}
}