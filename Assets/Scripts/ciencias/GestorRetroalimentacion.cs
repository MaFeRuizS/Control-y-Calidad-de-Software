using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestorRetroalimentacion : MonoBehaviour
{
    [Header("Conexión de Textos")]
    public TextMeshProUGUI txtTituloFeedback; 
    public TextMeshProUGUI txtAciertosTotales; // <-- Aquí declaramos el texto
    public TextMeshProUGUI txtErrMetales;
    public TextMeshProUGUI txtErrOrganicos;
    public TextMeshProUGUI txtErrInorganicos;
    public TextMeshProUGUI txtErrReciclables;
    public TextMeshProUGUI txtConsejoFinal;

    [Header("Configuración del Avatar")]
    public Image imagenAvatar; 
    public Sprite avatarFeliz;   
    public Sprite avatarNeutral; 
    public Sprite avatarTriste;  

    [Header("Bancos de Mensajes")]
    public string[] bancoMetales;
    public string[] bancoOrganicos;
    public string[] bancoInorganicos;
    public string[] bancoReciclables;

    public void MostrarResultados(int puntuacionFinal, int aciertos, int errMet, int errOrg, int errInorg, int errRec)
    {
        gameObject.SetActive(true);

        // Actualizamos el número de aciertos en pantalla
        if (txtAciertosTotales) {
            txtAciertosTotales.text = aciertos.ToString();
        }

        // 1. Lógica del Avatar
        if (imagenAvatar != null) {
            if (puntuacionFinal > 99) imagenAvatar.sprite = avatarFeliz;
            else if (puntuacionFinal >= 51) imagenAvatar.sprite = avatarNeutral;
            else imagenAvatar.sprite = avatarTriste;
        }

        // 2. Actualizar Contadores
        if (txtErrMetales) txtErrMetales.text = errMet.ToString();
        if (txtErrOrganicos) txtErrOrganicos.text = errOrg.ToString();
        if (txtErrInorganicos) txtErrInorganicos.text = errInorg.ToString();
        if (txtErrReciclables) txtErrReciclables.text = errRec.ToString();

        // 3. Determinar Título y Consejo
        DeterminarConsejoFinal(errMet, errOrg, errInorg, errRec);
    }

    private void DeterminarConsejoFinal(int errMet, int errOrg, int errInorg, int errRec)
    {
        int maxErrores = Mathf.Max(errMet, errOrg, errInorg, errRec);
        
        if (maxErrores == 0) {
            if (txtTituloFeedback) txtTituloFeedback.text = "¡Partida Perfecta!";
            txtConsejoFinal.text = "¡Excelente trabajo! Has demostrado un dominio total en la clasificación de residuos.";
            return;
        }

        string[] bancoSeleccionado = null;
        string categoriaCritica = "";

        if (maxErrores == errMet) {
            categoriaCritica = "Metales";
            bancoSeleccionado = bancoMetales;
        }
        else if (maxErrores == errOrg) {
            categoriaCritica = "Orgánicos";
            bancoSeleccionado = bancoOrganicos;
        }
        else if (maxErrores == errInorg) {
            categoriaCritica = "Inorgánicos";
            bancoSeleccionado = bancoInorganicos;
        }
        else if (maxErrores == errRec) {
            categoriaCritica = "Reciclables";
            bancoSeleccionado = bancoReciclables;
        }

        if (txtTituloFeedback) {
            txtTituloFeedback.text = "Mmm... fíjate en: " + categoriaCritica;
        }

        if (bancoSeleccionado != null && bancoSeleccionado.Length > 0) {
            txtConsejoFinal.text = bancoSeleccionado[Random.Range(0, bancoSeleccionado.Length)];
        } else {
            txtConsejoFinal.text = "Recuerda siempre revisar las etiquetas de tus residuos antes de desecharlos.";
        }
    }
}