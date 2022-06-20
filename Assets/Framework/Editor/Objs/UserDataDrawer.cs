#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UserData))]
public class UserDataDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        {
            var typeProp = property.FindPropertyRelative("type");
            var stringValueProp = property.FindPropertyRelative("stringValue");
            var gameObjectValueProp = property.FindPropertyRelative("gameObjectValue");

            float x, w;

            float totalW = position.width;
            float deltaW = totalW / 5.0f;

            x = position.x;
            w = deltaW * 1f;
            EditorGUI.LabelField(new Rect(x, position.y, w, position.height), property.displayName);

            x += w;
            w = deltaW * 2f;
            if (typeProp.intValue == (int)UserData.Type.String) {
                gameObjectValueProp.objectReferenceValue = null;
                EditorGUI.PropertyField(new Rect(x, position.y, w, position.height), stringValueProp, GUIContent.none);
            } else {
                EditorGUI.ObjectField(new Rect(x, position.y, w, position.height), gameObjectValueProp, GUIContent.none);
                stringValueProp.stringValue = null;
            }

            x += w;
            w = deltaW * 2f;
            EditorGUI.PropertyField(new Rect(x, position.y, w, position.height), typeProp, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}
#endif