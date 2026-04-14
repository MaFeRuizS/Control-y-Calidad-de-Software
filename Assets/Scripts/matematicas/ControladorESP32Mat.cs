using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[System.Serializable]
public class DatosControlMatematicas
{
    public int joyX;
    public int joyY;
    public int joyBtn;
    public int A;
    public int B;
    public int X;
    public int Y;
}

public class ControladorESP32Mat : MonoBehaviour
{
    [Header("Configuración de Red")]
    public string ipESP32 = "192.168.1.80";
    public float tiempoActualizacion = 0.1f;

    [Header("Botones Matemáticas (Arrastra tus botones aquí)")]
    public Button uiBotonEstrella;
    public Button uiBotonCirculo;
    public Button uiBotonTriangulo;
    public Button uiBotonRectangulo;

    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    private string url;
    private DatosControlMatematicas estadoAnterior = new DatosControlMatematicas();
    
    private bool moviendoX = false;
    private bool moviendoY = false;

    void Start()
    {
        // ¡Aquí está la magia del Trim para evitar errores de espacios!
        url = "http://" + ipESP32.Trim() + "/estado"; 
        
        estadoAnterior.A = 0; 
        estadoAnterior.B = 0; 
        estadoAnterior.X = 0; 
        estadoAnterior.Y = 0;
        estadoAnterior.joyBtn = 0;
        
        StartCoroutine(LeerControlador());
    }

    IEnumerator LeerControlador()
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
                {
                    string json = webRequest.downloadHandler.text;
                    DatosControlMatematicas estadoActual = JsonUtility.FromJson<DatosControlMatematicas>(json);
                    
                    ProcesarBotones(estadoActual);       // Lee botones físicos
                    ProcesarNavegacionMenu(estadoActual); // Lee el joystick
                    
                    estadoAnterior = estadoActual; 
                }
            }
            yield return new WaitForSeconds(tiempoActualizacion);
        }
    }

    void ProcesarBotones(DatosControlMatematicas estadoActual)
    {
        // 1. ESTRELLA -> Digamos que es el botón Amarillo (Y)
        if (estadoActual.Y == 1 && estadoAnterior.Y == 0) {
            if(uiBotonEstrella != null) uiBotonEstrella.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Estrella"); 
        }
        
        // 2. CÍRCULO -> Digamos que es el botón Rojo (B)
        if (estadoActual.B == 1 && estadoAnterior.B == 0) {
            if(uiBotonCirculo != null) uiBotonCirculo.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Circulo");
        }

        // 3. TRIÁNGULO -> Digamos que es el botón Verde (A)
        if (estadoActual.A == 1 && estadoAnterior.A == 0) {
            if(uiBotonTriangulo != null) uiBotonTriangulo.Select();
            if(GameManagerMatematicas.Instance != null) 
                GameManagerMatematicas.Instance.ClassifyElement("Triangulo");
        }

        // 4. RECTÁNGULO -> Digamos que es el botón Azul (X)
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