using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Ultraleap.UI.Editors
{
    [CustomPropertyDrawer(typeof(GraphicItem))]
    [CustomPropertyDrawer(typeof(GradientItem))]
    public class GraphicItemEditor : PropertyDrawer
    {
        private float _lineHeight = EditorGUIUtility.singleLineHeight + 2;

        private string _gradientTypeName = "";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.

            EditorGUI.PropertyField(position, property, label, true);

            if (property.isExpanded)
            {
                _gradientTypeName = typeof(GradientItem).ToString().Split('.').Last();

                Rect pos = new Rect(position.x + 4, position.y + position.height - GetExtraHeight(property) - 2, position.width - 4, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(pos, property.FindPropertyRelative("transitionTime"));
                pos.y += _lineHeight;

                if (property.type == _gradientTypeName)
                {
                    EditorGUI.PropertyField(pos, property.FindPropertyRelative("gradientAngle"));
                    pos.y += _lineHeight;
                }

                EditorGUI.PropertyField(pos, property.FindPropertyRelative("useRoundedCorners"));

                if (!property.FindPropertyRelative("useRoundedCorners").boolValue)
                {
                    return;
                }
                pos.y += _lineHeight;
                pos.width = position.width / 2;
                EditorGUI.PropertyField(pos, property.FindPropertyRelative("individualCorners"), new GUIContent("Individual Corners", "Allows individual control of the roundedness of each corner. If you do not need this then do not tick as it is slightly less performant."));
                pos.x += pos.width;
                EditorGUI.PropertyField(pos, property.FindPropertyRelative("copyHeightRound"), new GUIContent("Copy Height", "Dynamically copies the height of the object to decide the rounding of the object."));
                pos.x = position.x + 4;
                pos.width = position.width - 4;
                pos.y += _lineHeight;
                if (property.FindPropertyRelative("individualCorners").boolValue)
                {
                    if (!property.FindPropertyRelative("copyHeightRound").boolValue)
                    {
                        property.FindPropertyRelative("roundedCornersRadius").vector4Value = EditorGUI.Vector4Field(pos, new GUIContent("Corner Radius", "Sets the rounding amount."), property.FindPropertyRelative("roundedCornersRadius").vector4Value);
                    }
                }
                else
                {
                    if (!property.FindPropertyRelative("copyHeightRound").boolValue)
                    {
                        Vector4 radius = property.FindPropertyRelative("roundedCornersRadius").vector4Value;
                        radius[0] = EditorGUI.FloatField(pos, "Corner Radius", radius[0]);
                        property.FindPropertyRelative("roundedCornersRadius").vector4Value = radius;
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) + (property.isExpanded ? GetExtraHeight(property) + 4 : 0);
        }

        private float GetExtraHeight(SerializedProperty property)
        {
            return _lineHeight + (property.type == _gradientTypeName ? _lineHeight : 0) + (property.FindPropertyRelative("useRoundedCorners").boolValue ? _lineHeight * 2 : _lineHeight) +
                (!property.FindPropertyRelative("useRoundedCorners").boolValue || property.FindPropertyRelative("copyHeightRound").boolValue ? 0 : _lineHeight) +
                (!EditorGUIUtility.wideMode && property.FindPropertyRelative("individualCorners").boolValue && !property.FindPropertyRelative("copyHeightRound").boolValue ? _lineHeight : 0);
        }
    }
}