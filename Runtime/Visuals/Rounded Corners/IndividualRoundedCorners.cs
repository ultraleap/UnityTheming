using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ultraleap.UI
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class IndividualRoundedCorners : MonoBehaviour
    {

        public Vector4 corners;
        [SerializeField, HideInInspector]
        private Material _material;
        public Image Image { get; private set; }

        // xy - position,
        // zw - halfSize
        [HideInInspector, SerializeField] private Vector4 rect2props;

        private readonly int prop_halfSize = Shader.PropertyToID("_halfSize");
        private readonly int prop_radiuses = Shader.PropertyToID("_r");
        private readonly int prop_rect2props = Shader.PropertyToID("_rect2props");

        // Vector2.right rotated clockwise by 45 degrees
        private static readonly Vector2 wNorm = new Vector2(.7071068f, -.7071068f);
        // Vector2.right rotated counter-clockwise by 45 degrees
        private static readonly Vector2 hNorm = new Vector2(.7071068f, .7071068f);

        [SerializeField]
        public bool copyHeight = false;

        private float _height = 0f;
        public float Height { get { return _height; } }

        private bool _runTimeGenerated = false;

        void OnRectTransformDimensionsChange()
        {
            UpdateImage();
        }

        private void OnValidate()
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

        public void ForceUpdateCorners()
        {
            _material = null;
            UpdateImage();
        }

        private void UpdateImage()
        {
            Image = GetComponent<Image>();
            if (Image == null)
            {
                return;
            }
            Shader shader = Shader.Find("UI/RoundedCorners/IndividualRoundedCorners");
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
            if(shader != null && _material != null && Image.material.shader == shader)
            {
                Refresh();
            }
        }

        private void RecalculateProps(Vector2 size)
        {
            Vector4 copySize = Vector4.one * _height / 2f;
            // Vector that goes from left to right sides of rect2
            var aVec = new Vector2(size.x, -size.y + (copyHeight ? copySize.x : corners.x) + (copyHeight ? copySize.z : corners.z));

            // Project vector aVec to wNorm to get magnitude of rect2 width vector
            var halfWidth = Vector2.Dot(aVec, wNorm) * .5f;
            rect2props.z = halfWidth;


            // Vector that goes from bottom to top sides of rect2
            var bVec = new Vector2(size.x, size.y - (copyHeight ? copySize.w : corners.w) - (copyHeight ? copySize.y : corners.y));

            // Project vector bVec to hNorm to get magnitude of rect2 height vector
            var halfHeight = Vector2.Dot(bVec, hNorm) * .5f;
            rect2props.w = halfHeight;


            // Vector that goes from left to top sides of rect2
            var efVec = new Vector2(size.x - (copyHeight ? copySize.x : corners.x) - (copyHeight ? copySize.y : corners.y), 0);
            // Vector that goes from point E to point G, which is top-left of rect2
            var egVec = hNorm * Vector2.Dot(efVec, hNorm);
            // Position of point E relative to center of coord system
            var ePoint = new Vector2((copyHeight ? copySize.x : corners.x) - (size.x / 2), size.y / 2);
            // Origin of rect2 relative to center of coord system
            // ePoint + egVec == vector to top-left corner of rect2
            // wNorm * halfWidth + hNorm * -halfHeight == vector from top-left corner to center
            var origin = ePoint + egVec + wNorm * halfWidth + hNorm * -halfHeight;
            rect2props.x = origin.x;
            rect2props.y = origin.y;
        }

        private void Refresh()
        {
            var rect = ((RectTransform)transform).rect;
            _height = rect.height;
            RecalculateProps(rect.size);
            if(Image != null && _material != null)
            {
                _material.SetVector(prop_rect2props, rect2props);
                _material.SetVector(prop_halfSize, rect.size * .5f);
                _material.SetVector(prop_radiuses, copyHeight ? Vector4.one * _height / 2f : corners);
                Image.SetMaterialDirty();
            }
        }

        public void SeperateMaterial()
        {
            _material = new Material(_material);
            UpdateImage();
        }
    }
}