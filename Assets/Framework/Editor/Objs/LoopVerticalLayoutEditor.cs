#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LoopVerticalLayout)), CanEditMultipleObjects]
public class LoopVerticalLayoutEditor : Editor {

    private SerializedProperty m_script;
    private SerializedProperty m_childrenOrder;
    private SerializedProperty m_spacing;
    private SerializedProperty m_isScaleChildren;
    private SerializedProperty m_minScale;
    private SerializedProperty m_offsetBeyondViewport;
    private SerializedProperty m_scrollRect;

    private ReorderableList m_reorderableList;

    private void OnEnable () {
        m_script = serializedObject.FindProperty("m_Script");
        m_childrenOrder = serializedObject.FindProperty("childrenOrder");
        m_spacing = serializedObject.FindProperty("spacing");
        m_isScaleChildren = serializedObject.FindProperty("isScaleChildren");
        m_minScale = serializedObject.FindProperty("minScale");
        m_offsetBeyondViewport = serializedObject.FindProperty("m_offsetBeyondViewport");
        m_scrollRect = serializedObject.FindProperty("m_scrollRect");

        m_reorderableList = new ReorderableList(serializedObject, m_childrenOrder);
        m_reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = m_childrenOrder.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        };
        m_reorderableList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, new GUIContent(m_childrenOrder.displayName));
        };
    }

    public override void OnInspectorGUI () {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(m_script);
        GUI.enabled = true;

        m_reorderableList.DoLayoutList();
        EditorGUILayout.PropertyField(m_spacing);
        EditorGUILayout.PropertyField(m_isScaleChildren);
        EditorGUILayout.PropertyField(m_minScale);
        EditorGUILayout.PropertyField(m_offsetBeyondViewport);
        EditorGUILayout.PropertyField(m_scrollRect);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
