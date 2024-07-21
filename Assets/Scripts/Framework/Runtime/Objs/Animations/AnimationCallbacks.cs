using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class AnimationCallbacks : MonoBehaviour {

    [System.Serializable]
    public struct Callback {
        public float time;
        public UnityEvent func;
    }

    [SerializeField] private List<Callback> m_callbacks;

}
