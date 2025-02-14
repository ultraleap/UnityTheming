using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideToggleLabel : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private TextMeshProUGUI _text;

    private void OnValidate()
    {
        ValidateComponents();
    }

    // Start is called before the first frame update
    void Start()
    {
        ValidateComponents();
        _toggle?.onValueChanged.AddListener(OnToggleValueChanged);

        OnToggleValueChanged(_toggle.isOn);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        _text.text = isOn ? "ON" : "OFF";
    }

    private void ValidateComponents()
    {
        if (_toggle == null)
        {
            _toggle = gameObject.GetComponentInChildren<Toggle>();

            if (_toggle == null)
            {
                _toggle = gameObject.GetComponentInParent<Toggle>();
            }
        }

        if (_text == null)
        {
            _text = gameObject.GetComponent<TextMeshProUGUI>();
        }
    }
}
