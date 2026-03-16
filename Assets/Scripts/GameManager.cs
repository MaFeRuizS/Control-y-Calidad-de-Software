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

    [Header("Conexiones")]
    public GeneradorInteligente generador; 
    public List<RectTransform> categoryButtons;

    [Header("UI Panels y Navegación Joystick")]
    public GameObject pausePanel;
    public GameObject primerBotonPausa;
    // --- NUEVAS VARIABLES PARA EL JOYSTICK ---
    public GameObject botonPausaHUD; // El botón de pausa que está en la pantalla principal jugando
    public GameObject primerBotonGameOver; // El botón "Reiniciar" de tu panel de Game Over
    public GameObject primerBotonVictoria;
    
    // --- VARIABLES PRIVADAS 
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
        Time.timeScale = 1f; 

        ActualizarInterfaz();
        
        if(panelFeedback) panelFeedback.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
        if(pausePanel) pausePanel.SetActive(false); 

        // --- MAGIA DEL JOYSTICK: Al iniciar el juego, seleccionamos el botón de pausa de la pantalla principal ---
        if(botonPausaHUD) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(botonPausaHUD);
        }
    }

    void Update() {
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

        if (aciertosActuales >= elementsPerLevel) TerminarJuego(true);
    }

    public void ProcesarError() {
        if (juegoPausado) return;
        vidasActuales--;
        MostrarPopUp(imagenIncorrecto);
        
        GameObject objetoParaBorrar = generador.GetObjetoActual();
        if (objetoParaBorrar != null) Destroy(objetoParaBorrar);
        
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

            // --- MAGIA DEL JOYSTICK: Pasamos el control al panel de Game Over o Victoria ---
            GameObject botonFinal = victoria ? primerBotonVictoria : primerBotonGameOver;
            if(botonFinal) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(botonFinal);
            }
        }
    }

    public void PausarJuego() {
        juegoPausado = true;
        Time.timeScale = 0f; 
        if(pausePanel) pausePanel.SetActive(true); 

        // --- MAGIA DEL JOYSTICK: Pasamos el control al menú de pausa ---
        if(primerBotonPausa) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(primerBotonPausa);
        }
    }

    public void ReanudarJuego() {
        juegoPausado = false;
        Time.timeScale = 1f; 
        if(pausePanel) pausePanel.SetActive(false); 

        // --- MAGIA DEL JOYSTICK: Devolvemos el control al botón de la pantalla principal ---
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