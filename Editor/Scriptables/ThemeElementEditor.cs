using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Ultraleap.UI.Editors
{
    [CustomEditor(typeof(ThemeElement))]
    public class ThemeElementEditor : Editor
    {
        private const string THEME_ELEMENT_NAME = "theme-elementName";

        private GUIContent applyButton = new GUIContent("Apply Changes", "Will tell the UI manager to apply these changes to all objects in the scene with the relevant theme type.");
        private GUIContent addToCurrentTheme = new GUIContent("Add To Current Skin", "Adds this theme element to the currently loaded skin, dictated by the UI Manager in the scene.");
        private GUIContent goToCurrentTheme = new GUIContent("Swap To Current Skin", "Changes context to the current skin.");
        private GUIContent goToUIManager = new GUIContent("Swap To UI Manager", "Changes context to the UI Manager.");

        private UIManager _uiManager;
        private List<MultiImageTarget> _targets = new List<MultiImageTarget>();

        private ThemeElement _themeElement;

        private string _currentFocus;

        private void Awake()
        {
            _uiManager = FindObjectOfType<UIManager>(true);
            if (_uiManager != null && _uiManager.skin != null)
            {
                _targets = _uiManager.GetAllUsingTheme((ThemeElement)serializedObject.targetObject);
            }
        }

        public override void OnInspectorGUI()
        {
            if (_uiManager == null)
            {
                EditorGUILayout.HelpBox("Please add a UI Manager to your scene to use the features below.", MessageType.Warning);
            }
            
            if(_themeElement == null)
            {
                _themeElement = (ThemeElement)serializedObject.targetObject;
            }

            EditorGUI.BeginDisabledGroup(_uiManager == null);

            if (GUILayout.Button(applyButton))
            {
                UpdateManager();
            }

            EditorGUI.BeginDisabledGroup(_uiManager == null || _uiManager.skin == null);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(_uiManager != null && _uiManager.skin != null && _uiManager.skin.themes.Contains(_themeElement));
            if (GUILayout.Button(addToCurrentTheme))
            {
                AddToManager();
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(goToCurrentTheme))
            {
                SelectCurrentSkin();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(_uiManager == null);
            if (GUILayout.Button(goToUIManager))
            {
                SelectUIManager();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(_targets.Count == 0);
            serializedObject.FindProperty("expandedSearch").boolValue = EditorGUILayout.Foldout(serializedObject.FindProperty("expandedSearch").boolValue, "Scene Items Using Element", true);
            if (_targets.Count > 0 && serializedObject.FindProperty("expandedSearch").boolValue)
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    EditorGUILayout.ObjectField(_targets[i], typeof(MultiImageTarget), true);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUI.SetNextControlName(THEME_ELEMENT_NAME);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("elementName"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }

            if (_currentFocus == THEME_ELEMENT_NAME && GUI.GetNameOfFocusedControl() != THEME_ELEMENT_NAME)
            {
                RenameThemeElement();
            }

            if (GUILayout.Button(new GUIContent("Rename", "This will automatically happen when you lose focus of the text box, but won't fully work if you navigate away."),GUILayout.Width(80)))
            {
                RenameThemeElement();
            }

            EditorGUILayout.EndHorizontal();

            _currentFocus = GUI.GetNameOfFocusedControl();

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        private void UpdateManager()
        {
            if (_uiManager != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                _uiManager.ApplyThemeChange(_themeElement);
            }
        }

        private void AddToManager()
        {
            if (_uiManager != null && _uiManager.skin != null)
            {
                _uiManager.skin.themes.Add(_themeElement);
                EditorUtility.SetDirty(_uiManager.skin);
                AssetDatabase.SaveAssetIfDirty(_uiManager.skin);
            }
        }

        private void SelectCurrentSkin()
        {
            if (_uiManager != null && _uiManager.skin != null)
            {
                Selection.activeObject = _uiManager.skin;
            }
        }

        private void SelectUIManager()
        {
            if(_uiManager != null)
            {
                Selection.activeObject = _uiManager;
            }
        }

        private void RenameThemeElement()
        {
            string elementName = serializedObject.FindProperty("elementName").stringValue;

            if(elementName == EditorHelper.GetAssetName(serializedObject.targetObject))
            {
                return;
            }

            int append = 0;

            string path = EditorHelper.GetAssetFolder(serializedObject.targetObject);

            string fullPath = path + "/" + elementName + ".asset";

            while (EditorHelper.CheckPathExists(fullPath))
            {
                append++;
                fullPath = path + "/" + elementName + " " + append + ".asset";
            }

            serializedObject.FindProperty("elementName").stringValue = elementName + (append > 0 ? (" " + append) : "");

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(serializedObject.targetObject), elementName + (append > 0 ? (" " + append) : "") + ".asset");
            AssetDatabase.Refresh();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            RenameThemeElement();
        }
    }
}