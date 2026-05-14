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
    public GameObject pausePanel;
    public GameObject primerBotonPausa;
    // --- NUEVAS VARIABLES PARA EL JOYSTICK ---
    public GameObject botonPausaHUD;
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
    
    // --- NUEVA VARIABLE PARA LA MECÁNICA DE OBJETOS ---
    private int objetosJugados = 0; 

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        vidasActuales = maxLives;
        tiempoRestante = timeLimit;
        juegoPausado = false;
        Time.timeScale = 1f; 
        objetosJugados = 0; // Reiniciamos los objetos al empezar

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
            if (objetoActual.CompareTag(tagBoton)) {
                ProcesarAcierto(objetoActual);
            }
            else {
                // Llamamos a ProcesarError vacío (asume que fue por clasificar mal)
                ProcesarError();
            }
        }
    }

    public void ProcesarAcierto(GameObject obj) {
        puntuacion += 10;
        aciertosActuales++;
        objetosJugados++; // Sumamos a los objetos que ya pasaron
        
        MostrarPopUp(imagenCorrecto);
        Destroy(obj);
        ActualizarInterfaz();

        // El juego termina si ya jugaron todos los elementos
        if (objetosJugados >= elementsPerLevel) TerminarJuego(true);
    }

    public void ProcesarError(bool fuePorCaida = false) {
        if (juegoPausado) return;

        GameObject objetoActual = generador.GetObjetoActual();
        if (objetoActual != null) {
            RegistrarError(objetoActual.tag); 
        }
        
        if (fuePorCaida) {
            vidasActuales--; // Penalización por inactividad
        } else {
            puntuacion -= 5; // Penalización por error al clasificar
            if (puntuacion < 0) puntuacion = 0; // Evita puntaje negativo
        }
        
        objetosJugados++; 
        
        MostrarPopUp(imagenIncorrecto);
        
        if (objetoActual != null) Destroy(objetoActual);
        
        ActualizarInterfaz();

        if (vidasActuales <= 0) {
            TerminarJuego(false); 
        } else if (objetosJugados >= elementsPerLevel) {
            TerminarJuego(false); // Ponemos 'false' para forzar que abra la fachada de retroalimentación
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

        // --- UNIFICACIÓN DE PANELES ---
        if (gameOverPanel) {
            gameOverPanel.SetActive(true);
            
            // Llenamos los datos del reporte final
            if(finalScoreText) finalScoreText.text = puntuacion.ToString();
            if (finalAciertosText) finalAciertosText.text = aciertosActuales.ToString();
            if(finalTimeText) finalTimeText.text = tiempoUsado.ToString("F0") + "s";

            // Joystick al botón de reintentar
            if(primerBotonGameOver) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(primerBotonGameOver);
            }
        }

        // Siempre apagamos el de victoria por si acaso
        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        // ¡LA FACHADA SIEMPRE SE ACTIVA! 
        panelRetroalimentacion.MostrarResultados(puntuacion, aciertosActuales, erroresMetales, erroresOrganicos, erroresInorganicos, erroresReciclables);    }

    public void PausarJuego() {
        // 1.Si ya perdimos o ganamos, abortar misión inmediatamente
        if ((gameOverPanel != null && gameOverPanel.activeSelf) || (levelCompletePanel != null && levelCompletePanel.activeSelf)) 
        {
            return;
        }

        // 2. CONGELAR VARIABLES Y TIEMPO
        juegoPausado = true;
        Time.timeScale = 0f;

        if(pausePanel != null) 
        {
            pausePanel.SetActive(true);
        }

        // SELECCIONAR EL BOTÓN PARA EL MANDO/TECLADO
        if(primerBotonPausa != null) 
        {
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

   public void RegistrarError(string categoriaBasura)
    {
        // Esta línea nos dirá exactamente qué palabra está leyendo Unity
        Debug.Log("Intentando registrar error para el tag: [" + categoriaBasura + "]"); 

        if (categoriaBasura == "Metales") {
            erroresMetales++;
            Debug.Log("¡Éxito! Sumando a Metales. Total ahora: " + erroresMetales);
        }
        else if (categoriaBasura == "Organicos" || categoriaBasura == "Organico" || categoriaBasura == "Orgánico") {
            erroresOrganicos++;
            Debug.Log("¡Éxito! Sumando a Orgánicos. Total ahora: " + erroresOrganicos);
        }
        else if (categoriaBasura == "Inorganicos" || categoriaBasura == "Inorganico" || categoriaBasura == "Inorgánico") {
            erroresInorganicos++;
            Debug.Log("¡Éxito! Sumando a Inorgánicos. Total ahora: " + erroresInorganicos);
        }
        else if (categoriaBasura == "Reciclables" || categoriaBasura == "Reciclable") {
            erroresReciclables++;
            Debug.Log("¡Éxito! Sumando a Reciclables. Total ahora: " + erroresReciclables);
        }
        else {
            Debug.LogWarning("¡ALERTA! El tag '" + categoriaBasura + "' no pertenece a ninguna categoría de tu código.");
        }
    }
}