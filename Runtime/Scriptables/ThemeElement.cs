using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultraleap.UI
{
    [CreateAssetMenu(fileName = "Ultraleap UI Element", menuName = "Ultraleap/UI Skin Element")]
    public class ThemeElement : ScriptableObject
    {
        [HideInInspector]
        public string elementName = "";

        [SerializeField]
        public List<GraphicItem> graphicThemes = new List<GraphicItem>();

        [SerializeField]
        public List<GradientItem> gradientThemes = new List<GradientItem>();

        [SerializeField]
        public List<TextItem> textThemes = new List<TextItem>();

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField,HideInInspector]
        private bool expandedSearch = false;
#pragma warning restore 0414
#endif
    }
}