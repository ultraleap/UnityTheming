using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ultraleap.UI
{
    [DisallowMultipleComponent]
    public class MultiImageTarget : MonoBehaviour
    {
        private string state = "Normal";
        private bool active = false;

        [SerializeField, HideInInspector] private ThemeElement themeObject = null;
        public string themeName { get { return themeObject ? themeObject.elementName : ""; } }

        [SerializeField] private List<Graphic> targetGraphics;

        [SerializeField] private List<UIGradient> targetGradients = new List<UIGradient>();

        [SerializeField] private List<TextMeshProUGUI> targetText = new List<TextMeshProUGUI>();

        private TweenRunner<ColorTween>[] colorTweens;
        private TweenRunner<ColorTween>[] textTweens;
        private TweenRunner<VectorTween>[] textSizeTweens;
        private TweenRunner<TwinColorTween>[] gradientTweens;
        private TweenRunner<FloatTween>[] scaleTweens;

        private UIManager _manager;

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private bool _noGradients, _noGraphics;
#endif

        public bool ignoreFont = false;

        private void Awake()
        {
            if (colorTweens == null)
            {
                // Need one tween per graphic, text, and gradients
                colorTweens = new TweenRunner<ColorTween>[targetGraphics.Count];
                gradientTweens = new TweenRunner<TwinColorTween>[targetGradients.Count];
                textTweens = new TweenRunner<ColorTween>[targetText.Count];
                textSizeTweens = new TweenRunner<VectorTween>[targetText.Count];
                scaleTweens = new TweenRunner<FloatTween>[targetGraphics.Count + targetGradients.Count];
            }
            for (int i = 0; i < colorTweens.Length; i++)
            {
                colorTweens[i] = new TweenRunner<ColorTween>();
                colorTweens[i].Init(this);
            }
            for (int i = 0; i < gradientTweens.Length; i++)
            {
                gradientTweens[i] = new TweenRunner<TwinColorTween>();
                gradientTweens[i].Init(this);
            }
            for (int i = 0; i < textTweens.Length; i++)
            {
                textTweens[i] = new TweenRunner<ColorTween>();
                textTweens[i].Init(this);
            }
            for (int i = 0; i < textSizeTweens.Length; i++)
            {
                textSizeTweens[i] = new TweenRunner<VectorTween>();
                textSizeTweens[i].Init(this);
            }
            for (int i = 0; i < scaleTweens.Length; i++)
            {
                scaleTweens[i] = new TweenRunner<FloatTween>();
                scaleTweens[i].Init(this);
            }
            state = null;
            // Apply the current theme at start so we always ensure consistency
            TryGetAndApplyTheme();
        }

        private void OnValidate()
        {
            TryGetAndApplyTheme();
        }

        private void OnEnable()
        {
            TryGetAndApplyTheme();
        }

        // Editor helper function for finding graphical elements
        public void FindTargets(bool firstOnly)
        {
            targetGraphics = GetComponentsInChildren<Graphic>(true).ToList();
            targetGradients.Clear();
            targetText.Clear();
            for (int i = 0; i < targetGraphics.Count; i++)
            {
                UIGradient g;
                if ((!firstOnly || i == 0) && targetGraphics[i].TryGetComponent(out g))
                {
                    targetGradients.Add(g);
                    targetGraphics.RemoveAt(i);
                    i--;
                    continue;
                }
                TextMeshProUGUI t;
                if (targetGraphics[i].TryGetComponent(out t))
                {
                    targetText.Add(t);
                    targetGraphics.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            if (firstOnly)
            {
                if (targetGraphics.Count > 1)
                {
                    targetGraphics.RemoveRange(1, targetGraphics.Count - 1);
                }
                if (targetGradients.Count > 1)
                {
                    targetGradients.RemoveRange(1, targetGradients.Count - 1);
                }
                if (targetText.Count > 1)
                {
                    targetText.RemoveRange(1, targetText.Count - 1);
                }
            }
            ApplyTheme();
        }

        private void TryGetAndApplyTheme()
        {
            ThemeElement newTheme;
            if (_manager == null)
            {
                _manager = FindAnyObjectByType<UIManager>();
                if (_manager == null)
                {
                    return;
                }
            }

            if (_manager.TryGetTheme(themeName, out newTheme))
            {
                themeObject = newTheme;
                ApplyTheme();
            }
            else
            {
                Debug.Log("No Theme Element found with name " + themeName);
            }
        }

        public void ApplyTheme()
        {
            if (themeObject != null && gameObject.scene.name != null)
            {
                _manager = FindAnyObjectByType<UIManager>();
                ApplyTheme(themeObject);
            }
        }

        public void ApplyTheme(ThemeElement theme)
        {
            themeObject = theme;
            if (theme != null && gameObject.scene.name != null)
            {
                SetCorners();
                SetState("Normal", false);
            }
        }

        public void SetState(string state)
        {
            SetState(state, false);
        }

        // Sets rounded corners from theme
        private void SetCorners()
        {
            for (int i = 0; i < targetGradients.Count; i++)
            {
                if (i >= themeObject.gradientThemes.Count)
                {
                    break;
                }

                if (targetGradients[i] == null)
                {
                    continue;
                }
                GradientItem gi = themeObject.gradientThemes[i];
                if (gi != null)
                {
                    ApplyRoundedCornersSettings(targetGradients[i].gameObject, gi.useRoundedCorners, gi.individualCorners, gi.copyHeightRound, gi.roundedCornersRadius);
                }
            }
            for (int i = 0; i < targetGraphics.Count; i++)
            {
                if (i >= themeObject.graphicThemes.Count)
                {
                    break;
                }

                if (targetGraphics[i] == null)
                {
                    continue;
                }
                GraphicItem gi = themeObject.graphicThemes[i];
                if (gi != null)
                {
                    ApplyRoundedCornersSettings(targetGraphics[i].gameObject, gi.useRoundedCorners, gi.individualCorners, gi.copyHeightRound, gi.roundedCornersRadius);
                }
            }
        }

        private void ApplyRoundedCornersSettings(GameObject go, bool useCorners, bool individual, bool copyHeight, Vector4 radius)
        {
            if (!useCorners)
            {
                RemoveRoundedCorners(go);
                RemoveIndividualRoundedCorners(go);
                return;
            }

            if (individual)
            {
                RemoveRoundedCorners(go);
                RemoveIndividualRoundedCorners(go, 1);

#if UNITY_EDITOR
                // Re-use the parts from the prefab instead
                if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                {
                    var removedItems = PrefabUtility.GetRemovedComponents(go);
                    foreach (var item in removedItems)
                    {
                        if (item.containingInstanceGameObject == go && item.assetComponent.GetType() == typeof(IndividualRoundedCorners))
                        {
                            item.Revert();
                            break;
                        }
                    }
                }
#endif
                IndividualRoundedCorners individualRoundedCorners = null;
                if (!go.TryGetComponent(out individualRoundedCorners))
                {
                    individualRoundedCorners = go.AddComponent<IndividualRoundedCorners>();
                }

                individualRoundedCorners.copyHeight = copyHeight;
                if (!individualRoundedCorners.copyHeight)
                {
                    individualRoundedCorners.corners = radius;
                }
                individualRoundedCorners.ForceUpdateCorners();
            }
            else
            {
                RemoveIndividualRoundedCorners(go);
                RemoveRoundedCorners(go, 1);
#if UNITY_EDITOR
                // Re-use the parts from the prefab instead
                if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                {
                    var removedItems = PrefabUtility.GetRemovedComponents(go);
                    foreach (var item in removedItems)
                    {
                        if (item.containingInstanceGameObject == go && item.assetComponent.GetType() == typeof(RoundedCorners))
                        {
                            item.Revert();
                            break;
                        }
                    }
                }
#endif
                RoundedCorners roundedCorners;
                if (!go.TryGetComponent(out roundedCorners))
                {
                    roundedCorners = go.AddComponent<RoundedCorners>();
                }
                roundedCorners.copyHeight = copyHeight;
                if (!roundedCorners.copyHeight)
                {
                    roundedCorners.radius = radius[0];
                }
                roundedCorners.UpdateImage();
            }
        }

        // Offset allows us to remove duplicates easily without needing a new function
        private void RemoveRoundedCorners(GameObject go, int offset)
        {
            if (go.TryGetComponent<Image>(out var img))
            {
                img.material = null;
            }
            if (go.TryGetComponent<RawImage>(out var rawImg))
            {
                rawImg.material = null;
            }
            RoundedCorners[] roundedCorners = go.GetComponents<RoundedCorners>();

            for (int i = offset; i < roundedCorners.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(roundedCorners[i]);
                }
                else
                {
#if UNITY_EDITOR
                    if (roundedCorners[i].gameObject.activeInHierarchy)
                    {
                        StartCoroutine(DeleteComponent(roundedCorners[i]));
                    }
                    else if (go.scene.name != null)
                    {
                        UIManager uim = FindObjectOfType<UIManager>();
                        if (uim != null)
                        {
                            uim.StartCoroutine(DeleteComponent(roundedCorners[i]));
                        }
                    }
#endif
                }
            }
        }

        private void RemoveRoundedCorners(GameObject go)
        {
            RemoveRoundedCorners(go, 0);
        }

        // Offset allows us to remove duplicates easily without needing a new function
        private void RemoveIndividualRoundedCorners(GameObject go, int offset)
        {
            if (go.TryGetComponent<Image>(out var img))
            {
                img.material = null;
            }
            if (go.TryGetComponent<RawImage>(out var rawImg))
            {
                rawImg.material = null;
            }
            IndividualRoundedCorners[] individualRoundedCorners = go.GetComponents<IndividualRoundedCorners>();
            if (individualRoundedCorners.Length > 0)
            {
                for (int i = offset; i < individualRoundedCorners.Length; i++)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(individualRoundedCorners[i]);
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (individualRoundedCorners[i].gameObject.activeInHierarchy)
                        {
                            StartCoroutine(DeleteComponent(individualRoundedCorners[i]));
                        }
                        else if (go.scene.name != null)
                        {
                            UIManager uim = FindObjectOfType<UIManager>();
                            if (uim != null)
                            {
                                uim.StartCoroutine(DeleteComponent(individualRoundedCorners[i]));
                            }
                        }
#endif
                    }
                }
            }
        }

        private void RemoveIndividualRoundedCorners(GameObject go)
        {
            RemoveIndividualRoundedCorners(go, 0);
        }

#if UNITY_EDITOR
        private IEnumerator DeleteComponent(Component comp)
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate(comp, true);
        }
#endif

        public void SetState(string state, bool active, bool instant = false)
        {
            // Need to find the manager
            if (_manager == null)
            {
                _manager = FindAnyObjectByType<UIManager>();
            }

            // If there's no theme or manager then skip
            if (_manager == null || themeObject == null)
            {
                return;
            }

            // We add the isPlaying to prevent too many runtime adjustments but always want to update in edit mode
            if (!instant && this.state == state && this.active == active && Application.isPlaying)
            {
                return;
            }

            this.state = state;
            this.active = active;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _noGraphics = themeObject.graphicThemes.Count > 0 && targetGraphics.Count == 0;
                _noGradients = themeObject.gradientThemes.Count > 0 && targetGradients.Count == 0;
            }
#endif

            for (int i = 0; i < targetGradients.Count; i++)
            {
                if (i >= themeObject.gradientThemes.Count)
                {
                    break;
                }
                if (targetGradients[i] == null)
                {
                    continue;
                }

                targetGradients[i].m_angle = themeObject.gradientThemes[i].gradientAngle;

                GradientState gs = themeObject.gradientThemes[i].GetState(state);
                if (gs != null)
                {
                    if (instant)
                    {
                        gradientTweens[i].StopTween();
                        scaleTweens[targetGraphics.Count + i].StopTween();
                    }
                    // Tweens are null in edit mode, makes it instantly swap where needed
                    if (themeObject.gradientThemes[i].transitionTime == 0 || gradientTweens == null || i >= gradientTweens.Length || instant)
                    {
                        targetGradients[i].m_color1 = active ? gs.active : gs.basic;
                        targetGradients[i].m_color2 = active ? gs.activeSecondary : gs.basicSecondary;
                        targetGradients[i].Refresh();
                        targetGradients[i].transform.localScale = Vector3.one * (active ? gs.activeScale : gs.scale);
                    }
                    else
                    {
                        CrossFadeGradient(
                            targetGradients[i],
                            gradientTweens[i],
                            scaleTweens[targetGraphics.Count + i],
                            active ? gs.active : gs.basic,
                            active ? gs.activeSecondary : gs.basicSecondary,
                            active ? gs.activeScale : gs.scale,
                            themeObject.gradientThemes[i].transitionTime);
                    }
                }
            }

            for (int i = 0; i < targetGraphics.Count; i++)
            {
                if (i >= themeObject.graphicThemes.Count)
                {
                    break;
                }
                if (targetGraphics[i] == null)
                {
                    continue;
                }
                GraphicState gs = themeObject.graphicThemes[i].GetState(state);
                if (gs != null)
                {
                    if (instant)
                    {
                        colorTweens[i].StopTween();
                        scaleTweens[i].StopTween();
                    }
                    // Tweens are null in edit mode, makes it instantly swap where needed
                    if (themeObject.graphicThemes[i].transitionTime == 0 || colorTweens == null || i >= colorTweens.Length || instant)
                    {
                        targetGraphics[i].color = active ? gs.active : gs.basic;
                        targetGraphics[i].transform.localScale = Vector3.one * (active ? gs.activeScale : gs.scale);
                    }
                    else
                    {
                        CrossFadeGraphic(
                            targetGraphics[i],
                            colorTweens[i],
                            scaleTweens[i],
                            active ? gs.active : gs.basic,
                            active ? gs.activeScale : gs.scale,
                            themeObject.graphicThemes[i].transitionTime);
                    }
                }
            }

            for (int i = 0; i < targetText.Count; i++)
            {
                if (i >= themeObject.textThemes.Count)
                {
                    break;
                }
                if (targetText[i] == null)
                {
                    continue;
                }
                TextState ts = themeObject.textThemes[i].GetState(state);

                if (!ignoreFont && ((themeObject.textThemes[i].useDefaultFont && _manager.skin.defaultFont != null) || (!themeObject.textThemes[i].useDefaultFont && themeObject.textThemes[i].font != null)))
                {
                    targetText[i].font = themeObject.textThemes[i].useDefaultFont ? _manager.skin.defaultFont : themeObject.textThemes[i].font;
                }

                if (themeObject.textThemes[i].changeFontSizes)
                {
                    targetText[i].enableAutoSizing = themeObject.textThemes[i].useAutoFontSizes;
                }

                targetText[i].fontStyle = themeObject.textThemes[i].fontStyles;

                if (themeObject.textThemes[i].changeTextAlignment)
                {
                    targetText[i].alignment = themeObject.textThemes[i].textAlignment;
                }

                if (ts != null)
                {
                    // Tweens are null in edit mode, makes it instantly swap where needed
                    if (themeObject.textThemes[i].transitionTime == 0 || textTweens == null || i >= textTweens.Length)
                    {
                        targetText[i].color = this.active ? ts.textColorActive : ts.textColorBasic;
                        if (themeObject.textThemes[i].changeFontSizes)
                        {
                            if (themeObject.textThemes[i].useAutoFontSizes)
                            {
                                targetText[i].fontSizeMin = ts.fontSizes.y;
                                targetText[i].fontSizeMax = ts.fontSizes.z;
                            }
                            else
                            {
                                targetText[i].fontSize = ts.fontSizes.x;
                            }
                        }
                    }
                    else
                    {
                        CrossFadeText(
                            targetText[i],
                            textTweens[i],
                            textSizeTweens[i],
                            this.active ? ts.textColorActive : ts.textColorBasic,
                            themeObject.textThemes[i].changeFontSizes,
                            themeObject.textThemes[i].useAutoFontSizes,
                            ts.fontSizes,
                            themeObject.textThemes[i].transitionTime);
                    }
                }
            }
        }

        private void CrossFadeGraphic(Graphic graphic, TweenRunner<ColorTween> tween, TweenRunner<FloatTween> scaleTween, Color targetColor, float targetScale, float duration)
        {
            if (graphic.canvasRenderer == null)
                return;

            if (graphic.transform.localScale.x != targetScale)
            {
                var scaleTweenSettings = new FloatTween { duration = duration, startValue = graphic.transform.localScale.x, targetValue = targetScale };
                scaleTweenSettings.AddOnChangedCallback((value) => { graphic.transform.localScale = Vector3.one * value; });
                scaleTweenSettings.ignoreTimeScale = true;
                scaleTween.StartTween(scaleTweenSettings);
            }

            Color currentColor = graphic.color;

            if (TwoColoursAreClose(currentColor, targetColor))
            {
                tween.StopTween();
                return;
            }

            var colorTween = new ColorTween { duration = duration, startColor = currentColor, targetColor = targetColor };
            colorTween.AddOnChangedCallback((col) => { graphic.color = col; });
            colorTween.ignoreTimeScale = true;
            colorTween.tweenMode = ColorTween.ColorTweenMode.All;
            tween.StartTween(colorTween);
        }

        private void CrossFadeGradient(UIGradient gradient, TweenRunner<TwinColorTween> tween, TweenRunner<FloatTween> scaleTween, Color targetTop, Color targetBottom, float targetScale, float duration)
        {
            if (gradient == null)
                return;

            if (gradient.transform.localScale.x != targetScale)
            {
                var scaleTweenSettings = new FloatTween { duration = duration, startValue = gradient.transform.localScale.x, targetValue = targetScale };
                scaleTweenSettings.AddOnChangedCallback((value) => { gradient.transform.localScale = Vector3.one * value; });
                scaleTweenSettings.ignoreTimeScale = true;
                scaleTween.StartTween(scaleTweenSettings);
            }

            Color currentTop = gradient.m_color1;
            Color currentBottom = gradient.m_color2;

            if (TwoColoursAreClose(currentTop, targetTop) && TwoColoursAreClose(currentBottom, targetBottom))
            {
                tween.StopTween();
                return;
            }

            var gradientTween = new TwinColorTween { duration = duration, startColorA = gradient.m_color1, targetColorA = targetTop, startColorB = gradient.m_color2, targetColorB = targetBottom };
            gradientTween.AddOnChangedCallback((col, col2) => { gradient.m_color1 = col; gradient.m_color2 = col2; gradient.Refresh(); });
            gradientTween.ignoreTimeScale = true;
            gradientTween.tweenMode = TwinColorTween.ColorTweenMode.All;
            tween.StartTween(gradientTween);
        }

        private void CrossFadeText(TextMeshProUGUI text, TweenRunner<ColorTween> tween, TweenRunner<VectorTween> sizeTween, Color targetColor,
            bool changeFontSize, bool autoFontSize, Vector3 targetSizes, float duration)
        {
            if (text.canvasRenderer == null)
                return;

            Color currentColor = text.color;

            if (changeFontSize)
            {
                text.enableAutoSizing = autoFontSize;

                Vector3 currentSizes = new Vector3(autoFontSize ? 0 : text.fontSize, autoFontSize ? text.fontSizeMin : 0, autoFontSize ? text.fontSizeMax : 0);
                targetSizes.x = autoFontSize ? 0 : targetSizes.x;
                targetSizes.y = autoFontSize ? targetSizes.y : 0;
                targetSizes.z = autoFontSize ? targetSizes.z : 0;

                var sizeTweenSettings = new VectorTween { duration = duration, startValue = currentSizes, targetValue = targetSizes };
                sizeTweenSettings.AddOnChangedCallback((vector) => { text.fontSize = vector.x; text.fontSizeMin = vector.y; text.fontSizeMax = vector.z; });
                sizeTweenSettings.ignoreTimeScale = true;
                sizeTween.StartTween(sizeTweenSettings);
            }

            if (TwoColoursAreClose(currentColor, targetColor))
            {
                tween.StopTween();
                return;
            }

            var colorTween = new ColorTween { duration = duration, startColor = currentColor, targetColor = targetColor };
            colorTween.AddOnChangedCallback((col) => { text.color = col; });
            colorTween.ignoreTimeScale = true;
            colorTween.tweenMode = ColorTween.ColorTweenMode.All;

            tween.StartTween(colorTween);
        }

        private bool TwoColoursAreClose(Color a, Color b)
        {
            return Mathf.Approximately(a.r, b.r) && Mathf.Approximately(a.g, b.g) && Mathf.Approximately(a.b, b.b) && Mathf.Approximately(a.a, b.a);
        }
    }
}