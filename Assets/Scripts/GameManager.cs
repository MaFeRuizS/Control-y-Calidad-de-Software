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

    private int puntuacion = 0;
    private int aciertosActuales = 0;
    private int vidasActuales;
    private float tiempoRestante;
    private bool juegoPausado = false;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        vidasActuales = maxLives;
        tiempoRestante = timeLimit;
        ActualizarInterfaz();
        
        if(panelFeedback) panelFeedback.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
    }

    void Update() {
        if (!juegoPausado) ManejarCronometro();
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

    // ARREGLO CS1061: Proporciona la velocidad de caída a MathElement
    public float GetFallSpeed() => elementFallSpeed;

    // ARREGLO CS1503: Proporciona soporte para botones que envían índice (int)
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

    // ARREGLO CS0029: Conversión correcta de números a texto y formato 0/15
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

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}