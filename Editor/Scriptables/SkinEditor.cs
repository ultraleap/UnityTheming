using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    [CustomEditor(typeof(Skin))]
    public class SkinEditor : Editor
    {
        SerializedProperty skinElements;

        private UIManager _uiManager;

        public override void OnInspectorGUI()
        {
            skinElements = serializedObject.FindProperty("themes");

            if (_uiManager == null)
            {
                _uiManager = FindAnyObjectByType<UIManager>();
            }

            bool currentSkinIsThis = _uiManager != null && _uiManager.skin != (Skin)serializedObject.targetObject;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Element"))
            {
                CreateNewElement();
            }

            GUI.enabled = !currentSkinIsThis;
            if (GUILayout.Button(new GUIContent("Apply Changes", "Forces the UI Manager to apply any changes to your skin.")))
            {
                _uiManager.AssignTheme();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (_uiManager == null)
            {
                if (GUILayout.Button(new GUIContent("Create UI Manager", _uiManager == null ? "Adds a new object to the scene and assigns this skin to the manager." : "This scene already has a UI manager.")))
                {
                    GameObject gameObject = new GameObject();
                    _uiManager = gameObject.AddComponent<UIManager>();
                    _uiManager.skin = (Skin)serializedObject.targetObject;
                    _uiManager.AssignTheme();
                }
            }
            else
            {
                if(GUILayout.Button("Swap to UI Manager"))
                {
                    Selection.activeObject = _uiManager;
                }
            }

            GUI.enabled = currentSkinIsThis;
            if (GUILayout.Button(new GUIContent("Set This As Current Skin", currentSkinIsThis ? "Your UI is already using this skin." : "Changes the UI to this skin and applies it to all elements.")))
            {
                _uiManager.skin = (Skin)serializedObject.targetObject;
                _uiManager.AssignTheme();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultFont"));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(skinElements, new GUIContent("Theme Elements"));
            if (EditorGUI.EndChangeCheck())
            {
                _uiManager.RefreshThemes();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateNewElement()
        {
            ScriptableObject themeScriptable = CreateInstance(typeof(ThemeElement));

            ThemeElement themeElement = (ThemeElement)themeScriptable;

            string themeElementName = serializedObject.FindProperty("m_Name").stringValue + " Element ";
            themeScriptable.name = themeElementName + (skinElements.arraySize + 1);

            string path = EditorHelper.GetAssetFolder(serializedObject.targetObject);

            int append = skinElements.arraySize + 1;

            string fullPath = path + "/" + themeElementName + append + ".asset";

            while (EditorHelper.CheckPathExists(fullPath))
            {
                append++;
                fullPath = path + "/" + themeElementName + append + ".asset";
            }

            themeElement.elementName = themeElementName + append;

            AssetDatabase.CreateAsset(themeScriptable, fullPath);

            skinElements.InsertArrayElementAtIndex(skinElements.arraySize);

            serializedObject.ApplyModifiedProperties();

            skinElements.GetArrayElementAtIndex(skinElements.arraySize - 1).objectReferenceValue = themeScriptable;

            skinElements.isExpanded = true;

            EditorUtility.SetDirty(serializedObject.targetObject);
            AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);

            AssetDatabase.Refresh();
            _uiManager.RefreshThemes();
        }
    }
}