using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorDePaneles : MonoBehaviour
{
    [Header("Referencias a los Paneles")]
    public GameObject panelInstrucciones;
    public GameObject panelSeleccionModos;

    [Header("Configuración del Juego")]
    //  aquí va el nombre de la escena que se va cargar para el juego.
    public string nombreEscenaJuego = "EscenaDeJuego"; 

    public void RegresarAlMenu()
    {
        panelInstrucciones.SetActive(false);
        panelSeleccionModos.SetActive(false);
        
        Debug.Log("Regresando al menú principal...");
    }

    public void JugarUnJugador()
    {
        Debug.Log("Iniciando modo 1 Jugador");

        PlayerPrefs.SetInt("ModoJuego", 1); 

        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void JugarDosJugadores()
    {
        Debug.Log("Iniciando modo 2 Jugadores");

        PlayerPrefs.SetInt("ModoJuego", 2);
        
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}