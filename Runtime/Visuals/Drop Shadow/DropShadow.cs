using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Ultraleap.UI
{
    [AddComponentMenu("UI/Effects/DropShadow", 14), DisallowMultipleComponent]
    public class DropShadow : BaseMeshEffect
    {
        [SerializeField]
        private Color shadowColor = new Color(0f, 0f, 0f, 0.5f);

        [SerializeField]
        private Vector2 shadowDistance = new Vector2(0f, 0f);

        [SerializeField]
        private bool m_UseGraphicAlpha = true;
        public int iterations = 5;
        public float shadowSpread = 1f;

        protected DropShadow()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            EffectDistance = shadowDistance;
            base.OnValidate();
        }

#endif

        public Color effectColor
        {
            get { return shadowColor; }
            set
            {
                shadowColor = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public float ShadowSpread
        {
            get { return shadowSpread; }
            set
            {
                shadowSpread = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public int Iterations
        {
            get { return iterations; }
            set
            {
                iterations = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public Vector2 EffectDistance
        {
            get { return shadowDistance; }
            set
            {
                shadowDistance = value;

                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public bool useGraphicAlpha
        {
            get { return m_UseGraphicAlpha; }
            set
            {
                m_UseGraphicAlpha = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        void DropShadowEffect(List<UIVertex> verts)
        {
            UIVertex vt;
            int count = verts.Count;

            List<UIVertex> vertsCopy = new List<UIVertex>(verts);
            verts.Clear();

            Rect r = ((RectTransform)transform).rect;
            Vector3 position;

            for (int i = 0; i < iterations; i++)
            {
                float fac = (float)(i+1) / (float)iterations;
                for (int v = 0; v < count; v++)
                {
                    vt = vertsCopy[v];
                    position = vt.position;
                    position.x *= 1 + ((r.width + (shadowSpread*100)) / r.width) * fac * 0.01f;
                    position.y *= 1 + ((r.height + (shadowSpread*100)) / r.height) * fac * 0.01f;
                    position.x += shadowDistance.x * fac;
                    position.y += shadowDistance.y * fac;
                    vt.position = position;
                    Color32 color = shadowColor;
                    color.a = (byte)((float)color.a / (float)iterations);
                    vt.color = color;
                    verts.Add(vt);
                }
            }

            for (int i = 0; i < vertsCopy.Count; i++)
            {
                verts.Add(vertsCopy[i]);
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            List<UIVertex> output = new List<UIVertex>();
            vh.GetUIVertexStream(output);

            DropShadowEffect(output);

            vh.Clear();
            vh.AddUIVertexTriangleStream(output);
        }
    }
}