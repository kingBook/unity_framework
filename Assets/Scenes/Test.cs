using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour{
    public static Test _instance;

    void Start(){
		Debug.LogFormat("_instance:{0},_instance==this:{1}",_instance,_instance==this);
        _instance=this;
    }

    void Update(){
        
    }
}
