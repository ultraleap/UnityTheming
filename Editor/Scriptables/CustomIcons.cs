using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Callbacks;

namespace Ultraleap.UI
{

    public class GizmoIconUtility
    {

        [DidReloadScripts]
        static GizmoIconUtility()
        {
            EditorApplication.projectWindowItemOnGUI = ItemOnGUI;
        }

        static void ItemOnGUI(string guid, Rect rect)
        {

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // - - - 

            ThemeElement themeElement = AssetDatabase.LoadAssetAtPath(assetPath, typeof(ThemeElement)) as ThemeElement;

            if (themeElement != null)
            {

                Rect rbase = rect;
                if (rbase.height >= rbase.width)
                {
                    rbase.height -= 14; // fix for vertical grid layout
                }
                else
                {
                    rbase.width = 20; // fix for horizontal list layout
                }

                DrawGUIRoundedBasicTexture(rbase,Color.gray);

                // Draw a texture preview over the asset icon in the project window
                if (themeElement.gradientThemes.Count > 0)
                {
                    DrawGUIRoundedBasicTexture(rbase,new GraphicState[] { themeElement.gradientThemes[0].normal, themeElement.gradientThemes[0].hover, themeElement.gradientThemes[0].pressed });
                }
                else if (themeElement.graphicThemes.Count > 0)
                {
                    DrawGUIRoundedBasicTexture(rbase, new GraphicState[] { themeElement.graphicThemes[0].normal, themeElement.graphicThemes[0].hover, themeElement.graphicThemes[0].pressed });
                }

            }
        }



        static void DrawGUIRoundedBasicTexture(Rect _pos, Color _color)
        {
            _pos.width = _pos.width - 4;
            _pos.x += 2;
            GUI.DrawTexture(_pos, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1, _color, 0, _pos.width * 0.2f);
        }

        static void DrawGUIRoundedBasicTexture(Rect _pos, GraphicState[] _colors)
        {
            float height = _pos.height / _colors.Length;
            _pos.height = height;
            _pos.width = _pos.width - 6;
            _pos.x += 3;

            for (int i = 0; i < _colors.Length; i++)
            {
                GUI.DrawTexture(_pos, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1, _colors[i].basic, 0, _pos.width * .2f);
                _pos.y += height;
            }
        }
    }
}