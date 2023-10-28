/*******************************************************************************
*
*	タイトル：	色変更アトリビュート
*	ファイル：	CustomColorAttribute.cs
*	作成者：	古本 泰隆
*	制作日：    2023/09/17
*
*******************************************************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomColorAttribute : PropertyAttribute
{
    public Color color;

    public CustomColorAttribute(float r, float g, float b, float a)
    {
        color = new Color(r, g, b, a);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomColorAttribute))]
public class CustomColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var colorMe = attribute as CustomColorAttribute;
        if (colorMe != null) GUI.color = colorMe.color;
        EditorGUI.PropertyField(position, property, label);
        GUI.color = Color.white;
    }
}
#endif
