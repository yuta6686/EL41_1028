/*******************************************************************************
*
*	タイトル：	配列要素表示名変更アトリビュート
*	ファイル：	CustomArrayLabelAttribute.cs
*	作成者：	古本 泰隆
*	制作日：    2023/09/17
*
*******************************************************************************/
using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 配列のラベルを列挙型で定義した名前としてインスペクターで表示する
/// </summary>
public class CustomArrayLabelAttribute : PropertyAttribute
{
    private readonly string[] _names;

    public CustomArrayLabelAttribute(Type enumType) => _names = Enum.GetNames(enumType);
    public CustomArrayLabelAttribute(string[] names) => _names = names;

#if UNITY_EDITOR
    /// <summary>
    /// インスペクターへ反映
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomArrayLabelAttribute))]
    private class CustomArrayLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var names = ((CustomArrayLabelAttribute)attribute)._names;
            var index = int.Parse(property.propertyPath.Split('[', ']').Where(c => !string.IsNullOrEmpty(c)).Last());
            if (index < names.Length) label.text = names[index];
            EditorGUI.PropertyField(rect, property, label, includeChildren: true);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }
    }
#endif
}