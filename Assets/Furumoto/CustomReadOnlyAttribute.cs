/*******************************************************************************
*
*	タイトル：	編集不可アトリビュート
*	ファイル：	CustomReadOnlyAttribute.cs
*	作成者：	古本 泰隆
*	制作日：    2023/09/17
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 表示だけ
/// </summary>
public class CustomReadOnlyAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif
