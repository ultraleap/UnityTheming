using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace Ultraleap.UI
{
    [RequireComponent(typeof(MultiImageTarget))]
    public class MultiImageSlideToggle : Toggle
    {
        [SerializeField, HideInInspector] private MultiImageTarget _multiImageTarget;

        private TweenRunner<FloatTween> _toggleTween;
        [SerializeField] private RectTransform _handleWrapper,_handle;
        [SerializeField] private float _transitionTime = 0.1f;
        private float _startPos;

        protected override void Start()
        {
            base.Start();
            _toggleTween = new TweenRunner<FloatTween>();
            _toggleTween.Init(this);
            RecalcPositions(true);
            OnValueChanged(isOn, true);

            onValueChanged.AddListener(OnValueChanged);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            //get the component, if it could not get the graphics, return here
            if (!GetGraphics())
                return;
            _multiImageTarget?.SetState(state.ToString(), isOn);
        }

        public void OnValueChanged(bool value)
        {
            OnValueChanged(value, false, _transitionTime);
        }

        public void OnValueChanged(bool value, bool silent)
        {
            OnValueChanged(value, silent, _transitionTime);
        }

        public void OnValueChanged(bool value, bool silent, float transitionTime)
        {
            if (_handle != null)
            {
                RecalcPositions(false);
                SlideTween(_handle, _toggleTween, value ? _startPos * -1 : _startPos, transitionTime);
                if (Application.isPlaying)
                {
                    _multiImageTarget?.SetState(silent ? currentSelectionState.ToString() : SelectionState.Highlighted.ToString(), value);
                }
            }
        }

        private void SlideTween(RectTransform handle, TweenRunner<FloatTween> tween, float targetPos, float duration)
        {
            if (handle == null)
                return;

            float startX = handle.anchoredPosition.x;

            if (startX == targetPos)
            {
                tween.StopTween();
                return;
            }

            var slideTween = new FloatTween { duration = duration, startValue = handle.anchoredPosition.x, targetValue = targetPos };
            slideTween.AddOnChangedCallback((pos) => 
                { 
                    handle.anchoredPosition = new Vector2(pos, handle.anchoredPosition.y);
                    OnTweenEnd();
                }
            );
            slideTween.ignoreTimeScale = true;
            tween.StartTween(slideTween);
        }

        private bool GetGraphics()
        {
            if (_multiImageTarget == null)
            {
                this.GetImageTarget(out _multiImageTarget);
            }
            return _multiImageTarget != null;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_handleWrapper != null && _handle != null)
            {
                _startPos = (-_handleWrapper.rect.width/2) + (_handle.rect.width / 2);
                _handle.anchoredPosition = new Vector2(_startPos, _handle.anchoredPosition.y);
            }
            GetGraphics();
        }
#endif

        protected virtual void OnTweenEnd()
        {

        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            RecalcPositions(true);
        }

        private void RecalcPositions(bool force)
        {
            if (_handleWrapper != null && _handle != null)
            {
                _startPos = (-_handleWrapper.rect.width / 2) + (_handle.rect.width / 2);
                if (force)
                {
                    _handle.anchoredPosition = new Vector2(_startPos * (isOn ? -1 : 1), _handle.anchoredPosition.y);
                }
            }
        }
    }

}