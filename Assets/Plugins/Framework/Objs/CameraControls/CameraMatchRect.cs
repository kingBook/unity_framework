#pragma warning disable 0649
using UnityEngine;
using System.Collections;
/// <summary>
/// 设置相机的在不同的屏幕分辨率下自动设置大小匹配一个矩形
/// </summary>
public class CameraMatchRect : MonoBehaviour {

    public enum FitMode { Auto, Width, Height }

    public Transform min;
    public Transform max;
    public Camera viewCamera;
    public bool isUpdate;
    public FitMode fitMode = FitMode.Auto;

#if UNITY_EDITOR
    private void Reset () {
        if (!viewCamera) {
            viewCamera = GetComponent<Camera>();
        }
    }
#endif

    private void Start () {
        Fit();
    }

    private void LateUpdate () {
        if (isUpdate) Fit();
    }

    private void Fit () {
        Vector3 rectCenter = (min.position + max.position) * 0.5f;
        Vector3 rectExtents = (max.position - min.position) * 0.5f;
        float referenceScaleFactor = rectExtents.x / rectExtents.y;
        float scaleFactor = (float)Screen.width / Screen.height;

        FitMode tempFitMode = fitMode;
        if (tempFitMode == FitMode.Auto) {
            if (scaleFactor > referenceScaleFactor) {
                tempFitMode = FitMode.Height;
            } else if (scaleFactor < referenceScaleFactor) {
                tempFitMode = FitMode.Width;
            }
        }

        if (viewCamera.orthographic) {
            Vector3 viewCameraPos = viewCamera.transform.position;
            viewCameraPos.x = rectCenter.x;
            viewCameraPos.y = rectCenter.y;
            viewCamera.transform.position = viewCameraPos;
            if (tempFitMode == FitMode.Height) {
                //匹配高度
                viewCamera.orthographicSize = rectExtents.y;
            } else if (tempFitMode == FitMode.Width) {
                //匹配宽度
                viewCamera.orthographicSize = rectExtents.x / scaleFactor;
            }
        } else {
            float distance = Vector3.Distance(rectCenter, viewCamera.transform.position);
            viewCamera.transform.LookAt(rectCenter);
            if (tempFitMode == FitMode.Height) {
                //匹配高度
                viewCamera.fieldOfView = Mathf.Atan(rectExtents.y / distance) * Mathf.Rad2Deg * 2f;
            } else if (tempFitMode == FitMode.Width) {
                //匹配宽度
                float tempExtentsY = rectExtents.x / viewCamera.aspect;
                viewCamera.fieldOfView = Mathf.Atan(tempExtentsY / distance) * Mathf.Rad2Deg * 2f;
            }
        }
    }

}