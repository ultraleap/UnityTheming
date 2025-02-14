using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultraleap.UI
{
    [CreateAssetMenu(fileName = "Ultraleap UI Skin", menuName ="Ultraleap/UI Skin")]
    public class Skin : ScriptableObject
    {
        [SerializeReference]
        public TMPro.TMP_FontAsset defaultFont;

        [SerializeReference]
        public List<ThemeElement> themes = new List<ThemeElement>();
    }
}