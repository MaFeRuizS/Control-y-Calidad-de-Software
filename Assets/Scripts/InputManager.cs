using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Keyboard Mappings")]
    public KeyCode[] categoryKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    public int[] categoryValues = { 4, 5, 6, 10 }; // Valores por defecto

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

        // Teclas adicionales de control
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reiniciar juego
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Pausar o salir
            Application.Quit();
        }
    }

    // Método para cuando implementes el ESP32
    public void ProcessESP32Input(string command)
    {
        // Este método se usará cuando conectes el ESP32
        Debug.Log("ESP32 Input: " + command);
        
        // Ejemplo de cómo procesar comandos del ESP32
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
