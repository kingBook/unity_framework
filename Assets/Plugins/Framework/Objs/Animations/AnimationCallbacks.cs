using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class AnimationCallbacks:BaseMonoBehaviour{
	
	[System.Serializable]
	public struct Callback{
		public float time;
		public UnityEvent func;
	}

	[SerializeField] private List<Callback> m_callbacks;

}
