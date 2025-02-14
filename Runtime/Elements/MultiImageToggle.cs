using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Ultraleap.UI
{

    [RequireComponent(typeof(MultiImageTarget))]
    public class MultiImageToggle : Toggle
    {
        [SerializeField, HideInInspector] private MultiImageTarget _multiImageTarget;

        protected override void Start()
        {
            base.Start();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            //get the component, if it could not get the graphics, return here
            if (!GetGraphics())
                return;

            _multiImageTarget?.SetState(state.ToString(), isOn);
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
