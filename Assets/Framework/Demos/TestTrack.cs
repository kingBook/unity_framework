#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestTrack : MonoBehaviour {
    
    private void Start () {
        var editors = UnityEditor.ActiveEditorTracker.sharedTracker.activeEditors;
        // ActiveEditorTracker.sharedTracker.SetVisible(i, 0); //0是折叠, 1是展开
        Debug.Log($"editors.Length:{editors.Length}");
        for (int i = 0, len = editors.Length; i < len; i++) {
            Debug.Log($"i:{i} {editors[i]}, instanceID:{editors[i].GetInstanceID()}");
           // UnityEditor.ActiveEditorTracker.sharedTracker.SetVisible(i, 1); //0是折叠, 1是展开
        }
    }

    
    private void Update () {
        
    }
}
