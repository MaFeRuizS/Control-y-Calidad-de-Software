using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorMapa : MonoBehaviour
{
    [System.Serializable]
    public class ConfigNivel
    {
        public string nombreEscena;
        public Button botonUI;      
        public Image imagenIcono;
        public Sprite spriteDesbloqueado; 
        public Sprite spriteBloqueado;
    }

    [Header("Configuración de Niveles")]
    public ConfigNivel[] niveles;

    [Header("Estadísticas del Jugador")]
    public TextMeshProUGUI textoNivelesCompletados;
    public int totalDeNiveles = 3;

    void Start()
    {
        Time.timeScale = 1f;
        
        ActualizarProgresoMapa();
    }

    public void ActualizarProgresoMapa()
    {
        int nivelMaximoDesbloqueado = PlayerPrefs.GetInt("ProgresoNivelActual", 1);
        Debug.Log("EL MAPA ESTÁ LEYENDO EL PROGRESO: " + nivelMaximoDesbloqueado);

        for (int i = 0; i < niveles.Length; i++)
        {
            int numeroNivel = i + 1;

            if (numeroNivel <= nivelMaximoDesbloqueado)
            {
                if (niveles[i].botonUI != null) niveles[i].botonUI.interactable = true;
                if (niveles[i].imagenIcono != null && niveles[i].spriteDesbloqueado != null)
                {
                    niveles[i].imagenIcono.sprite = niveles[i].spriteDesbloqueado;
                }
            }
            else
            {
                if (niveles[i].botonUI != null) niveles[i].botonUI.interactable = false;
                if (niveles[i].imagenIcono != null && niveles[i].spriteBloqueado != null)
                {
                    niveles[i].imagenIcono.sprite = niveles[i].spriteBloqueado;
                }
            }
        } 
        if (textoNivelesCompletados != null)
        {
            int nivelesCompletados = nivelMaximoDesbloqueado - 1; 

            if (nivelesCompletados > totalDeNiveles) 
            {
                nivelesCompletados = totalDeNiveles;
            }

            textoNivelesCompletados.text = nivelesCompletados + "/" + totalDeNiveles;
        }
    } 

    public void CargarNivel(string nombreEscena)
    {
        //Debug.Log("¡Botón presionado! Intentando cargar la escena: " + nombreEscena);
        SceneManager.LoadScene(nombreEscena);
    }

    [ContextMenu("Resetear Progreso")]
    public void ResetearProgreso()
    {
        PlayerPrefs.SetInt("ProgresoNivelActual", 1);
        PlayerPrefs.Save();
        ActualizarProgresoMapa();
        Debug.Log("Progreso reiniciado: Solo el Nivel 1 está disponible.");
    }
}