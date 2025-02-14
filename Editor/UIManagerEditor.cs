using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ultraleap.UI.Editors
{
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {

        private UIManager _manager;

        private GUIContent _openButton;
        private List<MultiImageTarget> _graphics;

        private void Awake()
        {
            _openButton = EditorGUIUtility.IconContent("d_ScriptableObject Icon", "| Open the theme file.");
        }

        public override void OnInspectorGUI()
        {
            if (_manager == null)
            {
                _manager = (UIManager)serializedObject.targetObject;
            }

            DrawDefaultInspector();

            if (_manager.skin != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Skin Themes", EditorStyles.boldLabel);
                foreach (var element in _manager.skin.themes)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(element.elementName, EditorStyles.boldLabel);
                    if (GUILayout.Button(_openButton, GUILayout.Width(26), GUILayout.Height(EditorStyles.miniButton.fixedHeight)))
                    {
                        Selection.activeObject = element;
                    }
                    EditorGUILayout.EndHorizontal();

                    int count = 0;
                    foreach (var target in _manager.GetAllUsingTheme(element))
                    {
                        EditorGUILayout.ObjectField(target.gameObject.name, target, typeof(MultiImageTarget), true);
                        count++;
                    }
                    if(count == 0)
                    {
                        EditorGUILayout.LabelField("No targets using this theme.");
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}