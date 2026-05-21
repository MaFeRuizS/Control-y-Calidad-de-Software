using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManagerMatematicas : MonoBehaviour
{
    public static GameManagerMatematicas Instance;

    [Header("Configuración del Juego")]
    public float elementFallSpeed = 2f;
    public int maxLives = 3;
    public int elementsPerLevel = 15; 
    public float timeLimit = 135f; 

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

    [Header("Panel Level Complete")]
    public GameObject panelLevelComplete;
    public TextMeshProUGUI PuntajeFinal;
    public TextMeshProUGUI AciertosFinal;
    public TextMeshProUGUI TiempoFinal;

    [Header("UI Memorización (Pre-Juego)")]
    public GameObject panelMemorizacion; // Arrastra aquí tu panel de los 5 segundos

    [Header("Conexiones Matemáticas")]
    public GestorPatrones gestorPatrones; 
    public GeneradorMatematicas generador; 
    public List<RectTransform> categoryButtons;

    [Header("UI Panels y Navegación Joystick")]
    public GameObject pausePanel;
    public GameObject primerBotonPausa;
    public GameObject botonPausaHUD; 
    public GameObject primerBotonGameOver; 
    public GameObject primerBotonVictoria;

    [Header("Configuración de Progresión")]
    public int numeroDeEsteNivel = 2;
    
    // --- VARIABLES PRIVADAS 
    private bool juegoPausado = false;
    private bool preparandose = true; // Variable para los 5 segundos iniciales
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
        preparandose = true; // Empezamos en modo memorización
        Time.timeScale = 1f; 

        ActualizarInterfaz();
        
        if(panelFeedback) panelFeedback.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
        if(pausePanel) pausePanel.SetActive(false); 

        if(botonPausaHUD) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(botonPausaHUD);
        }

        // Apagamos la lluvia temporalmente e iniciamos los 5 segundos
        if(generador != null) generador.enabled = false;
        StartCoroutine(SecuenciaDeMemorizacion());
    }

    // --- CORUTINA DE LOS 5 SEGUNDOS DE MEMORIZACIÓN ---
    IEnumerator SecuenciaDeMemorizacion() {
        if(panelMemorizacion) panelMemorizacion.SetActive(true);
        
        yield return new WaitForSeconds(5f);
        
        if(panelMemorizacion) panelMemorizacion.SetActive(false);
        preparandose = false;
        if(generador != null) generador.enabled = true; // Que empiece a llover
    }

    void Update() {
        // Solo corre el tiempo si el juego NO está pausado y NO estamos en los 5 segundos iniciales
        if (!juegoPausado && !preparandose) {
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
        // Ignoramos clics si estamos en pausa o en los 5 segundos de memorización
        if (juegoPausado || preparandose) return; 
        
        string tagNecesario = gestorPatrones.tagCorrectoParaAtrapar;
        GameObject objetoCorrectoEnPantalla = GameObject.FindGameObjectWithTag(tagNecesario);
        bool presionoElBotonCorrecto = tagNecesario.Contains(tagBoton);

        Debug.Log("Juego pide: " + tagNecesario);
        Debug.Log("Tu botón mandó la palabra: " + tagBoton);
        Debug.Log("¿La figura está en pantalla AHORA MISMO?: " + (objetoCorrectoEnPantalla != null));

        if (objetoCorrectoEnPantalla != null && presionoElBotonCorrecto) {
            ProcesarAcierto(objetoCorrectoEnPantalla);
            if (gestorPatrones != null) gestorPatrones.GenerarNuevoPatron(); 
            
            // --- AQUÍ ESTÁ EL RESET DEL SISTEMA DE 5 FIGURAS ---
            if (generador != null) generador.ResetearContador(); 
        } 
        else {
            ProcesarError();
        }
    }

    public void ProcesarAcierto(GameObject obj) {
        puntuacion += 10;
        aciertosActuales++;
        MostrarPopUp(imagenCorrecto);
        Destroy(obj);
        ActualizarInterfaz();

        if (aciertosActuales >= elementsPerLevel) TerminarJuego(true);
    }

    public void ProcesarError() {
        if (juegoPausado) return;
        vidasActuales--;
        MostrarPopUp(imagenIncorrecto);
        ActualizarInterfaz();

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
        Time.timeScale = 0f; 

        if (generador != null) generador.enabled = false; 

        float tiempoUsado = timeLimit - tiempoRestante;

        if (victoria) {
            int progresoActual = PlayerPrefs.GetInt("ProgresoNivelActual", 1);
            
            if (progresoActual == numeroDeEsteNivel) 
            {
                PlayerPrefs.SetInt("ProgresoNivelActual", numeroDeEsteNivel + 1); 
                PlayerPrefs.Save(); 
                Debug.Log("¡Matemáticas superado! Desbloqueado el Nivel " + (numeroDeEsteNivel + 1));
            }

            if (panelLevelComplete) panelLevelComplete.SetActive(true);
            
            if (PuntajeFinal) PuntajeFinal.text = puntuacion.ToString();
            if (AciertosFinal) AciertosFinal.text = aciertosActuales.ToString() + "/" + elementsPerLevel.ToString();
            if (TiempoFinal) TiempoFinal.text = tiempoUsado.ToString("F0") + "s";

            if (primerBotonVictoria) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(primerBotonVictoria);
            }
        } 
        else {
            if (gameOverPanel) gameOverPanel.SetActive(true);
            
            if (finalScoreText) finalScoreText.text = puntuacion.ToString();
            if (finalAciertosText) finalAciertosText.text = aciertosActuales.ToString() + "/" + elementsPerLevel.ToString();
            if (finalTimeText) finalTimeText.text = tiempoUsado.ToString("F0") + "s";

            if (primerBotonGameOver) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(primerBotonGameOver);
            }
        }
    }

    public void PausarJuego() {
        juegoPausado = true;
        Time.timeScale = 0f; 
        if(pausePanel) pausePanel.SetActive(true); 

        if(primerBotonPausa) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(primerBotonPausa);
        }
    }

    public void ReanudarJuego() {
        juegoPausado = false;
        Time.timeScale = 1f; 
        if(pausePanel) pausePanel.SetActive(false); 

        if(botonPausaHUD) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(botonPausaHUD);
        }
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
}