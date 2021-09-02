using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UITitle : MonoBehaviour {

    ///<summary>清除本地存储的数据</summary>
    public void ClearLocalData () {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

}
