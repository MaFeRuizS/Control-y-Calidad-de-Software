using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAnimacion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 escalaOriginal;
    Vector3 escalaObjetivo;

    public float escalaHover = 1.05f;
    public float velocidad = 10f;

    void Start()
    {
        escalaOriginal = transform.localScale;
        escalaObjetivo = escalaOriginal;
    }

    void Update()
    {
        // movimiento suave hacia la escala objetivo
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaObjetivo,
            Time.deltaTime * velocidad
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaObjetivo = escalaOriginal * escalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaObjetivo = escalaOriginal;
    }
}