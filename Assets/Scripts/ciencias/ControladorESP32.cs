using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ControladorESP32 : MonoBehaviour
{
    [Header("Botones de la Interfaz (Arrastra tus botones aquí)")]
    public Button uiBotonAmarillo_Metales;
    public Button uiBotonRojo_Organico;
    public Button uiBotonVerde_Inorganico;
    public Button uiBotonAzul_Reciclable;

    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    // Usamos la misma estructura de datos del Manager Global
    private DatosControlMatematicas estadoAnterior = new DatosControlMatematicas();
    
    private bool moviendoX = false;
    private bool moviendoY = false;

    void Start()
    {
        // Inicializamos estados en 0
        estadoAnterior.A = 0; 
        estadoAnterior.B = 0; 
        estadoAnterior.X = 0; 
        estadoAnterior.Y = 0;
        estadoAnterior.joyBtn = 0;
    }

    void Update()
    {
        // 1. Verificamos que el Manager Global exista
        if (InputArcadeManager.Instance == null) return;

        // 2. Le pedimos el estado actual
        DatosControlMatematicas estadoActual = InputArcadeManager.Instance.estadoActual;

        // 3. Procesamos los inputs
        ProcesarBotones(estadoActual);       
        ProcesarNavegacionMenu(estadoActual); 
        
        // 4. Guardamos el estado
        estadoAnterior = estadoActual; 
    }

    void ProcesarBotones(DatosControlMatematicas estadoActual)
    {
        // 1. METALES (Botón Amarillo -> Y)
        if (estadoActual.Y == 1 && estadoAnterior.Y == 0) {
            if(uiBotonAmarillo_Metales != null) uiBotonAmarillo_Metales.Select();
            if(GameManager.Instance != null) GameManager.Instance.ClassifyElement(0); 
        }
        
        // 2. ORGÁNICO (Botón Rojo -> B)
        if (estadoActual.B == 1 && estadoAnterior.B == 0) {
            if(uiBotonRojo_Organico != null) uiBotonRojo_Organico.Select();
            if(GameManager.Instance != null) GameManager.Instance.ClassifyElement(1);
        }

        // 3. INORGÁNICO (Botón Verde -> A)
        if (estadoActual.A == 1 && estadoAnterior.A == 0) {
            if(uiBotonVerde_Inorganico != null) uiBotonVerde_Inorganico.Select();
            if(GameManager.Instance != null) GameManager.Instance.ClassifyElement(2);
        }

        // 4. RECICLABLE (Botón Azul -> X)
        if (estadoActual.X == 1 && estadoAnterior.X == 0) {
            if(uiBotonAzul_Reciclable != null) uiBotonAzul_Reciclable.Select();
            if(GameManager.Instance != null) GameManager.Instance.ClassifyElement(3);
        }
    }

    void ProcesarNavegacionMenu(DatosControlMatematicas estadoActual)
    {
        // 1. CLIC EN EL JOYSTICK PARA PAUSA/ENTER
        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            if (Time.timeScale == 1f) 
            {
                if(GameManager.Instance != null) 
                    GameManager.Instance.PausarJuego();
            }
            else 
            {
                GameObject seleccionado = EventSystem.current.currentSelectedGameObject;
                if (seleccionado != null)
                {
                    ExecuteEvents.Execute(seleccionado, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                }
            }
        }

        // --- EL CANDADO MAESTRO PARA EL MOVIMIENTO ---
        if (Time.timeScale == 1f) return; 

        // 2. NAVEGACIÓN DERECHA / IZQUIERDA
        if (estadoActual.joyX > umbralAlto && !moviendoX) {
            MoverUI(MoveDirection.Right); 
            moviendoX = true;
        } 
        else if (estadoActual.joyX < umbralBajo && !moviendoX) {
            MoverUI(MoveDirection.Left);
            moviendoX = true;
        } 
        else if (estadoActual.joyX >= umbralBajo && estadoActual.joyX <= umbralAlto) {
            moviendoX = false; 
        }

        // 3. NAVEGACIÓN ARRIBA / ABAJO
        if (estadoActual.joyY > umbralAlto && !moviendoY) {
            MoverUI(MoveDirection.Up); 
            moviendoY = true;
        } 
        else if (estadoActual.joyY < umbralBajo && !moviendoY) {
            MoverUI(MoveDirection.Down);
            moviendoY = true;
        } 
        else if (estadoActual.joyY >= umbralBajo && estadoActual.joyY <= umbralAlto) {
            moviendoY = false; 
        }
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