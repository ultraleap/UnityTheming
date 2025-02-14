using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ultraleap.UI
{
    public class UIManager : MonoBehaviour
    {
        public Skin skin;

        [SerializeField, HideInInspector]
        private List<string> _skinThemes = new List<string>();
        public List<string> skinThemes
        {
            get
            {
                if (skin == null)
                    return null;

                return _skinThemes;
            }
        }

        [SerializeField, HideInInspector]
        private List<MultiImageTarget> graphics;

        private void OnEnable()
        {
            AssignTheme();
        }

        public void AssignTheme()
        {
            graphics = FindObjectsOfType<MultiImageTarget>(true).ToList();
            _skinThemes.Clear();
            if (skin != null)
            {
                for (int j = 0; j < graphics.Count; j++)
                {
                    for (int i = 0; i < skin.themes.Count; i++)
                    {
                        if (graphics[j].themeName == skin.themes[i].elementName)
                        {
                            graphics[j].ApplyTheme(skin.themes[i]);
                        }
                    }
                }
                RefreshThemes();
            }
        }

        public bool TryGetTheme(string themeName, out ThemeElement theme)
        {
            for (int i = 0; i < skin.themes.Count; i++)
            {
                if (themeName == skin.themes[i].elementName)
                {
                    theme = skin.themes[i];
                    return true;
                }
            }
            theme = null;
            return false;
        }

        public void ApplyThemeChange(ThemeElement element)
        {
            graphics = FindObjectsOfType<MultiImageTarget>(true).ToList();
            _skinThemes.Clear();
            if (skin != null)
            {
                if (skin.themes.Contains(element))
                {
                    for (int i = 0; i < graphics.Count; i++)
                    {
                        if (graphics[i].themeName == element.elementName)
                        {
                            graphics[i].ApplyTheme();
                        }
                    }
                }
                RefreshThemes();
            }
        }

        public void RefreshThemes()
        {
            _skinThemes.Clear();
            if (skin != null)
            {
                _skinThemes = skin.themes.Select(x => x.elementName).ToList();
            }
        }

        public List<MultiImageTarget> GetAllUsingTheme(ThemeElement element)
        {
            if (skin == null)
            {
                return null;
            }
            graphics = FindObjectsOfType<MultiImageTarget>(true).ToList();
            List<MultiImageTarget> targets = new List<MultiImageTarget>();
            if (skin.themes.Contains(element))
            {
                targets = graphics.Where(x => x.themeName == element.elementName).ToList();
            }
            return targets;
        }
    }
}