using System.Collections.Generic;
using Ultraleap.UI;
using UnityEngine.UI;

public class MultiImageToggleGroup : ToggleGroup
{
    private bool _valueChangedThisFrame = false;
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        foreach (Toggle toggle in m_Toggles)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            UpdateMultiImageTarget(toggle);
        }
    }

    protected override void OnDisable()
    {
        foreach (Toggle toggle in m_Toggles)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            UpdateMultiImageTarget(toggle);
        }
    }

    private void OnToggleValueChanged(bool val)
    {
        if (_valueChangedThisFrame)
        {
            return;
        }

        foreach (Toggle toggle in m_Toggles)
        {
            UpdateMultiImageTarget(toggle);
        }

        _valueChangedThisFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        _valueChangedThisFrame = false;
    }

    protected void UpdateMultiImageTarget(Toggle toggle)
    {
        //force all toggles graphic states to update when one toggle in the group updates
        MultiImageTarget[] toggleTargets = toggle.GetComponentsInChildren<MultiImageTarget>();

        for (int i = 0; i < toggleTargets.Length; i++)
        {
            toggleTargets[i].SetState(toggle.isOn ? "Highlighted" : "Normal", toggle.isOn);
        }

    }

    public void ForceCorrectState()
    {
        foreach (Toggle toggle in m_Toggles)
        {
            UpdateMultiImageTarget(toggle);
        }
    }
}
