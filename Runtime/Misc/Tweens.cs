using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ultraleap.UI
{
    // Copied from the Unity UI https://github.com/Unity-Technologies/uGUI/blob/5ab4c0fee7cd5b3267672d877ec4051da525913c/UnityEngine.UI/UI/Animation/CoroutineTween.cs
    public interface ITweenValue
    {
        void TweenValue(float floatPercentage);
        bool ignoreTimeScale { get; }
        float duration { get; }
        bool ValidTarget();
    }

    public struct ColorTween : ITweenValue
    {
        public enum ColorTweenMode
        {
            All,
            RGB,
            Alpha
        }

        public class ColorTweenCallback : UnityEvent<Color> { }

        private ColorTweenCallback m_Target;
        private Color m_StartColor;
        private Color m_TargetColor;
        private ColorTweenMode m_TweenMode;

        private float m_Duration;
        private bool m_IgnoreTimeScale;

        public Color startColor
        {
            get { return m_StartColor; }
            set { m_StartColor = value; }
        }

        public Color targetColor
        {
            get { return m_TargetColor; }
            set { m_TargetColor = value; }
        }

        public ColorTweenMode tweenMode
        {
            get { return m_TweenMode; }
            set { m_TweenMode = value; }
        }

        public float duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public bool ignoreTimeScale
        {
            get { return m_IgnoreTimeScale; }
            set { m_IgnoreTimeScale = value; }
        }

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;

            var newColor = Color.Lerp(m_StartColor, m_TargetColor, floatPercentage);

            if (m_TweenMode == ColorTweenMode.Alpha)
            {
                newColor.r = m_StartColor.r;
                newColor.g = m_StartColor.g;
                newColor.b = m_StartColor.b;
            }
            else if (m_TweenMode == ColorTweenMode.RGB)
            {
                newColor.a = m_StartColor.a;
            }
            m_Target.Invoke(newColor);
        }

        public void AddOnChangedCallback(UnityAction<Color> callback)
        {
            if (m_Target == null)
                m_Target = new ColorTweenCallback();

            m_Target.AddListener(callback);
        }

        public bool GetIgnoreTimescale()
        {
            return m_IgnoreTimeScale;
        }

        public float GetDuration()
        {
            return m_Duration;
        }

        public bool ValidTarget()
        {
            return m_Target != null;
        }
    }

    public struct TwinColorTween : ITweenValue
    {
        public enum ColorTweenMode
        {
            All,
            RGB,
            Alpha
        }

        public class TwinColorTweenCallback : UnityEvent<Color,Color> { }

        private TwinColorTweenCallback m_Target;
        private Color m_StartColorA, m_StartColorB;
        private Color m_TargetColorA, m_TargetColorB;
        private ColorTweenMode m_TweenMode;

        private float m_Duration;
        private bool m_IgnoreTimeScale;

        public Color startColorA
        {
            get { return m_StartColorA; }
            set { m_StartColorA = value; }
        }

        public Color targetColorA
        {
            get { return m_TargetColorA; }
            set { m_TargetColorA = value; }
        }

        public Color startColorB
        {
            get { return m_StartColorB; }
            set { m_StartColorB = value; }
        }

        public Color targetColorB
        {
            get { return m_TargetColorB; }
            set { m_TargetColorB = value; }
        }

        public ColorTweenMode tweenMode
        {
            get { return m_TweenMode; }
            set { m_TweenMode = value; }
        }

        public float duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public bool ignoreTimeScale
        {
            get { return m_IgnoreTimeScale; }
            set { m_IgnoreTimeScale = value; }
        }

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;

            m_Target.Invoke(
                LerpColor(m_StartColorA,m_TargetColorA,floatPercentage),
                LerpColor(m_StartColorB,m_TargetColorB,floatPercentage)
                );
        }

        private Color LerpColor(Color start, Color target, float percent)
        {
            var newColor = Color.Lerp(start, target, percent);
            if (m_TweenMode == ColorTweenMode.Alpha)
            {
                newColor.r = start.r;
                newColor.g = start.g;
                newColor.b = start.b;
            }
            else if (m_TweenMode == ColorTweenMode.RGB)
            {
                newColor.a = start.a;
            }
            return newColor;
        }

        public void AddOnChangedCallback(UnityAction<Color,Color> callback)
        {
            if (m_Target == null)
                m_Target = new TwinColorTweenCallback();

            m_Target.AddListener(callback);
        }

        public bool GetIgnoreTimescale()
        {
            return m_IgnoreTimeScale;
        }

        public float GetDuration()
        {
            return m_Duration;
        }

        public bool ValidTarget()
        {
            return m_Target != null;
        }
    }

    public struct FloatTween : ITweenValue
    {
        public class FloatTweenCallback : UnityEvent<float> { }

        private FloatTweenCallback m_Target;
        private float m_StartValue;
        private float m_TargetValue;

        private float m_Duration;
        private bool m_IgnoreTimeScale;

        public float startValue
        {
            get { return m_StartValue; }
            set { m_StartValue = value; }
        }

        public float targetValue
        {
            get { return m_TargetValue; }
            set { m_TargetValue = value; }
        }

        public float duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public bool ignoreTimeScale
        {
            get { return m_IgnoreTimeScale; }
            set { m_IgnoreTimeScale = value; }
        }

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;

            var newValue = Mathf.Lerp(m_StartValue, m_TargetValue, floatPercentage);
            m_Target.Invoke(newValue);
        }

        public void AddOnChangedCallback(UnityAction<float> callback)
        {
            if (m_Target == null)
                m_Target = new FloatTweenCallback();

            m_Target.AddListener(callback);
        }

        public bool GetIgnoreTimescale()
        {
            return m_IgnoreTimeScale;
        }

        public float GetDuration()
        {
            return m_Duration;
        }

        public bool ValidTarget()
        {
            return m_Target != null;
        }
    }

    public struct VectorTween : ITweenValue
    {
        public class VectorTweenCallback : UnityEvent<Vector3> { }

        private VectorTweenCallback m_Target;
        private Vector3 m_StartValue;
        private Vector3 m_TargetValue;

        private float m_Duration;
        private bool m_IgnoreTimeScale;

        public Vector3 startValue
        {
            get { return m_StartValue; }
            set { m_StartValue = value; }
        }

        public Vector3 targetValue
        {
            get { return m_TargetValue; }
            set { m_TargetValue = value; }
        }

        public float duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public bool ignoreTimeScale
        {
            get { return m_IgnoreTimeScale; }
            set { m_IgnoreTimeScale = value; }
        }

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;

            var newValue = Vector3.Lerp(m_StartValue, m_TargetValue, floatPercentage);
            m_Target.Invoke(newValue);
        }

        public void AddOnChangedCallback(UnityAction<Vector3> callback)
        {
            if (m_Target == null)
                m_Target = new VectorTweenCallback();

            m_Target.AddListener(callback);
        }

        public bool GetIgnoreTimescale()
        {
            return m_IgnoreTimeScale;
        }

        public float GetDuration()
        {
            return m_Duration;
        }

        public bool ValidTarget()
        {
            return m_Target != null;
        }
    }

    public class TweenRunner<T> where T : struct, ITweenValue
    {
        protected MonoBehaviour m_CoroutineContainer;
        protected IEnumerator m_Tween;

        // utility function for starting the tween
        private static IEnumerator Start(T tweenInfo)
        {
            if (!tweenInfo.ValidTarget())
                yield break;

            var elapsedTime = 0.0f;
            while (elapsedTime < tweenInfo.duration)
            {
                elapsedTime += tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                var percentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
                tweenInfo.TweenValue(percentage);
                yield return null;
            }
            tweenInfo.TweenValue(1.0f);
        }

        public void Init(MonoBehaviour coroutineContainer)
        {
            if (coroutineContainer == m_CoroutineContainer)
                return;

            m_CoroutineContainer = coroutineContainer;
        }

        public void StartTween(T info)
        {
            if (m_CoroutineContainer == null)
            {
                Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
                return;
            }

            StopTween();

            if (!m_CoroutineContainer.gameObject.activeInHierarchy)
            {
                info.TweenValue(1.0f);
                return;
            }

            m_Tween = Start(info);
            m_CoroutineContainer.StartCoroutine(m_Tween);
        }

        public void StopTween()
        {
            if (m_Tween != null)
            {
                m_CoroutineContainer.StopCoroutine(m_Tween);
                m_Tween = null;
            }
        }
    }

}