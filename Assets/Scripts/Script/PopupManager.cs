using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PopupManager : MonoBehaviour
{
    public GameObject popupReciclaje;
    public GameObject popupMates;
    public GameObject popupEspanol;

    [Header("Botones 'X'")]
    public GameObject botonX_Reciclaje;
    public GameObject botonX_Mates;
    public GameObject botonX_Espanol;

    [Header("Botones de Origen")]
    public GameObject botonAbrirReciclaje;
    public GameObject botonAbrirMates;
    public GameObject botonAbrirEspanol;

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
        if (botonX_Reciclaje != null) EventSystem.current.SetSelectedGameObject(botonX_Reciclaje);
    }

    public void AbrirMates()
    {
        CerrarTodo();
        popupMates.SetActive(true);
        if (botonX_Mates != null) EventSystem.current.SetSelectedGameObject(botonX_Mates);
    }

    public void AbrirEspanol()
    {
        CerrarTodo();
        popupEspanol.SetActive(true);
        if (botonX_Espanol != null) EventSystem.current.SetSelectedGameObject(botonX_Espanol);
    }

    public void CerrarTodo()
    {
        StartCoroutine(CerrarPopup(popupReciclaje, animReciclaje, botonAbrirReciclaje));
        StartCoroutine(CerrarPopup(popupMates, animMates, botonAbrirMates));
        StartCoroutine(CerrarPopup(popupEspanol, animEspanol, botonAbrirEspanol));
    }

    IEnumerator CerrarPopup(GameObject popup, Animator anim, GameObject botonRegreso)
    {
        if (popup.activeSelf)
        {
            anim.SetTrigger("Cerrar");
            if (botonRegreso != null)
            {
                EventSystem.current.SetSelectedGameObject(botonRegreso);
            }
            yield return new WaitForSeconds(0.25f);
            popup.SetActive(false);
        }
    }
}