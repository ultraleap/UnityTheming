using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    // DO NOT DELETE, MAY WANT TO IMPLEMENT IN FUTURE
    //[CustomPropertyDrawer(typeof(GradientItem))]
    //public class GradientItemEditor : PropertyDrawer
    //{
    //    // Draw the property inside the given rect
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        // Using BeginProperty / EndProperty on the parent property means that
    //        // prefab override logic works on the entire property.

    //        EditorGUI.PropertyField(position, property, label, true);

    //        if (property.isExpanded)
    //        {
    //            //position.y += position.height;
    //            Rect pos = new Rect(position.x + 4, position.y + position.height - EditorGUIUtility.singleLineHeight - 2, position.width-4, EditorGUIUtility.singleLineHeight);
    //            bool wide = EditorGUIUtility.wideMode;
    //            EditorGUIUtility.wideMode = true;
    //            property.FindPropertyRelative("roundedCorners").vector4Value = EditorGUI.Vector4Field(pos, new GUIContent("Rounded Corners", "Dictactes whether the item will be rounded. " +
    //                "If the RoundedCorners script is found it will use the first value. " +
    //                "If the IndividualRoundedCorners script is found it will use all four."), property.FindPropertyRelative("roundedCorners").vector4Value);
    //            EditorGUIUtility.wideMode = wide;
    //        }
    //    }

    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        return EditorGUI.GetPropertyHeight(property) + (property.isExpanded ? EditorGUIUtility.singleLineHeight + 4 : 0);
    //    }
    //}
}