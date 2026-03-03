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
    public int elementsPerLevel = 15; // Condición 1: Meta de aciertos
    public float timeLimit = 120f;    // Condición 2: Límite de tiempo

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
    public GameObject gameOverPanel;      // Para Tiempo/Vidas
    public GameObject levelCompletePanel; // Para los 15 aciertos
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalAciertosText;
    public TextMeshProUGUI finalTimeText;

    [Header("Conexiones")]
    public GeneradorInteligente generador; 
    public List<RectTransform> categoryButtons;

    [Header("UI Panels")]
    public GameObject pausePanel; // Aquí arrastraremos tu panel
    
    // --- VARIABLES PRIVADAS (Ya sin duplicados) ---
    private bool juegoPausado = false;
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
        juegoPausado = false;
        Time.timeScale = 1f; // Aseguramos que el tiempo fluya al iniciar

        ActualizarInterfaz();
        
        // Apagamos todos los paneles al iniciar
        if(panelFeedback) panelFeedback.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
        if(pausePanel) pausePanel.SetActive(false); 
    }

    // --- UPDATE UNIFICADO ---
    void Update() {
        // 1. Manejar el cronómetro solo si el juego NO está pausado
        if (!juegoPausado) {
            ManejarCronometro();
        }
    }

    void ManejarCronometro() {
        if (tiempoRestante > 0) {
            tiempoRestante -= Time.deltaTime;
            ActualizarInterfaz();
        } else {
            // CONDICIÓN 2: El tiempo se agota
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
        if (juegoPausado) return; // Si está pausado, los botones no hacen nada
        
        GameObject objetoActual = generador.GetObjetoActual();
        if (objetoActual != null) {
            if (objetoActual.CompareTag(tagBoton)) ProcesarAcierto(objetoActual);
            else ProcesarError();
        }
    }

    public void ProcesarAcierto(GameObject obj) {
        puntuacion += 10;
        aciertosActuales++;
        MostrarPopUp(imagenCorrecto);
        Destroy(obj);
        ActualizarInterfaz();

        // CONDICIÓN 1: Victoria al llegar a la meta
        if (aciertosActuales >= elementsPerLevel) TerminarJuego(true);
    }

    public void ProcesarError() {
        if (juegoPausado) return;
        vidasActuales--;
        MostrarPopUp(imagenIncorrecto);
        
        GameObject objetoParaBorrar = generador.GetObjetoActual();
        if (objetoParaBorrar != null) Destroy(objetoParaBorrar);
        
        ActualizarInterfaz();

        // CONDICIÓN 3: Derrota al perder todas las vidas
        if (vidasActuales <= 0) TerminarJuego(false);
    }

    void ActualizarInterfaz() {
        if (scoreText) scoreText.text = "Puntos: " + puntuacion.ToString();
        if (vidasText) vidasText.text = "Vidas:" + vidasActuales.ToString();
        if (aciertosText) aciertosText.text = aciertosActuales.ToString() + "/" + elementsPerLevel.ToString();
        
        if (timeText) {
            int min = Mathf.FloorToInt(tiempoRestante / 60);
            int seg = Mathf.FloorToInt(tiempoRestante % 60);
            timeText.text = string.Format("{0:00}:{1:00}", min, seg);
        }
    }

    public void TerminarJuego(bool victoria) {
        if (juegoPausado) return;
        
        juegoPausado = true;
        Time.timeScale = 0f; // Congela el juego al terminar la partida

        if (generador != null) generador.DetenerGeneracion();

        GameObject objetoEnVuelo = generador.GetObjetoActual();
        if (objetoEnVuelo != null) Destroy(objetoEnVuelo);

        GameObject panelFinal = victoria ? levelCompletePanel : gameOverPanel;
        
        if (panelFinal) {
            panelFinal.SetActive(true);
            if(finalScoreText) finalScoreText.text = puntuacion.ToString();
            if (finalAciertosText) finalAciertosText.text = aciertosActuales.ToString() + "/" + elementsPerLevel.ToString();
            float tiempoUsado = timeLimit - tiempoRestante;
            if(finalTimeText) finalTimeText.text = tiempoUsado.ToString("F0") + "s";
        }
    }

    // ==========================================
    // --- FUNCIONES DE PAUSA Y MENÚS ---
    // ==========================================

    public void PausarJuego() {
        juegoPausado = true;
        Time.timeScale = 0f; // Congela el tiempo (los residuos dejan de caer)
        if(pausePanel) pausePanel.SetActive(true); // Muestra el panel
    }

    public void ReanudarJuego() {
        juegoPausado = false;
        Time.timeScale = 1f; // Descongela el tiempo
        if(pausePanel) pausePanel.SetActive(false); // Oculta el panel
    }

    public void ReiniciarJuego() {
        Time.timeScale = 1f; // Vital: Descongelar antes de recargar la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Mantenemos esta por si ya la habías conectado a algún otro botón viejo
    public void RestartGame() {
        ReiniciarJuego(); 
    }

    // ==========================================
    // --- FUNCIONES DE FEEDBACK (POP-UPS) ---
    // ==========================================

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
}