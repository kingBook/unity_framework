using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 用户输入工具类
/// </summary>
public static class InputUtil {

    /// <summary>
    /// 返回指定TouchPhase的第一个触摸点，未找到时touch.fingerId等于-1（注：此方法无关任何鼠标操作）
    /// </summary>
    /// <param name="phase">触摸的阶段</param>
    /// <param name="ignorePointerOverUI">是否过滤触摸UI的触摸点</param>
    /// <returns>返回指定TouchPhase的第一个触摸点</returns>
    public static Touch GetFirstTouch(TouchPhase phase, bool ignorePointerOverUI) {
        Touch touch;
        for (int i = 0, len = Input.touchCount; i < len; i++) {
            touch = Input.GetTouch(i);
            if (touch.phase != phase) continue;
            if (ignorePointerOverUI && EventSystem.current.IsPointerOverGameObject(touch.fingerId)) continue;
            return touch;
        }
        //
        touch = new Touch { fingerId = -1 };
        return touch;
    }

    /// <summary>
    /// 返回指定手指Id的Touch，未找到时touch.fingerId等于-1（注：此方法无关任何鼠标操作）
    /// </summary>
    /// <param name="fingerId">手指 ID</param>
    /// <param name="isIgnorePointerOverUI">当手指ID指定的 Touch 位置在UI上时是否跳过</param>
    /// <param name="phases">触摸阶段</param>
    /// <returns></returns>
    public static Touch GetTouchWithFingerId(int fingerId, bool isIgnorePointerOverUI, params TouchPhase[] phases) {
        Touch touch;
        for (int i = 0, len = Input.touchCount; i < len; i++) {
            touch = Input.GetTouch(i);
            if (touch.fingerId != fingerId) continue;
            if (isIgnorePointerOverUI && EventSystem.current.IsPointerOverGameObject(fingerId)) continue;
            if (phases.Length > 0) {
                if (Array.IndexOf(phases, touch.phase) > -1) {
                    return touch;
                }
            } else {
                //不填写 phases 参数时，找到 Touch 直接返回
                return touch;
            }
        }
        //
        touch = new Touch { fingerId = -1 };
        return touch;
    }

    /// <summary>
    /// 鼠标左键按下/有触摸处于 TouchPhase.Began 阶段时返回 true,并输出鼠标/触摸点的屏幕坐标，
    /// 鼠标左键未按下/没有触摸处于 TouchPhase.Began 阶段则返回 false,并输出屏幕坐标(0,0,0)和输出手指id（-1）
    /// （注：此方法关联鼠标操作）
    /// </summary>
    /// <param name="isIgnorePointerOverUI">是否忽略UI上的点击</param>
    /// <param name="screenPoint">输出鼠标/第一个处于  TouchPhase.Began 阶段的触摸点的屏幕坐标</param>
    /// <param name="fingerId">鼠标模式输出0，触摸模式输出手指 id，鼠标左键未按下/没有触摸处于 TouchPhase.Began 阶段输出 -1</param>
    /// <returns></returns>
    public static bool GetTouchBeganScreenPoint(bool isIgnorePointerOverUI, out Vector3 screenPoint, out int fingerId) {
        fingerId = -1;
        screenPoint = new Vector3();
        if (isIgnorePointerOverUI && IsPointerOverUI(0)) {
            //忽略UI上的点击
        } else if (Application.isMobilePlatform && Input.touchSupported) {
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    fingerId = touch.fingerId;
                    screenPoint = touch.position;
                    return true;
                }
            }
        } else {
            if (Input.GetMouseButtonDown(0)) {
                fingerId = 0;
                screenPoint = Input.mousePosition;
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 鼠标左键不松开/有触摸按住不松开返回 true，并输出鼠标/第一个触摸点的屏幕坐标和手指 id，否则返回 false,并输出屏幕坐标 (0,0,0) 手指 id -1
    /// （注：此方法关联鼠标操作）
    /// </summary>
    /// <param name="isIgnorePointerOverUI">是否忽略UI上的点击</param>
    /// <param name="screenPoint">输出鼠标/第一个处于按住的触摸的屏幕坐标</param>
    /// <param name="fingerId">鼠标模式输出0，触摸模式输出手指 id，鼠标左键未按下/没有触摸处于按下状态则输出 -1</param>
    /// <returns></returns>
    public static bool GetPressScreenPoint(bool isIgnorePointerOverUI, out Vector3 screenPoint, out int fingerId) {
        fingerId = -1;
        screenPoint = new Vector3();
        if (isIgnorePointerOverUI && IsPointerOverUI(0)) {
            //忽略UI上的点击
        } else if (Application.isMobilePlatform && Input.touchSupported) {
            for (int i = 0, len = Input.touchCount; i < len; i++) {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                    fingerId = touch.fingerId;
                    screenPoint = touch.position;
                    return true;
                }
            }
        } else {
            if (Input.GetMouseButton(0)) {
                fingerId = 0;
                screenPoint = Input.mousePosition;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 检测鼠标位置/指定手指Id的触摸点是否在UI上方（不管鼠标处于按下或松开，不管触摸处于任何阶段）（注：此方法关联鼠标操作）
    /// </summary>
    /// <param name="fingerId"></param>
    /// <returns></returns>
    public static bool IsPointerOverUI(int fingerId) {
        bool result = false;
        if (EventSystem.current) {
            if (Application.isMobilePlatform && Input.touchSupported) {
                for (int i = 0, len = Input.touchCount; i < len; i++) {
                    Touch touch = Input.GetTouch(i);
                    if (touch.fingerId == fingerId) {
                        if (EventSystem.current.IsPointerOverGameObject(fingerId)) {
                            result = true;
                            break;
                        }
                    }
                }
            } else {
                if (EventSystem.current.IsPointerOverGameObject()) {
                    result = true;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 检测鼠标左键/指定fingerId的触摸是否释放（注：此方法关联鼠标操作）
    /// </summary>
    /// <param name="fingerId"></param>
    /// <returns></returns>
    public static bool IsTouchUp(int fingerId) {
        bool result = true;
        if (Application.isMobilePlatform && Input.touchSupported) {
            Touch touch = GetTouchWithFingerId(fingerId, false);
            if (touch.fingerId > -1) {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                    result = false;
                }
            }
        } else {
            if (Input.GetMouseButton(0)) {
                result = false;
            }
        }
        return result;
    }

    /// <summary>
    /// 返回滑屏增量位置（注：此方法关联鼠标操作）
    /// </summary>
    public static Vector2 GetSlideScreenDeltaPosition() {
        Vector2 deltaPosition = new Vector2();
        if (Input.touchSupported || Input.GetMouseButton(0)) {
            //支持触摸时，如果 Input.simulateMouseWithTouches 为 true 时，可以使用 "Mouse X" 和 "Mouse Y"
            deltaPosition.x = Input.GetAxis("Mouse X");
            deltaPosition.y = Input.GetAxis("Mouse Y");
        }
        return deltaPosition;
    }

}