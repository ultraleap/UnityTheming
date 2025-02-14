using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    [CustomPropertyDrawer(typeof(GradientState))]
    public class GradientStateEditor : PropertyDrawer
    {
        private const float _leftSide = 90f;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.

            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            GUI.BeginGroup(position, EditorStyles.helpBox);
            GUI.EndGroup();

            var titleRect = position;
            titleRect.x += 4;
            titleRect.y -= (EditorGUIUtility.singleLineHeight * 2) - 5;

            float colourWidth = ((position.width - _leftSide) / 2) - 6;
            EditorGUI.LabelField(titleRect, label, EditorStyles.boldLabel);
            titleRect.x += 90;
            titleRect.width = colourWidth;
            EditorGUI.LabelField(titleRect, "Basic");
            titleRect.x += colourWidth;
            EditorGUI.LabelField(titleRect, "Active");

            // Calculate rects
            var topRect = new Rect(position.x + 4, position.y + (EditorGUIUtility.singleLineHeight) + 4, _leftSide, EditorGUIUtility.singleLineHeight);
            var bottomRect = new Rect(position.x + 4, position.y + (EditorGUIUtility.singleLineHeight * 2) + 6, _leftSide, EditorGUIUtility.singleLineHeight);
            var scaleRect = new Rect(position.x + 4, position.y + (EditorGUIUtility.singleLineHeight * 3) + 8, _leftSide, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(topRect, "Top");
            EditorGUI.LabelField(bottomRect, "Bottom");
            topRect.x += _leftSide;
            topRect.width = colourWidth;
            bottomRect.x += _leftSide;
            bottomRect.width = colourWidth;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(topRect, property.FindPropertyRelative("basic"), GUIContent.none);
            EditorGUI.PropertyField(bottomRect, property.FindPropertyRelative("basicSecondary"), GUIContent.none);

            topRect.x += colourWidth + 4;
            bottomRect.x += colourWidth + 4;
            EditorGUI.PropertyField(topRect, property.FindPropertyRelative("active"), GUIContent.none);
            EditorGUI.PropertyField(bottomRect, property.FindPropertyRelative("activeSecondary"), GUIContent.none);

            EditorGUI.LabelField(scaleRect, new GUIContent("Scale","A uniform scale value to apply to the element. Does not scale the whole UI element."));
            scaleRect.x += _leftSide;
            scaleRect.width = colourWidth / 2f;
            EditorGUI.PropertyField(scaleRect, property.FindPropertyRelative("scale"), GUIContent.none);
            scaleRect.x += colourWidth + 4;
            EditorGUI.PropertyField(scaleRect, property.FindPropertyRelative("activeScale"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4 + 10;
        }
    }
}