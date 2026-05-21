using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ControladorESP32 : MonoBehaviour
{
    [Header("Botones de la Interfaz (Arrastra tus botones aquí)")]
    public Button uiBotonAmarillo_Metales;
    public Button uiBotonVerde_Organico;
    public Button uiBotonBlanco_OtrosInorganicos;
    public Button uiBotonAzul_Reciclable;

    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    private DatosControlMatematicas estadoAnterior = new DatosControlMatematicas();
    
    private bool moviendoX = false;
    private bool moviendoY = false;

    void Start()
    {
        estadoAnterior.A = 0; 
        estadoAnterior.B = 0; 
        estadoAnterior.X = 0; 
        estadoAnterior.Y = 0;
        estadoAnterior.joyBtn = 0;
    }

    void Update()
    {
        if (InputArcadeManager.Instance == null) return;

        DatosControlMatematicas estadoActual = InputArcadeManager.Instance.estadoActual;

        ProcesarBotones(estadoActual);       
        ProcesarNavegacionMenu(estadoActual); 
        
        estadoAnterior = estadoActual; 
    }

    void ProcesarBotones(DatosControlMatematicas estadoActual)
    {
        ControladorPartida cp = FindObjectOfType<ControladorPartida>();
        if (cp != null && !cp.juegoIniciado) return;

        // 1. METALES (Botón Amarillo -> Y)
        if (estadoActual.Y == 1 && estadoAnterior.Y == 0) {
            if(uiBotonAmarillo_Metales != null) uiBotonAmarillo_Metales.Select();
            if(GameManager.Instance != null) GameManager.Instance.ClassifyElement(0); 
        }

         // 2. ORGÁNICO (Botón Verde -> A)
        if (estadoActual.A == 1 && estadoAnterior.A == 0) {
            if(uiBotonVerde_Organico != null) uiBotonVerde_Organico.Select();
            if(GameManager.Instance != null) GameManager.Instance.ClassifyElement(1);
        }
        
        // 3. INORGÁNICO (Botón blanco -> B)
        if (estadoActual.B == 1 && estadoAnterior.B == 0) {
            if(uiBotonBlanco_OtrosInorganicos != null) uiBotonBlanco_OtrosInorganicos.Select();
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
        // ESCUDO DE CONTEO: Si el contador está activo, ignoramos por completo el menú y los clics
        ControladorPartida cp = FindObjectOfType<ControladorPartida>();
        if (cp != null && !cp.juegoIniciado) return;

        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            if (Time.timeScale == 1f) 
            {
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