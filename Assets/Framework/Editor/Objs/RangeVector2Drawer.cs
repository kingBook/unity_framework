#if UNITY_EDITOR
#pragma warning disable 0649

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangeVector2))]
public class RangeVector2Drawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var minProperty = property.FindPropertyRelative("min");
        var maxProperty = property.FindPropertyRelative("max");
        EditorGUI.BeginProperty(position, label, property);
        {
            float x, w;
            int elementCount = 3;
            for (int i = 0; i < elementCount; i++) {
                w = position.width / elementCount;
                x = position.x + w * i;
                switch (i) {
                    case 0:
                        EditorGUI.LabelField(new Rect(x, position.y, w, position.height), property.displayName);
                        break;
                    case 1:
                        EditorGUI.PropertyField(new Rect(x, position.y, w, position.height), minProperty, GUIContent.none);
                        break;
                    case 2:
                        EditorGUI.PropertyField(new Rect(x, position.y, w, position.height), maxProperty, GUIContent.none);
                        break;
                }
            }
        }
        EditorGUI.EndProperty();
    }

}
#endif