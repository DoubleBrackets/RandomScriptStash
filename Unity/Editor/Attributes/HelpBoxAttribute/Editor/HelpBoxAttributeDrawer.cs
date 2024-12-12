using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as HelpBoxAttribute;
        EditorGUILayout.HelpBox(attr.Text, MessageType.Info);
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * attr.Space);
        EditorGUILayout.PropertyField(property, label, true);
    }
}