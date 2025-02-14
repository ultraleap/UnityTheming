using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultraleap.UI
{
    [System.Serializable]
    public class GraphicState : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private bool _serialized = false;

        public Color basic = Color.black, active = Color.black;
        public float scale = 1f, activeScale = 1f;

        public void OnAfterDeserialize()
        {
            if (!_serialized)
            {
                _serialized = true;
                Reset();
            }
        }

        protected virtual void Reset()
        {
            basic = Color.black;
            active = Color.black;
            scale = 1f;
            activeScale = 1f;
        }

        public void OnBeforeSerialize()
        {

        }
    }

    [System.Serializable]
    public class GradientState : GraphicState
    {
        public Color basicSecondary = Color.black, activeSecondary = Color.black;

        protected override void Reset()
        {
            base.Reset();
            basicSecondary = Color.black;
            activeSecondary = Color.black;
        }
    }

    [System.Serializable]
    public class GraphicItem
    {
        public string name = "";
        [HideInInspector]
        public Vector4 roundedCornersRadius = Vector4.zero;
        [HideInInspector]
        public bool useRoundedCorners = false, individualCorners = false, copyHeightRound = false;

        [HideInInspector]
        public float transitionTime = 0.1f;

        public GraphicState normal = new GraphicState(), hover = new GraphicState(), pressed = new GraphicState(), disabled = new GraphicState();
        public GraphicState GetState(string state)
        {
            switch (state)
            {
                case "Normal":
                case "Selected":
                    return normal;
                case "Highlighted":
                    return hover;
                case "Pressed":
                    return pressed;
                case "Disabled":
                    return disabled;
            }
            return null;
        }
    }

    [System.Serializable]
    public class GradientItem
    {
        public string name = "";
        [HideInInspector]
        public Vector4 roundedCornersRadius = Vector4.zero;
        [HideInInspector]
        public bool useRoundedCorners = false, individualCorners = false, copyHeightRound = false;

        [HideInInspector]
        public float transitionTime = 0.1f;

        [Range(-180, 180), HideInInspector]
        public float gradientAngle = 0;

        public GradientState normal, hover, pressed, disabled;

        public GradientState GetState(string state)
        {
            switch (state)
            {
                case "Normal":
                case "Selected":
                    return normal;
                case "Highlighted":
                    return hover;
                case "Pressed":
                    return pressed;
                case "Disabled":
                    return disabled;
            }
            return null;
        }
    }

    [System.Serializable]
    public class TextState : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private bool _serialized = false;

        public Color textColorBasic = Color.white;
        public Color textColorActive = Color.white;

        /// <summary>
        /// X standard font size, Y min font size, Z max font size
        /// </summary>
        public Vector3 fontSizes = new Vector3(16, 12, 16);

        public void OnAfterDeserialize()
        {
            if (!_serialized)
            {
                _serialized = true;
                Reset();
            }
        }

        public void OnBeforeSerialize()
        {
        }

        protected virtual void Reset()
        {
            textColorBasic = Color.black;
            textColorActive = Color.black;
            fontSizes = new Vector3(16, 12, 16);
        }
    }

    [System.Serializable]
    public class TextItem : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private bool _serialized = false;


        public bool ignoreHover = true, ignorePressed = true, ignoreDisabled = true;
        public TextState normal, hover, pressed, disabled;

        public bool useDefaultFont = true;
        public TMPro.TMP_FontAsset font;
        public bool changeFontSizes = false;
        public bool useAutoFontSizes = false;

        public TMPro.FontStyles fontStyles;
        public bool changeTextAlignment = true;
        public TMPro.TextAlignmentOptions textAlignment;

        public float transitionTime = 0.1f;

        public TextState GetState(string state)
        {
            switch (state)
            {
                case "Normal":
                case "Selected":
                    return normal;
                case "Highlighted":
                    return ignoreHover ? normal : hover;
                case "Pressed":
                    return ignorePressed ? GetState("Highlighted") : pressed;
                case "Disabled":
                    return ignoreDisabled ? GetState("Pressed") : disabled;
            }
            return null;
        }

        public void OnAfterDeserialize()
        {
            if (!_serialized)
            {
                _serialized = true;

                useDefaultFont = true;
                changeFontSizes = false;
                changeTextAlignment = true;
                useAutoFontSizes = false;
                ignoreHover = true;
                ignorePressed = true;
                ignoreDisabled = true;
                useDefaultFont = true;
            }
        }

        public void OnBeforeSerialize()
        {

        }
    }
}