using UnityEngine;
using UnityEngine.InputSystem;

public class AdminPin : MonoBehaviour
{
    [Header("Admin Prompt UI")]
    [SerializeField] private GameObject screen;

    private bool shortcutTriggered = false;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        bool ctrlHeld =
            Keyboard.current.leftCtrlKey.isPressed ||
            Keyboard.current.rightCtrlKey.isPressed;

        bool shiftHeld =
            Keyboard.current.leftShiftKey.isPressed ||
            Keyboard.current.rightShiftKey.isPressed;

        bool altHeld =
            Keyboard.current.leftAltKey.isPressed ||
            Keyboard.current.rightAltKey.isPressed;

        // Trigger once when all three keys are held
        if (ctrlHeld && shiftHeld && altHeld)
        {
            if (!shortcutTriggered)
            {
                ShowPrompt();
                shortcutTriggered = true;
            }
        }
        else
        {
            // Reset after the user releases any modifier
            shortcutTriggered = false;
        }
    }

    private void ShowPrompt()
    {
        if (screen != null)
        {
            screen.SetActive(true);
            Debug.Log("Admin Pin Prompt Triggered!");
        }
    }

    public void HidePrompt()
    {
        if (screen != null)
        {
            screen.SetActive(false);
        }

        shortcutTriggered = false;
    }

}
