using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Ultraleap.UI
{

    [RequireComponent(typeof(MultiImageTarget))]
    public class MultiImageButton : Button
    {
        [SerializeField, HideInInspector] private MultiImageTarget _multiImageTarget;

        protected override void Start()
        {
            base.Start();

            _multiImageTarget?.SetState(currentSelectionState.ToString());
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!GetGraphics())
                return;

            _multiImageTarget?.SetState(state.ToString());
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
            GetGraphics();
        }
#endif
    }

}