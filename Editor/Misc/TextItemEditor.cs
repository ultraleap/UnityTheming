using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    [CustomPropertyDrawer(typeof(TextItem))]
    public class TextItemEditor : PropertyDrawer
    {
        private SerializedProperty _changeFontSize, _changeTextAlignment, _autoFontSize, _useDefaultFont;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.

            _changeFontSize = property.FindPropertyRelative("changeFontSizes");
            _changeTextAlignment = property.FindPropertyRelative("changeTextAlignment");
            _autoFontSize = property.FindPropertyRelative("useAutoFontSizes");
            _useDefaultFont = property.FindPropertyRelative("useDefaultFont");

            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var titleRect = position;
            titleRect.x += 4;
            titleRect.y -= (position.height / 2f) - 8;

            float colourWidth = ((position.width - 90) / 4f) - 5.5f;
            EditorGUI.LabelField(titleRect, label, EditorStyles.boldLabel);

            // Calculate rects
            int rows = 1;
            float extraSpacing = 0;
            var sizeRect = new Rect(position.x + 4, position.y + GetRowHeight(rows), 90, EditorGUIUtility.singleLineHeight);

            Rect background = new Rect(sizeRect.x - 4, sizeRect.y - 2, position.width,
                GetLineHeight(GetPropertyActiveElements(property) 
                - (property.FindPropertyRelative("changeFontSizes").boolValue ? 0 : -1)
                - (property.FindPropertyRelative("changeTextAlignment").boolValue ? 2 : 0)
                - (property.FindPropertyRelative("useDefaultFont").boolValue ? 1 : 2)
                +4));

            GUI.BeginGroup(background, EditorStyles.helpBox);
            GUI.EndGroup();

            EditorGUI.LabelField(sizeRect, "State", EditorStyles.boldLabel);
            sizeRect.x += 90;
            EditorGUI.LabelField(sizeRect, "Normal");
            sizeRect.x += colourWidth + 4;
            EditorGUI.LabelField(sizeRect, "Hover");
            sizeRect.x += colourWidth + 4;
            EditorGUI.LabelField(sizeRect, "Pressed");
            sizeRect.x += colourWidth + 4;
            EditorGUI.LabelField(sizeRect, "Disabled");
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            EditorGUI.LabelField(sizeRect, "Basic Colour");
            sizeRect.x += 90;
            sizeRect.width = colourWidth;
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("normal").FindPropertyRelative("textColorBasic"), GUIContent.none);
            DrawTextColorOptions(property, ref sizeRect, colourWidth, "hover", "ignoreHover", "Basic");
            DrawTextColorOptions(property, ref sizeRect, colourWidth, "pressed", "ignorePressed", "Basic");
            DrawTextColorOptions(property, ref sizeRect, colourWidth, "disabled", "ignoreDisabled", "Basic");
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);


            EditorGUI.LabelField(sizeRect, "Active Colour");
            sizeRect.x += 90;
            sizeRect.width = colourWidth;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("normal").FindPropertyRelative("textColorActive"), GUIContent.none);
            DrawTextColorOptions(property, ref sizeRect, colourWidth, "hover", "ignoreHover", "Active");
            DrawTextColorOptions(property, ref sizeRect, colourWidth, "pressed", "ignorePressed", "Active");
            DrawTextColorOptions(property, ref sizeRect, colourWidth, "disabled", "ignoreDisabled", "Active");
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            if (_changeFontSize.boolValue)
            {
                if (_autoFontSize.boolValue)
                {
                    EditorGUI.LabelField(sizeRect, "Min Size");
                    sizeRect.x += 90;
                    sizeRect.width = colourWidth;
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "normal", "");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "hover", "ignoreHover");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "pressed", "ignorePressed");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "disabled", "ignoreDisabled");
                    RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

                    EditorGUI.LabelField(sizeRect, "Max Size");
                    sizeRect.x += 90;
                    sizeRect.width = colourWidth;
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, false, "normal", "");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, false, "hover", "ignoreHover");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, false, "pressed", "ignorePressed");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, false, "disabled", "ignoreDisabled");
                }
                else
                {
                    EditorGUI.LabelField(sizeRect, "Font Size");
                    sizeRect.x += 90;
                    sizeRect.width = colourWidth;
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "normal", "");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "hover", "ignoreHover");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "pressed", "ignorePressed");
                    DrawTextSizeOptions(property, ref sizeRect, colourWidth, _autoFontSize.boolValue, true, "disabled", "ignoreDisabled");
                }
            }
            extraSpacing = 2;
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);


            EditorGUI.LabelField(sizeRect, "Copy Previous");
            sizeRect.x += 90 + colourWidth + 4;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("ignoreHover"), GUIContent.none);
            sizeRect.x += colourWidth + 4;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("ignorePressed"), GUIContent.none);
            sizeRect.x += colourWidth + 4;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("ignoreDisabled"), GUIContent.none);

            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            sizeRect.width = position.width - 4;
            EditorGUI.PropertyField(sizeRect, _changeFontSize);
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            if (_changeFontSize.boolValue)
            {
                sizeRect.width = position.width - 4;
                EditorGUI.PropertyField(sizeRect, _autoFontSize);
                RestoreRect(position, ref sizeRect, ref rows, extraSpacing);
            }

            sizeRect.width = position.width - 4;
            EditorGUI.PropertyField(sizeRect, _useDefaultFont);
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            if (!_useDefaultFont.boolValue)
            {
                sizeRect.width = position.width - 4;
                EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("font"));
                RestoreRect(position, ref sizeRect, ref rows, extraSpacing);
            }

            sizeRect.width = position.width - 4;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("fontStyles"));
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            sizeRect.width = position.width - 4;
            EditorGUI.PropertyField(sizeRect, _changeTextAlignment);
            RestoreRect(position, ref sizeRect, ref rows, extraSpacing);

            if (_changeTextAlignment.boolValue)
            {
                EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("textAlignment"));
                extraSpacing = 6;
                RestoreRect(position, ref sizeRect, ref rows, extraSpacing);
                if (position.width < 448)
                {
                    RestoreRect(position, ref sizeRect, ref rows, extraSpacing);
                }
            }

            sizeRect.width = position.width - 8;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("transitionTime"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void RestoreRect(Rect original, ref Rect current, ref int rows, float extraSpacing)
        {
            rows++;
            current.x = original.x + 4;
            current.y = original.y + GetRowHeight(rows) + extraSpacing;
            current.width = 90;
        }

        private void DrawTextColorOptions(SerializedProperty property, ref Rect topRect, float colourWidth, string item, string ignore, string colorType)
        {
            topRect.x += colourWidth + 4;
            if (!property.FindPropertyRelative(ignore).boolValue)
            {
                EditorGUI.PropertyField(topRect, property.FindPropertyRelative(item).FindPropertyRelative("textColor" + colorType), GUIContent.none);
            }
        }

        private void DrawTextSizeOptions(SerializedProperty property, ref Rect position, float width, bool autoSizing, bool min, string item, string ignore)
        {
            Vector3 value = property.FindPropertyRelative(item).FindPropertyRelative("fontSizes").vector3Value;
            float singleValue;
            if (autoSizing)
            {
                if (min)
                {
                    singleValue = value.y;
                }
                else
                {
                    singleValue = value.z;
                }
            }
            else
            {
                singleValue = value.x;
            }

            if (ignore == "" || !property.FindPropertyRelative(ignore).boolValue)
            {
                singleValue = EditorGUI.FloatField(position, singleValue);
            }

            if (autoSizing)
            {
                if (min)
                {
                    value.y = singleValue;
                }
                else
                {
                    value.z = singleValue;
                }
            }
            else
            {
                value.x = singleValue;
            }

            property.FindPropertyRelative(item).FindPropertyRelative("fontSizes").vector3Value = value;

            position.x += width + 4;
        }

        private float GetRowHeight(int row)
        {
            return (EditorGUIUtility.singleLineHeight * row) + ((2 * row) + 2);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int rows = 10 + GetPropertyActiveElements(property);
            return GetLineHeight(rows);
        }

        private float GetLineHeight(int rows)
        {
            return EditorGUIUtility.singleLineHeight * rows + ((2 * rows) + 6);
        }

        private int GetPropertyActiveElements(SerializedProperty property)
        {
            return (property.FindPropertyRelative("changeFontSizes").boolValue ? (property.FindPropertyRelative("useAutoFontSizes").boolValue ? 4 : 3) : 1) +
                 (property.FindPropertyRelative("changeTextAlignment").boolValue ? 2 : 0) +
                (property.FindPropertyRelative("useDefaultFont").boolValue ? 1 : 2);
        }
    }
}