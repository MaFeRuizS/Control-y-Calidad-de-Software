using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[System.Serializable]
public class DatosControl
{
    public int joyX;
    public int joyY;
    public int joyBtn;
    public int A;
    public int B;
    public int X;
    public int Y;
}

public class ControladorESP32 : MonoBehaviour
{
    [Header("Configuración de Red")]
    public string ipESP32 = "172.20.10.10";
    public float tiempoActualizacion = 0.1f;

    [Header("Botones de la Interfaz (Arrastra tus botones aquí)")]
    public Button uiBotonAmarillo_Metales;
    public Button uiBotonRojo_Organico;
    public Button uiBotonVerde_Inorganico;
    public Button uiBotonAzul_Reciclable;

    [Header("Calibración del Joystick")]
    public int umbralAlto = 3000; 
    public int umbralBajo = 1000; 

    private string url;
    private DatosControl estadoAnterior = new DatosControl();
    
    private bool moviendoX = false;
    private bool moviendoY = false;

    void Start()
    {
        url = "http://" + ipESP32 + "/estado"; 
        
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
                    DatosControl estadoActual = JsonUtility.FromJson<DatosControl>(json);
                    
                    ProcesarBotones(estadoActual);       // Lee botones físicos para el juego
                    ProcesarNavegacionMenu(estadoActual); // Lee el joystick para los menús
                    
                    estadoAnterior = estadoActual; 
                }
            }
            yield return new WaitForSeconds(tiempoActualizacion);
        }
    }

    void ProcesarBotones(DatosControl estadoActual)
    {
        // 1. METALES (Botón Amarillo en Interfaz) -> Conectado al físico Y
        if (estadoActual.Y == 1 && estadoAnterior.Y == 0) {
            if(uiBotonAmarillo_Metales != null) uiBotonAmarillo_Metales.Select();
            GameManager.Instance.ClassifyElement(0); 
        }
        
        // 2. ORGÁNICO (Botón Rojo en Interfaz) -> Conectado al físico B
        if (estadoActual.B == 1 && estadoAnterior.B == 0) {
            if(uiBotonRojo_Organico != null) uiBotonRojo_Organico.Select();
            GameManager.Instance.ClassifyElement(1);
        }

        // 3. INORGÁNICO (Botón Verde en Interfaz) -> Conectado al físico A
        if (estadoActual.A == 1 && estadoAnterior.A == 0) {
            if(uiBotonVerde_Inorganico != null) uiBotonVerde_Inorganico.Select();
            GameManager.Instance.ClassifyElement(2);
        }

        // 4. RECICLABLE (Botón Azul en Interfaz) -> Conectado al físico X
        if (estadoActual.X == 1 && estadoAnterior.X == 0) {
            if(uiBotonAzul_Reciclable != null) uiBotonAzul_Reciclable.Select();
            GameManager.Instance.ClassifyElement(3);
        }
    }

    void ProcesarNavegacionMenu(DatosControl estadoActual)
    {
        // 1. CLIC EN EL MENÚ:
        if (estadoActual.joyBtn == 1 && estadoAnterior.joyBtn == 0)
        {
            GameObject seleccionado = EventSystem.current.currentSelectedGameObject;
            if (seleccionado != null)
            {
                ExecuteEvents.Execute(seleccionado, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }

        // 2. NAVEGACIÓN DERECHA / IZQUIERDA (Eje X)
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

        // 3. NAVEGACIÓN ARRIBA / ABAJO (Eje Y)
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
        // Encuentra qué botón está seleccionado actualmente y busca el siguiente
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

        // Si hay un botón hacia donde moviste la palanca, lo selecciona
        if (siguiente != null) siguiente.Select();
    }
}