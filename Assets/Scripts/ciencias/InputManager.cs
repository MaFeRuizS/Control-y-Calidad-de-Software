using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Keyboard Mappings")]
    public KeyCode[] categoryKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    public int[] categoryValues = { 0, 1, 2, 3 };

    void Update()
    {
        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        for (int i = 0; i < categoryKeys.Length && i < categoryValues.Length; i++)
        {
            if (Input.GetKeyDown(categoryKeys[i]))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ClassifyElement(categoryValues[i]);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ProcessESP32Input(string command)
    {
        Debug.Log("ESP32 Input: " + command);
        
        if (command.StartsWith("CATEGORY_"))
        {
            int value = int.Parse(command.Replace("CATEGORY_", ""));
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClassifyElement(value);
            }
        }
    }
}
