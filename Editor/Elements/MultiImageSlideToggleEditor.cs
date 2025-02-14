using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ultraleap.UI.Editors
{
    [CustomEditor(typeof(MultiImageSlideToggle))]
    public class MultiImageSlideToggleEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.FindProperty("m_Interactable").boolValue = EditorGUILayout.Toggle("Interactable", serializedObject.FindProperty("m_Interactable").boolValue);
            serializedObject.FindProperty("m_IsOn").boolValue = EditorGUILayout.Toggle("Is On",serializedObject.FindProperty("m_IsOn").boolValue);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Colors").FindPropertyRelative("m_FadeDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onValueChanged"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handleWrapper"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handle"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionTime"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}