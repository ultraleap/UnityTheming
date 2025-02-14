using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ultraleap.UI
{
    [RequireComponent(typeof(MultiImageTarget))]
    public class SmoothDropdown : TMP_Dropdown
    {
        [SerializeField, HideInInspector] private MultiImageTarget _multiImageTarget;

        private GameObject _dropdownTemplate;

        private Transform _templateHeight;
        private float _dropdownHeight = 0;

        protected override void Start()
        {
            base.Start();
            ResizeElements();
            _multiImageTarget?.SetState(currentSelectionState.ToString());
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            //get the component, if it could not get the graphics, return here
            if (!GetGraphics())
                return;

            _multiImageTarget?.SetState(state.ToString());
        }

        protected override GameObject CreateDropdownList(GameObject template)
        {
            this._dropdownTemplate = base.CreateDropdownList(template);
            return this._dropdownTemplate;
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            if (interactable)
            {
                RebindItems();
                ApplyRoundedCorners();
            }
            
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (interactable)
            {
                RebindItems();
                ApplyRoundedCorners();
            }
        }

        private void ApplyRoundedCorners()
        {
            RoundedCorners rc = _dropdownTemplate.transform.GetComponentInChildren<RoundedCorners>(true);
            if(rc != null)
            {
                rc.copyHeight = false;
                rc.radius = _dropdownHeight;
                rc.UpdateImage();
            }

            IndividualRoundedCorners[] irc = _dropdownTemplate.transform.GetComponentsInChildren<IndividualRoundedCorners>();
            if(irc.Length == 0)
            {
                return;
            }
            if(irc.Length == 1)
            {
                irc[0].corners = Vector4.one * irc[0].Height / 2f;
                irc[0].ForceUpdateCorners();
            }
            if(irc.Length >= 1)
            {
                irc[0].corners = new Vector4(irc[0].Height / 2f, irc[0].Height / 2f, 0, 0);
                irc[irc.Length - 1].corners = new Vector4(0, 0, irc[0].Height / 2f, irc[0].Height / 2f);

                irc[0].ForceUpdateCorners();
                irc[irc.Length - 1].ForceUpdateCorners();

                for (int i = 1; i < irc.Length - 1; i++)
                {
                    irc[i].corners = new Vector4(0, 0, 0, 0);
                    irc[i].ForceUpdateCorners();
                }
            }
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
            ResizeElements();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            ResizeElements();
        }

        private void ResizeElements()
        {
            if(_templateHeight == null)
            {
                _templateHeight = template.Find("Viewport/Content");
            }
            if (_templateHeight != null)
            {
                Vector2 r = ((RectTransform)_templateHeight).sizeDelta;
                r.y = ((RectTransform)transform).rect.height;
                ((RectTransform)_templateHeight).sizeDelta = r;
                _dropdownHeight = r.y;
            }
        }

        // The TMP dropdown adds items that block interaction states on dropdown elements
        // To fix this we just delete it, the info on it is generally only used for initialisation anyway
        private void RebindItems()
        {
            IndividualRoundedCorners[] irc = _dropdownTemplate.transform.GetComponentsInChildren<IndividualRoundedCorners>();
            for (int i = 0; i < irc.Length; i++)
            {
                Toggle t = irc[i].GetComponentInParent<Toggle>();
                if(t == null)
                {
                    continue;
                }

                // This prevents the dropdown being locked into the selected state when an item is selected
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => { Deselect(); });
                trigger.triggers.Add(entry);

                DropdownItem di = t.GetComponent<DropdownItem>();
                if(di != null)
                {
                    Destroy(di);
                }
            }
        }

        // This also prevents the dropdown being locked into the selected state on close
        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            GameObject blocker = base.CreateBlocker(rootCanvas);
            Button blockerButton = blocker.GetComponent<Button>();
            blockerButton.onClick.AddListener(Deselect);
            return blocker;
        }

        private void Deselect()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

}