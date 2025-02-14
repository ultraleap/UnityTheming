using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    [CustomPropertyDrawer(typeof(GraphicState))]
    public class GraphicStateEditor : PropertyDrawer
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
            titleRect.y -= (EditorGUIUtility.singleLineHeight + 2);

            float colourWidth = ((position.width - _leftSide) / 2) - 6;
            EditorGUI.LabelField(titleRect, label, EditorStyles.boldLabel);
            titleRect.x += _leftSide;
            titleRect.width = colourWidth;
            EditorGUI.LabelField(titleRect, new GUIContent("Basic", "Basic colours are used as the default for elements that do not have a state."));
            titleRect.x += colourWidth;
            EditorGUI.LabelField(titleRect, new GUIContent("Active", "Active colours are used for items such as Toggles where there is an on and off state."));


            // Calculate rects
            var colourRect = new Rect(position.x + 4, position.y + (EditorGUIUtility.singleLineHeight) + 4, _leftSide, EditorGUIUtility.singleLineHeight);
            var scaleRect = new Rect(position.x + 4, position.y + (EditorGUIUtility.singleLineHeight * 2) + 6, _leftSide, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(colourRect, "Colour");
            colourRect.x += _leftSide;
            colourRect.width = colourWidth;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(colourRect, property.FindPropertyRelative("basic"), GUIContent.none);
            colourRect.x += colourWidth + 4;
            EditorGUI.PropertyField(colourRect, property.FindPropertyRelative("active"), GUIContent.none);

            EditorGUI.LabelField(scaleRect, new GUIContent("Scale", "A uniform scale value to apply to the element. Does not scale the whole UI element."));
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
            return EditorGUIUtility.singleLineHeight * 3 + 8;
        }
    }
}