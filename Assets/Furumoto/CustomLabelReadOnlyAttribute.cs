/*******************************************************************************
*
*	�^�C�g���F	�ϐ��\�����ύX�E�ҏW�s�A�g���r���[�g
*	�t�@�C���F	CustomLabelReadOnlyAttribute.cs
*	�쐬�ҁF	�Ö{ �ח�
*	������F    2023/09/17
*
*******************************************************************************/
using UnityEditor;
using UnityEngine;

public class CustomLabelReadOnlyAttribute : PropertyAttribute
{
    public string Name { get; }

    public CustomLabelReadOnlyAttribute(string name) => Name = name;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CustomLabelReadOnlyAttribute))]
    class CustomLabelReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            string[] path = property.propertyPath.Split('.');
            bool isArray = path.Length > 1 && path[1] == "Array";

            if (!isArray && attribute is CustomLabelReadOnlyAttribute fieldName)
                label.text = fieldName.Name;

            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}
