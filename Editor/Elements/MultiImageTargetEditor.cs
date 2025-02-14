using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    [CustomEditor(typeof(MultiImageTarget))]
    public class MultiImageTargetEditor : Editor
    {
        private int themeInd = -1;
        private List<string> themes = new List<string>();

        private UIManager manager = null;

        private GUIContent openButton, refreshButton;

        private void Awake()
        {
            openButton = EditorGUIUtility.IconContent("d_ScriptableObject Icon", "| Open the theme file.");
            refreshButton = EditorGUIUtility.IconContent("Refresh", "| Refresh the theme.");
        }

        public override void OnInspectorGUI()
        {
            bool wasNull = manager == null;
            if (manager == null)
            {
                manager = FindObjectOfType<UIManager>();
            }

            if (manager == null)
            {
                EditorGUILayout.HelpBox("Please add a UI Manager to your scene.", MessageType.Warning);
            }
            else
            {
                if (wasNull)
                {
                    manager.RefreshThemes();
                }
                if (manager.skinThemes == null)
                {
                    themes = new List<string>();
                }
                else
                {
                    themes = new List<string>(manager.skinThemes);
                }
                themes.Insert(0, "None");
                if (themeInd == -1 && themes.Count > 1)
                {
                    themeInd = manager.skin.themes.FindIndex(x => x.elementName == ((MultiImageTarget)serializedObject.targetObject).themeName) + 1;
                }
                if (themeInd == -1)
                {
                    themeInd = 0;
                }
            }


            MultiImageTarget multiImageTarget = (MultiImageTarget)serializedObject.targetObject;
            EditorGUILayout.LabelField("Targets");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find First"))
            {
                multiImageTarget.FindTargets(true);
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            if (GUILayout.Button("Find All"))
            {
                multiImageTarget.FindTargets(false);
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(multiImageTarget.gameObject.scene.name == null || themes.Count == 1);

            if (themes.Count == 1)
            {
                EditorGUILayout.HelpBox("Please assign a UI Skin to your UI Manager.", MessageType.Warning);
            }

            GUI.enabled = true;
            if (GUILayout.Button("Swap to UI Manager"))
            {
                Selection.activeObject = manager;
            }

            GUI.enabled = false;
            EditorGUILayout.ObjectField(new GUIContent("Current Skin", "The skin currently loaded in the UI manager."), manager?.skin, typeof(Skin), allowSceneObjects: true);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();

            int oldIndex = themeInd;
            themeInd = EditorGUILayout.Popup("Theme", themeInd, themes.ToArray());

            EditorGUI.BeginDisabledGroup(themeInd == 0);
            if (GUILayout.Button(refreshButton, GUILayout.Width(26), GUILayout.Height(EditorStyles.miniButton.fixedHeight)))
            {
                multiImageTarget.ApplyTheme();
            }
            if (GUILayout.Button(openButton, GUILayout.Width(26), GUILayout.Height(EditorStyles.miniButton.fixedHeight)))
            {
                Selection.activeObject = serializedObject.FindProperty("themeObject").objectReferenceValue;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            if (themeInd != oldIndex)
            {
                serializedObject.FindProperty("themeObject").objectReferenceValue = themeInd == 0 ? null : manager.skin.themes[themeInd - 1];
                multiImageTarget.ApplyTheme();
            }

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            if (serializedObject.FindProperty("_noGraphics").boolValue)
            {
                EditorGUILayout.HelpBox("There are no target graphics, but the current theme has graphic targets.\nNo graphic theming will be applied.", MessageType.Warning);
            }
            if(serializedObject.FindProperty("_noGradients").boolValue)
            {
                EditorGUILayout.HelpBox("There are no target gradients, but the current theme has gradient targets.\nNo gradient theming will be applied.", MessageType.Warning);
            }
            DrawDefaultInspector();
        }
    }
}