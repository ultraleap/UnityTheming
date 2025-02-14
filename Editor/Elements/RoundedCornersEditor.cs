using UnityEditor;
using UnityEngine;

namespace Ultraleap.UI.Editors
{
    [CustomEditor(typeof(RoundedCorners))]
    public class RoundedCornersEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button(new GUIContent("Force Unique Material", "Forces the rounded corner material to be unique, preventing duplication issues between other elements.")))
            {
                RoundedCorners corners = (RoundedCorners)serializedObject.targetObject;
                corners.SeperateMaterial();
                EditorUtility.SetDirty(corners);
                EditorHelper.UpdateSkin();
            }
            DrawDefaultInspector();
        }
    }
}