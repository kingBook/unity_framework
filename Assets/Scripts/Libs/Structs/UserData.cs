using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserData {

    public enum Type {
        String,
        GameObject
    }


    public Type type;
    public string stringValue;
    public GameObject gameObjectValue;

    
    public UserData(string value) {
        type = Type.String;
        stringValue = value;
        gameObjectValue = null;
    }

    public UserData(GameObject value) {
        type = Type.GameObject;
        stringValue = null;
        gameObjectValue = value;
    }
}
