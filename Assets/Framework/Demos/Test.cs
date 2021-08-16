using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    public Text textTop;
    public Text text;
    public Text textBottom;


    // Update is called once per frame
    void Update () {
        /*bool hasScreenPoint=InputUtil.GetTouchBeginScreenPoint(true,out Vector3 screenPoint,out int fingerId);
		
        text.text=$"IsPointerOverUI:{InputUtil.IsPointerOverUI(0)}, hasScreenPoint:{hasScreenPoint},fingerId:{fingerId},screenPoint:{screenPoint}";
		if(hasScreenPoint){
			textTop.text=$"fingerId:{fingerId},screenPoint:{screenPoint}";
		}*/
         

        textBottom.text = InputUtil.GetSlideScreenDeltaPosition().ToString();
        bool hasScreenPoint = InputUtil.GetPressScreenPoint(true, out Vector3 screenPoint, out int fingerId);
        text.text = $"IsPointerOverUI:{InputUtil.IsPointerOverUI(0)}, hasScreenPoint:{hasScreenPoint},fingerId:{fingerId},screenPoint:{screenPoint}";
        if (hasScreenPoint) {
            textTop.text = $"fingerId:{fingerId},screenPoint:{screenPoint}";
        }
    }
}
