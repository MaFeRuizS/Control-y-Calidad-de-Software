using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavegadorMenusESP32 : MonoBehaviour
{
    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    // Opcional: Arrastra aquí el primer botón que quieres que se seleccione al abrir la escena
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

        // Si asignaste un botón, lo selecciona automáticamente al entrar a la escena
        if (primerBotonDeLaEscena != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonDeLaEscena);
        }
    }

    void Update()
    {
        if (InputArcadeManager.Instance == null) return;

        DatosControlMatematicas estadoActual = InputArcadeManager.Instance.estadoActual;

        if (EventSystem.current.currentSelectedGameObject == null && primerBotonDeLaEscena != null)
        {
            bool movioJoystick = estadoActual.joyX > umbralAlto || estadoActual.joyX < umbralBajo || 
                                 estadoActual.joyY > umbralAlto || estadoActual.joyY < umbralBajo;
            if (movioJoystick) {
                EventSystem.current.SetSelectedGameObject(primerBotonDeLaEscena);
                return; 
            }
        }
        // ----------------------------

        // Clic del Joystick
        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            GameObject seleccionado = EventSystem.current.currentSelectedGameObject;
            if (seleccionado != null)
            {
                ExecuteEvents.Execute(seleccionado, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }

        // Navegación X e Y
        if (estadoActual.joyX > umbralAlto && !moviendoX) { MoverUI(MoveDirection.Right); moviendoX = true; } 
        else if (estadoActual.joyX < umbralBajo && !moviendoX) { MoverUI(MoveDirection.Left); moviendoX = true; } 
        else if (estadoActual.joyX >= umbralBajo && estadoActual.joyX <= umbralAlto) { moviendoX = false; }

        if (estadoActual.joyY > umbralAlto && !moviendoY) { MoverUI(MoveDirection.Up); moviendoY = true; } 
        else if (estadoActual.joyY < umbralBajo && !moviendoY) { MoverUI(MoveDirection.Down); moviendoY = true; } 
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