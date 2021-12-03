using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 同步 Button 子级的按下与释放状态。
/// <para> 此脚本对拥有继承自 <see cref="MaskableGraphic"/> (Text, Image, RawImage, ...) 组件的所有子级执行了以下操作：</para>
/// <para> 1) 禁用 raycastTarget 属性，使子级不可以交互</para>
/// <para> 2) 添加 Selectable 组件（如果已有此组件则跳过） </para>
/// <para> 3) 同步 Selectable 组件的 OnPointerEnter、OnPointerExit、OnPointerDown、OnPointerUp 方法 </para>
/// <para> 注意：如果需要调整子对象的按下、经过、释放等状态，请手动给子对象添加 Selectable 组件进行调整。 </para>
/// </summary>
[RequireComponent(typeof(Selectable)), DisallowMultipleComponent]
public class ButtonChildrenSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    private Selectable[] m_selectables;

    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
        for (int i = 0, length = m_selectables.Length; i < length; i++) {
            m_selectables[i].OnPointerEnter(eventData);
        }
    }

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
        for (int i = 0, length = m_selectables.Length; i < length; i++) {
            m_selectables[i].OnPointerExit(eventData);
        }
    }

    void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
        for (int i = 0, length = m_selectables.Length; i < length; i++) {
            m_selectables[i].OnPointerDown(eventData);
        }
    }

    void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
        for (int i = 0, length = m_selectables.Length; i < length; i++) {
            m_selectables[i].OnPointerUp(eventData);
        }
    }

    private void Start () {
        List<Selectable> selectables = new List<Selectable>();
        MaskableGraphic[] maskableGraphics = GetComponentsInChildren<MaskableGraphic>();
        for (int i = 0, length = maskableGraphics.Length; i < length; i++) {
            MaskableGraphic maskableGraphic = maskableGraphics[i];
            if (maskableGraphic.gameObject == gameObject) continue;

            maskableGraphic.raycastTarget = false;
            Selectable selectable = maskableGraphic.gameObject.GetComponent<Selectable>();
            if (!selectable) {
                selectable = maskableGraphic.gameObject.AddComponent<Selectable>();
            }
            selectables.Add(selectable);
        }
        m_selectables = selectables.ToArray();
    }

}
