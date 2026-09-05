using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Summary
// Allows for the visual / backend swapping to view different binds depending on input device
// Allows us to for-go a full blown controller page
namespace VIAW.UI {
    public class ControllerTabsManager : MonoBehaviour
    {
        public static int controllerType = 0;
        
        // [SerializeField] private ButtonManager tabsButton;

        private static readonly Color disabledColor = new Color(96f/255f, 96f/255f, 96f/255f, 1f);
        private static readonly Color enabledColor = new Color(1f, 1f, 1f, 1f);

        private int lastControllerType = -1;

        // TODO: Rewrite the Live Updating and swapping of respective inputs
        // Update
        // Fixed Update 
        // Were handling this, but was pointless, and expensive for it's overall purpose

        public void SwapInput()
        {
            controllerType = (controllerType == 0) ? 1 : 0;
        }

        public static void SetControllerType(int newType)
        {
            if(newType >= 0 && newType <= 1) { controllerType = newType; }
        }

        public static int GetControllerType()
        {
            return controllerType;
        }

        public static bool IsUsingKeyboard()
        {
            return controllerType == 0;
        }

        public static bool IsUsingController()
        {
            return controllerType == 1;
        }
    }
}