using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasLevel : MonoBehaviour {

    public DirectionInput directionInput;
    public KeyInput keyInput;
    [Space]
    [SerializeField] private AudioClip m_clickAudioClip;

    public void OnClick(Vector2 screenPoint) {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject) {
            bool isButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null;
            if (isButton && m_clickAudioClip) {
                App.instance.audioManager.PlayEffect(m_clickAudioClip, Camera.main.transform.position);
            }
        }
    }


    private void Update() {
        if (InputUtil.GetTouchBeganScreenPoint(false, out Vector3 screenPoint, out int fingerId)) {
            OnClick(screenPoint);
        }
    }
}
