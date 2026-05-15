using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ControladorESP32Mat : MonoBehaviour
{
    [Header("Botones Matemáticas (Arrastra tus botones aquí)")]
    public Button uiBotonEstrella;
    public Button uiBotonCirculo;
    public Button uiBotonTriangulo;
    public Button uiBotonRectangulo;

    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    private DatosControlMatematicas estadoAnterior = new DatosControlMatematicas();
    
    private bool moviendoX = false;
    private bool moviendoY = false;

    void Start()
    {
        // Inicializamos el estado anterior en 0 para evitar falsos clics al iniciar
        estadoAnterior.A = 0; 
        estadoAnterior.B = 0; 
        estadoAnterior.X = 0; 
        estadoAnterior.Y = 0;
        estadoAnterior.joyBtn = 0;
    }

    void Update()
    {
        // 1. Verificamos que el Manager Global exista (Vital por si corres la escena sola)
        if (InputArcadeManager.Instance == null) return;

        // 2. Le pedimos el estado actual de los botones al Manager Global
        DatosControlMatematicas estadoActual = InputArcadeManager.Instance.estadoActual;

        // 3. Procesamos los inputs (El código de adentro ya se encarga de ver si hubo clics)
        ProcesarBotones(estadoActual);       
        ProcesarNavegacionMenu(estadoActual); 
        
        // 4. Guardamos el estado para compararlo en el siguiente frame
        estadoAnterior = estadoActual; 
    }

    void ProcesarBotones(DatosControlMatematicas estadoActual)
    {
        // 1. ESTRELLA -> Botón Amarillo (Y)
        if (estadoActual.Y == 1 && estadoAnterior.Y == 0) {
            if(uiBotonEstrella != null) uiBotonEstrella.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Estrella"); 
        }
        
        // 2. CÍRCULO -> Botón Rojo (B)
        if (estadoActual.B == 1 && estadoAnterior.B == 0) {
            if(uiBotonCirculo != null) uiBotonCirculo.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Circulo");
        }

        // 3. TRIÁNGULO -> Botón Verde (A)
        if (estadoActual.A == 1 && estadoAnterior.A == 0) {
            if(uiBotonTriangulo != null) uiBotonTriangulo.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Triangulo");
        }

        // 4. RECTÁNGULO -> Botón Azul (X)
        if (estadoActual.X == 1 && estadoAnterior.X == 0) {
            if(uiBotonRectangulo != null) uiBotonRectangulo.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Rectangulo");
        }
    }

    void ProcesarNavegacionMenu(DatosControlMatematicas estadoActual)
    {
        // 1. EL CLIC DEL JOYSTICK (Presionar la palanca hacia abajo)
        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            // A. Si el juego está corriendo normalmente, el clic funciona para PAUSAR
            if (Time.timeScale == 1f) 
            {
                if(GameManagerMatematicas.Instance != null) 
                    GameManagerMatematicas.Instance.PausarJuego();
            }
            // B. Si el juego YA está pausado (o en Game Over/Victoria), el clic funciona como "ENTER"
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
        // Si el juego está corriendo, apagamos el movimiento del joystick para evitar accidentes
        if (Time.timeScale == 1f) return; 

        // 2. NAVEGACIÓN DERECHA / IZQUIERDA (Solo en Menús)
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

        // 3. NAVEGACIÓN ARRIBA / ABAJO (Solo en Menús)
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