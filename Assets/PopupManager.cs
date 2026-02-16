using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject popupReciclaje;
    public GameObject popupMatematicas;
    public GameObject popupEspanol;

    public void AbrirReciclaje()
    {
        CerrarTodo();
        popupReciclaje.SetActive(true);
    }

    public void AbrirMatematicas()
    {
        CerrarTodo();
        popupMatematicas.SetActive(true);
    }

    public void AbrirEspanol()
    {
        CerrarTodo();
        popupEspanol.SetActive(true);
    }

    public void CerrarTodo()
    {
        if (popupReciclaje != null) popupReciclaje.SetActive(false);
        if (popupMatematicas != null) popupMatematicas.SetActive(false);
        if (popupEspanol != null) popupEspanol.SetActive(false);
    }
}
