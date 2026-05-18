using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración del Juego")]
    public float elementFallSpeed = 2f;
    public int maxLives = 3;
    public int elementsPerLevel = 15; 
    public float timeLimit = 120f; 

    [Header("UI Marcadores")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI vidasText; 
    public TextMeshProUGUI aciertosText; 

    [Header("UI Feedback")]
    public GameObject panelFeedback;
    public Image imagenFondoPopup;
    public Sprite imagenCorrecto;
    public Sprite imagenIncorrecto;

    [Header("UI Final")]
    public GameObject gameOverPanel;      
    public GameObject levelCompletePanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalAciertosText;
    public TextMeshProUGUI finalTimeText;

    [Header("Textos Level Complete")]
    public TextMeshProUGUI PuntajeFinal;
    public TextMeshProUGUI AciertosFinal;
    public TextMeshProUGUI TiempoFinal;

    [Header("Conexiones")]
    public GeneradorInteligente generador; 
    public List<RectTransform> categoryButtons;

    [Header("UI Panels y Navegación Joystick")]
    public GameObject primerBotonGameOver; 
    public GameObject primerBotonVictoria;
    
    [Header("Conteo de Errores")]
    public int erroresMetales = 0;
    public int erroresOrganicos = 0;
    public int erroresInorganicos = 0;
    public int erroresReciclables = 0;

    [Header("Conexión con la Fachada")]
    public GestorRetroalimentacion panelRetroalimentacion;

    // --- VARIABLES PRIVADAS ---
    public bool juegoPausado = false;
    private int puntuacion = 0;
    private int aciertosActuales = 0;
    private int vidasActuales;
    private float tiempoRestante;
    

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        vidasActuales = maxLives;
        tiempoRestante = timeLimit;
        // Asumimos que el juego inicia pausado hasta que el ControladorPartida termine el 3,2,1
        juegoPausado = true; 

        ActualizarInterfaz();
        
        if(panelFeedback) panelFeedback.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
    }

    void Update() {
        // Solo corre el tiempo si el nuevo Controlador Partida quitó la pausa
        if (!juegoPausado) {
            ManejarCronometro();
        }
    }

    void ManejarCronometro() {
        if (tiempoRestante > 0) {
            tiempoRestante -= Time.deltaTime;
            ActualizarInterfaz();
        } else {
            TerminarJuego(false);
        }
    }

    public float GetFallSpeed() => elementFallSpeed;

    public void ClassifyElement(int index) {
        if (index >= 0 && index < categoryButtons.Count) {
            string tagBoton = categoryButtons[index].gameObject.tag;
            ClassifyElement(tagBoton);
        }
    }

    public void ClassifyElement(string tagBoton) {
        if (juegoPausado) return; 
        
        GameObject objetoActual = generador.GetObjetoActual();
        if (objetoActual != null) {
            if (objetoActual.CompareTag(tagBoton)) {
                ProcesarAcierto(objetoActual);
            }
            else {
                ProcesarError();
            }
        }
    }

    public void ProcesarAcierto(GameObject obj) {
        puntuacion += 10;
        aciertosActuales++;
        
        MostrarPopUp(imagenCorrecto);
        Destroy(obj);
        ActualizarInterfaz();

        if (aciertosActuales >= elementsPerLevel) {
            TerminarJuego(true);
        }
    }

    public void ProcesarError(bool fuePorCaida = false) {
        if (juegoPausado) return;

        GameObject objetoActual = generador.GetObjetoActual();
        if (objetoActual != null) {
            RegistrarError(objetoActual.tag); 
        }
        
        if (fuePorCaida) {
            vidasActuales--; 
        } else {
            puntuacion -= 5; 
            if (puntuacion < 0) puntuacion = 0; 
        }
        
        MostrarPopUp(imagenIncorrecto);
        
        if (objetoActual != null) Destroy(objetoActual);
        
        ActualizarInterfaz();

        if (vidasActuales <= 0) {
            TerminarJuego(false); 
        }
    }

    void ActualizarInterfaz() {
        if (scoreText) scoreText.text = "Puntos: " + puntuacion.ToString();
        if (vidasText) vidasText.text = "Vidas:" + vidasActuales.ToString();
        if (aciertosText) aciertosText.text = "Aciertos:" + aciertosActuales.ToString();
        
        if (timeText) {
            int min = Mathf.FloorToInt(tiempoRestante / 60);
            int seg = Mathf.FloorToInt(tiempoRestante % 60);
            timeText.text = string.Format("{0:00}:{1:00}", min, seg);
        }
    }

    public void TerminarJuego(bool victoria) {
        if (juegoPausado) return;
        
        juegoPausado = true;
        Time.timeScale = 0f; 

        if (generador != null) generador.DetenerGeneracion();
        GameObject objetoEnVuelo = generador.GetObjetoActual();
        if (objetoEnVuelo != null) Destroy(objetoEnVuelo);

        float tiempoUsado = timeLimit - tiempoRestante;

        if (gameOverPanel) {
            gameOverPanel.SetActive(true);
            
            if(finalScoreText) finalScoreText.text = puntuacion.ToString();
            if (finalAciertosText) finalAciertosText.text = aciertosActuales.ToString();
            if(finalTimeText) finalTimeText.text = tiempoUsado.ToString("F0") + "s";

            if(primerBotonGameOver) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(primerBotonGameOver);
            }
        }

        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        panelRetroalimentacion.MostrarResultados(puntuacion, aciertosActuales, erroresMetales, erroresOrganicos, erroresInorganicos, erroresReciclables);    
    }

    public void ReiniciarJuego() {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame() {
        ReiniciarJuego(); 
    }

    void MostrarPopUp(Sprite diseño) {
        if (imagenFondoPopup && panelFeedback) {
            imagenFondoPopup.sprite = diseño;
            panelFeedback.SetActive(true);
            StopCoroutine("OcultarMensaje");
            StartCoroutine(OcultarMensaje());
        }
    }

    IEnumerator OcultarMensaje() {
        yield return new WaitForSeconds(0.8f);
        if(panelFeedback) panelFeedback.SetActive(false);
    }

   public void RegistrarError(string categoriaBasura)
    {
        Debug.Log("Intentando registrar error para el tag: [" + categoriaBasura + "]"); 

        if (categoriaBasura == "Metales") {
            erroresMetales++;
        }
        else if (categoriaBasura == "Organicos" || categoriaBasura == "Organico" || categoriaBasura == "Orgánico") {
            erroresOrganicos++;
        }
        else if (categoriaBasura == "Inorganicos" || categoriaBasura == "Inorganico" || categoriaBasura == "Inorgánico") {
            erroresInorganicos++;
        }
        else if (categoriaBasura == "Reciclables" || categoriaBasura == "Reciclable") {
            erroresReciclables++;
        }
    }
}