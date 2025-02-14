using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ultraleap.UI
{
    public class ToggleIcons : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Image _activeIcon, _inactiveIcon;

        // Start is called before the first frame update
        void Start()
        {
            if (_toggle == null)
            {
                _toggle = GetComponent<Toggle>();
                if(_toggle == null)
                {
                    Debug.LogError("Toggle not assigned to toggle icons", gameObject);
                    return;
                }
            }

            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
            OnToggleValueChanged(_toggle.isOn);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            _activeIcon.gameObject.SetActive(isOn);
            _inactiveIcon.gameObject.SetActive(!isOn);
        }
    }
}