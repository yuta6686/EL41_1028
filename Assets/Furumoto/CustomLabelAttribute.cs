/*******************************************************************************
*
*	�^�C�g���F	�F�ύX�A�g���r���[�g
*	�t�@�C���F	SceneNameEnumCreator.cs
*	�쐬�ҁF	�Ö{ �ח�
*	������F    2023/09/17
*
*******************************************************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomLabelAttribute : PropertyAttribute
{
    public readonly string Value;

    public CustomLabelAttribute(string value)
    {
        Value = value;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomLabelAttribute))]
public class CustomLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var newLabel = attribute as CustomLabelAttribute;
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel.Value), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
#endif
