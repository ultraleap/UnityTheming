using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ultraleap.UI.Editors
{
    public static class EditorHelper
    {
        public static bool CheckPathExists(string path)
        {
            return AssetDatabase.GetMainAssetTypeAtPath(path) != null;
        }

        public static string GetAssetFolder(Object target)
        {
            string path = AssetDatabase.GetAssetPath(target);
            int index = path.LastIndexOf("/");
            if (index >= 0)
                path = path.Substring(0, index);

            return path;
        }

        public static string GetAssetName(Object target)
        {
            string path = AssetDatabase.GetAssetPath(target);
            int index = path.LastIndexOf("/");
            if (index >= 0)
                path = path.Substring(index + 1, path.Length - index - 1);

            index = path.LastIndexOf(".");
            if(index >= 0)
                path = path.Substring(0, index);

            return path;
        }

        public static void UpdateSkin()
        {
            UIManager manager = GameObject.FindAnyObjectByType<UIManager>();
            manager.AssignTheme();
        }
    }
}