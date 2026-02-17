using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float elementFallSpeed = 2f;
    public int maxLives = 3;
    public int elementsPerLevel = 15;
    public float timeLimit = 120f;

    [Header("UI References (Marcadores)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI vidasText; 
    public TextMeshProUGUI aciertosText; 

    [Header("UI Feedback (Aciertos/Errores)")]
    public GameObject panelFeedback;
    public Image imagenFondoPopup;
    public Sprite imagenCorrecto;
    public Sprite imagenIncorrecto;

    [Header("UI Final (Game Over)")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalAciertosText;
    public TextMeshProUGUI finalTimeText;

    [Header("Configuración del Repechaje")]
    public GameObject repechajePanel;
    public List<PreguntaRepechaje> bancoPreguntas;
    public TextMeshProUGUI textoEnunciado;
    public TextMeshProUGUI[] textosBotonesOpciones;

    [Header("Gameplay Connections")]
    public GeneradorInteligente generador; 
    public List<RectTransform> categoryButtons;

    private int puntuacion = 0;
    private int aciertosActuales = 0;
    private int vidasActuales;
    private float tiempoRestante;
    private bool juegoPausado = false;
    private bool repechajeUsado = false;
    private int indicePreguntaActual;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        vidasActuales = maxLives;
        tiempoRestante = timeLimit;
        ActualizarInterfaz();
        
        // Aseguramos que los paneles estén apagados al iniciar
        if(panelFeedback) panelFeedback.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
        if(levelCompletePanel) levelCompletePanel.SetActive(false);
        if(repechajePanel) repechajePanel.SetActive(false);
    }

    void Update() {
        if (!juegoPausado) ManejarCronometro();
    }

    void ManejarCronometro() {
        if (tiempoRestante > 0) {
            tiempoRestante -= Time.deltaTime;
            ActualizarInterfaz();
        } else {
            TerminarJuego(false);
        }
    }

    // --- FUNCIONES DE UTILIDAD ---
    public float GetFallSpeed() => elementFallSpeed;

    public void TerminarJuego() => TerminarJuego(false);

    public void TerminarJuego(bool victoria) {
        juegoPausado = true;
        
        if (generador != null) generador.DetenerGeneracion();

        // Limpieza de objetos que quedaron en el aire
        GameObject objetoEnVuelo = generador.GetObjetoActual();
        if (objetoEnVuelo != null) Destroy(objetoEnVuelo);

        // Seleccionamos qué panel mostrar
        GameObject panelFinal = victoria ? levelCompletePanel : gameOverPanel;
        
        if (panelFinal) {
            panelFinal.SetActive(true); // Aquí es donde se "dispara" el desorden si el diseño está mal

            // CORRECCIÓN: Usar .ToString() y asegurar que las referencias existan
            if(finalScoreText) finalScoreText.text = puntuacion.ToString();
            if(finalAciertosText) finalAciertosText.text = aciertosActuales.ToString() + "/" + elementsPerLevel.ToString();
            if(finalTimeText) {
                float tiempoUsado = timeLimit - tiempoRestante;
                finalTimeText.text = tiempoUsado.ToString("F0") + "s";
            }
        }
    }

    // --- LÓGICA DE CLASIFICACIÓN ---
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

        if (vidasActuales <= 0) {
            if (!repechajeUsado) ActivarRepechaje();
            else TerminarJuego(false);
        }
    }

    // --- SISTEMA DE REPECHAJE ---
    void ActivarRepechaje() {
        juegoPausado = true;
        repechajeUsado = true;
        if(repechajePanel) {
            repechajePanel.SetActive(true);
            MostrarPreguntaAleatoria();
        }
    }

    void MostrarPreguntaAleatoria() {
        if (bancoPreguntas.Count > 0) {
            indicePreguntaActual = Random.Range(0, bancoPreguntas.Count);
            PreguntaRepechaje p = bancoPreguntas[indicePreguntaActual];
            textoEnunciado.text = p.enunciado;
            for (int i = 0; i < 4; i++) textosBotonesOpciones[i].text = p.opciones[i];
        }
    }

    public void ComprobarRespuestaRepechaje(int indiceSeleccionado) {
        if (indiceSeleccionado == bancoPreguntas[indicePreguntaActual].indiceCorrecto) {
            vidasActuales = 1;
            if (tiempoRestante < 10f) tiempoRestante += 15f; // Bonus de tiempo por acertar
            repechajePanel.SetActive(false);
            juegoPausado = false;
            ActualizarInterfaz();
        } else {
            repechajePanel.SetActive(false);
            TerminarJuego(false);
        }
    }

    // --- UI Y FEEDBACK ---
    void ActualizarInterfaz() {
        // Usamos .ToString() para convertir el número a texto correctamente
        if (scoreText) scoreText.text = puntuacion.ToString();
        if (vidasText) vidasText.text = vidasActuales.ToString();
        if (aciertosText) aciertosText.text = aciertosActuales.ToString() + "/" + elementsPerLevel.ToString();
        
        if (timeText) {
            int min = Mathf.FloorToInt(tiempoRestante / 60);
            int seg = Mathf.FloorToInt(tiempoRestante % 60);
            timeText.text = string.Format("{0:00}:{1:00}", min, seg);
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

[System.Serializable]
public class PreguntaRepechaje {
    public string enunciado;
    public string[] opciones; 
    public int indiceCorrecto; 
}