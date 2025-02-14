using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ultraleap.UI
{
    public static class UGUIHelper
    {
        
        public static bool GetImageTarget(this Selectable selectable, out MultiImageTarget target)
        {
            selectable.navigation = new Navigation() { mode = Navigation.Mode.None };
            return selectable.TryGetComponent(out target);
        }

    }
}