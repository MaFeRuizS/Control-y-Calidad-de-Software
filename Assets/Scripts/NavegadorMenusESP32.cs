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
    private int estadoPalancaY = 0; 
    private int estadoPalancaX = 0;

    void Start(){
        if (InputArcadeManager.Instance != null) 
        {
            estadoAnterior = InputArcadeManager.Instance.estadoActual;
        }
        else 
        {
            estadoAnterior.joyBtn = 0;
            estadoAnterior.joyX = 2048; 
            estadoAnterior.joyY = 2048;
        }

        if (primerBotonDeLaEscena != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonDeLaEscena);
        }
        
    }

    void Update(){
        if (InputArcadeManager.Instance == null) {
            Debug.LogWarning("ALERTA: No encuentro el InputArcadeManager en la escena.");
            return;
        }

        DatosControlMatematicas estadoActual = InputArcadeManager.Instance.estadoActual;

        //Debug.Log("Joystick X: " + estadoActual.joyX + " | Joystick Y: " + estadoActual.joyY);

        if (EventSystem.current.currentSelectedGameObject == null && primerBotonDeLaEscena != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonDeLaEscena);
        }
        
        // 3. Clic de Botón
        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            GameObject seleccionado = EventSystem.current.currentSelectedGameObject;
            if (seleccionado != null)
            {
                ExecuteEvents.Execute(seleccionado, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }

        // Navegación X 
        int posicionFisicaX = estadoPalancaX; 

        if (estadoActual.joyX > 3500) posicionFisicaX = 1; // Tope Derecha
        else if (estadoActual.joyX < 500) posicionFisicaX = -1; // Tope Izquierda
        else if (estadoActual.joyX > 1500 && estadoActual.joyX < 2500) posicionFisicaX = 0; 

        if (posicionFisicaX == 1 && estadoPalancaX == 0) 
        {
            MoverUI(MoveDirection.Right); 
            estadoPalancaX = 1; 
        }
        else if (posicionFisicaX == -1 && estadoPalancaX == 0) 
        {
            MoverUI(MoveDirection.Left); 
            estadoPalancaX = -1; 
        }
        else if (posicionFisicaX == 0)
        {
            estadoPalancaX = 0; 
        }

        // Navegación Y
        int posicionFisicaY = estadoPalancaY;

        if (estadoActual.joyY > 3500) posicionFisicaY = -1;
        else if (estadoActual.joyY < 500) posicionFisicaY = 1;
        else if (estadoActual.joyY > 1500 && estadoActual.joyY < 2500) posicionFisicaY = 0;

        if (posicionFisicaY == -1 && estadoPalancaY == 0) 
        {
            //Debug.Log("¡SALTO HACIA ABAJO! Ejecutado por el objeto: " + gameObject.name);
            MoverUI(MoveDirection.Down); 
            estadoPalancaY = -1;
        }
        else if (posicionFisicaY == 1 && estadoPalancaY == 0) 
        {
            MoverUI(MoveDirection.Up); // Subimos un botón
            estadoPalancaY = 1; // Bloqueamos
        }
        else if (posicionFisicaY == 0)
        {
            estadoPalancaY = 0; 
        }

        estadoAnterior = estadoActual;
    }

    void MoverUI(MoveDirection direccion){
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