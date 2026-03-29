using UnityEngine;
using UnityEngine.UI;

public class ConectorNiveles : MonoBehaviour
{
    [Header("Asigna los niveles en orden")]
    public RectTransform[] niveles;

    [Header("Estilo de línea")]
    public Color colorLinea = new Color(1f, 1f, 1f, 0.3f);
    public float grosorLinea = 5f;

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        DibujarLineas();
    }

    void DibujarLineas()
    {
        // Conectar cada nivel con el siguiente
        for (int i = 0; i < niveles.Length - 1; i++)
        {
            if (niveles[i] != null && niveles[i + 1] != null)
            {
                CrearLinea(niveles[i], niveles[i + 1], i);
            }
        }
    }

    void CrearLinea(RectTransform desde, RectTransform hasta, int index)
    {
        // Crear objeto para la línea
        GameObject lineaObj = new GameObject("Linea_" + index);
        lineaObj.transform.SetParent(transform, false);

        // Moverla al fondo para que quede detrás de los niveles
        lineaObj.transform.SetAsFirstSibling();

        // Agregar componente Image
        Image lineaImg = lineaObj.AddComponent<Image>();
        lineaImg.color = colorLinea;

        // Calcular posición y rotación
        Vector2 posDesde = desde.anchoredPosition;
        Vector2 posHasta = hasta.anchoredPosition;

        // Punto medio entre los dos niveles
        Vector2 puntoMedio = (posDesde + posHasta) / 2f;

        // Distancia entre los dos niveles
        float distancia = Vector2.Distance(posDesde, posHasta);

        // Ángulo de la línea
        float angulo = Mathf.Atan2(
            posHasta.y - posDesde.y,
            posHasta.x - posDesde.x
        ) * Mathf.Rad2Deg;

        // Aplicar al RectTransform
        RectTransform lineaRect = lineaObj.GetComponent<RectTransform>();
        lineaRect.anchoredPosition = puntoMedio;
        lineaRect.sizeDelta = new Vector2(distancia, grosorLinea);
        lineaRect.rotation = Quaternion.Euler(0, 0, angulo);
        lineaRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineaRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineaRect.pivot = new Vector2(0.5f, 0.5f);
    }
}