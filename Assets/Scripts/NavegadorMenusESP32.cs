using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavegadorMenusESP32 : MonoBehaviour
{
    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    [Header("Auto-Selección (Opcional)")]
    public GameObject primerBotonDeLaEscena; 

    private DatosControlMatematicas estadoAnterior = new DatosControlMatematicas();
    private bool moviendoX = false;
    private bool moviendoY = false;

    void Start()
    {
        estadoAnterior.joyBtn = 0;
        estadoAnterior.joyX = 2048; // Centro aproximado
        estadoAnterior.joyY = 2048;

        if (primerBotonDeLaEscena != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonDeLaEscena);
        }
    }

    void Update()
    {
        if (InputArcadeManager.Instance == null) {
            Debug.LogWarning("ALERTA: No encuentro el InputArcadeManager en la escena.");
            return;
        }

        DatosControlMatematicas estadoActual = InputArcadeManager.Instance.estadoActual;

        Debug.Log("Joystick X: " + estadoActual.joyX + " | Joystick Y: " + estadoActual.joyY);

        if (EventSystem.current.currentSelectedGameObject == null && primerBotonDeLaEscena != null)
        {
            bool movioJoystick = estadoActual.joyX > umbralAlto || estadoActual.joyX < umbralBajo || 
                                 estadoActual.joyY > umbralAlto || estadoActual.joyY < umbralBajo;
            if (movioJoystick) {
                EventSystem.current.SetSelectedGameObject(primerBotonDeLaEscena);
                return; 
            }
        }
        
        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            GameObject seleccionado = EventSystem.current.currentSelectedGameObject;
            if (seleccionado != null)
            {
                ExecuteEvents.Execute(seleccionado, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }

        // Navegación X 
        if (estadoActual.joyX > umbralAlto && !moviendoX) { MoverUI(MoveDirection.Right); moviendoX = true; } 
        else if (estadoActual.joyX < umbralBajo && !moviendoX) { MoverUI(MoveDirection.Left); moviendoX = true; } 
        else if (estadoActual.joyX >= umbralBajo && estadoActual.joyX <= umbralAlto) { moviendoX = false; }

        // --- NAVEGACIÓN Y CORREGIDA ---
        // Ahora si supera el umbral alto (ej. 4000), baja. Si baja del umbral bajo (ej. 0), sube.
        if (estadoActual.joyY > umbralAlto && !moviendoY) { MoverUI(MoveDirection.Down); moviendoY = true; } 
        else if (estadoActual.joyY < umbralBajo && !moviendoY) { MoverUI(MoveDirection.Up); moviendoY = true; } 
        else if (estadoActual.joyY >= umbralBajo && estadoActual.joyY <= umbralAlto) { moviendoY = false; }

        estadoAnterior = estadoActual;
    }

    void MoverUI(MoveDirection direccion)
    {
        GameObject objetoActual = EventSystem.current.currentSelectedGameObject;
        if (objetoActual == null) return; 

        Selectable selectableActual = objetoActual.GetComponent<Selectable>();
        if (selectableActual == null) return;

        Selectable siguiente = null;
        switch (direccion)
        {
            case MoveDirection.Up: siguiente = selectableActual.FindSelectableOnUp(); break;
            case MoveDirection.Down: siguiente = selectableActual.FindSelectableOnDown(); break;
            case MoveDirection.Left: siguiente = selectableActual.FindSelectableOnLeft(); break;
            case MoveDirection.Right: siguiente = selectableActual.FindSelectableOnRight(); break;
        }

        if (siguiente != null) siguiente.Select();
    }
}