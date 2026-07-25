using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitchTester : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene("MainScene");
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene("OutdoorHighSchoolScene");
        }
    }
}