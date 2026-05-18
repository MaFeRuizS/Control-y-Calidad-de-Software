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

    private bool juegoIniciado = false;
    private bool enPausa = false;
    private int estadoAnteriorBtn = 0;

    void Start()
    {
        panelPausa.SetActive(false);
        panelContador.SetActive(true);

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

        panelContador.SetActive(false);
        Time.timeScale = 1f; 
        
        juegoIniciado = true; 
    }

    void Update()
    {
        if (!juegoIniciado) return; 

        if (InputArcadeManager.Instance != null)
        {
            int estadoActualBtn = InputArcadeManager.Instance.estadoActual.joyBtn;

            if (estadoActualBtn == 1 && estadoAnteriorBtn == 0)
            {
                AlternarPausa();
            }
            estadoAnteriorBtn = estadoActualBtn;
        }
    }

    public void AlternarPausa()
    {
        enPausa = !enPausa;
        
        panelPausa.SetActive(enPausa);
        
        Time.timeScale = enPausa ? 0f : 1f;

        if (enPausa && primerBotonPausa != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(primerBotonPausa);
        }
    }
}