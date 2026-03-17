using UnityEngine;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public GameObject popupReciclaje;
    public GameObject popupMates;
    public GameObject popupEspanol;

    public Animator animReciclaje;
    public Animator animMates;
    public Animator animEspanol;

    void Start()
    {
        popupReciclaje.SetActive(false);
        popupMates.SetActive(false);
        popupEspanol.SetActive(false);
    }

    public void AbrirReciclaje()
    {
        CerrarTodo();
        popupReciclaje.SetActive(true);
    }

    public void AbrirMates()
    {
        CerrarTodo();
        popupMates.SetActive(true);
    }

    public void AbrirEspanol()
    {
        CerrarTodo();
        popupEspanol.SetActive(true);
    }

    public void CerrarTodo()
    {
        StartCoroutine(CerrarPopup(popupReciclaje, animReciclaje));
        StartCoroutine(CerrarPopup(popupMates, animMates));
        StartCoroutine(CerrarPopup(popupEspanol, animEspanol));
    }

    IEnumerator CerrarPopup(GameObject popup, Animator anim)
    {
        if (popup.activeSelf)
        {
            anim.SetTrigger("Cerrar");
            yield return new WaitForSeconds(0.25f);
            popup.SetActive(false);
        }
    }
}