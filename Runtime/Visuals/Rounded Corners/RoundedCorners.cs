using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ultraleap.UI
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class RoundedCorners : MonoBehaviour
    {
        private static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");

        [SerializeField, HideInInspector]
        private Material _material;

        public Image Image { get; private set; }
        public float radius;

        [SerializeField]
        public bool copyHeight = false;

        private float _height = 0f;
        public float Height { get { return _height; } }

        private bool _runTimeGenerated = false;
        private bool _needRefresh = false;

        void OnRectTransformDimensionsChange()
        {
            UpdateImage();
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                _material = null;
            }
            UpdateImage();
        }

        private void OnEnable()
        {
            _needRefresh = true;
            UpdateImage();
        }

        private void OnValidate()
        {
            _needRefresh = true;
            UpdateImage();
        }

        public void UpdateImage()
        {
            Image = GetComponent<Image>();
            if (Image == null)
            {
                return;
            }
            Shader shader = Shader.Find("UI/RoundedCorners/RoundedCorners");
            if (_material == null)
            {
                if ((_runTimeGenerated || !Application.isPlaying) && shader != null)
                {
                    _material = Image.material;
                }
            }
            if (shader != null && (_material == null || _material.shader != shader))
            {
                _material = new Material(shader);
                _material.name = "Rounded Corners";
                if (!_runTimeGenerated && Application.isPlaying)
                {
                    _runTimeGenerated = true;
                }
            }
            if (Image.material != _material)
            {
                Image.material = _material;
            }
            if (shader != null && _material != null && Image.material.shader == shader)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var rect = ((RectTransform)transform).rect;
            _height = rect.height;
            if (Image != null && Image.materialForRendering != null)
            {
                Vector4 props = Image.materialForRendering.GetVector(Props);
                if (_needRefresh)
                {
                    _needRefresh = false;
                    Image.material.SetVector(Props, new Vector4(rect.width, rect.height, copyHeight ? _height : radius, 0));
                    Image.materialForRendering.SetVector(Props, new Vector4(rect.width, rect.height, copyHeight ? _height : radius, 0));
                }
                else if (props.x != rect.width || props.y != rect.height || props.z != (copyHeight ? _height : radius))
                {
                    Image.materialForRendering.SetVector(Props, new Vector4(rect.width, rect.height, copyHeight ? _height : radius, 0));
                }

                Image.SetAllDirty();
            }
        }

        public void SeperateMaterial()
        {
            _material = new Material(_material);
            UpdateImage();
        }
    }
}