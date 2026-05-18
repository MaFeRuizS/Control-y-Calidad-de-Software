using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class ControladorPartida : MonoBehaviour
{
    [Header("UI de la Partida")]
    public GameObject panelContador;
    public TextMeshProUGUI textoContador;
    public GameObject panelPausa;
    
    [Header("Navegación de Pausa")]
    public GameObject primerBotonPausa;

    [HideInInspector]
    public bool juegoIniciado = false;

    private bool enPausa = false;
    private int estadoAnteriorBtn = 0;

    void Start()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (panelPausa) panelPausa.SetActive(false);
        if (panelContador) panelContador.SetActive(true);

        Time.timeScale = 0f; 
        StartCoroutine(RutinaContador());
    }

    IEnumerator RutinaContador()
    {
        textoContador.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        
        textoContador.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        
        textoContador.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        
        textoContador.text = "¡A CLASIFICAR!";
        yield return new WaitForSecondsRealtime(0.5f);

        if (panelContador) panelContador.SetActive(false);
        if (panelPausa) panelPausa.SetActive(false);
        
        Time.timeScale = 1f; 
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.juegoPausado = false;
        }
        
        juegoIniciado = true; 
    }

    void Update()
    {
        if (InputArcadeManager.Instance == null) return;

        int estadoActualBtn = InputArcadeManager.Instance.estadoActual.joyBtn;

        if (juegoIniciado)
        {
            if (estadoActualBtn == 1 && estadoAnteriorBtn == 0)
            {
                AlternarPausa();
            }
        }

        estadoAnteriorBtn = estadoActualBtn;
    }

    public void AlternarPausa()
    {
        enPausa = !enPausa;
        
        if (panelPausa) panelPausa.SetActive(enPausa);
        
        Time.timeScale = enPausa ? 0f : 1f;

        if (enPausa && primerBotonPausa != null)
        {
            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(primerBotonPausa);
        }
    }
}