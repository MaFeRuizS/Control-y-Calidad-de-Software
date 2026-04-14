using UnityEngine;
using UnityEngine.UI;

public class GestorPatrones : MonoBehaviour
{
    // --- ESTA ES LA LÍNEA QUE ME FALTÓ DARTE ---
    [Header("Conexiones UI (Tus casillas blancas)")]
    public Image[] casillasUI; 

    [Header("Sprites Figuras ORIGINALES")]
    public Sprite spriteEstrella;
    public Sprite spriteCirculo;
    public Sprite spriteTriangulo;
    public Sprite spriteRectangulo;

    [Header("Sprites Figuras NUEVAS (Colores)")]
    public Sprite spriteCirculoRosa;
    public Sprite spriteTrianguloTurquesa;
    public Sprite spriteEstrellaNaranja;
    public Sprite spriteRectanguloMorado;

    [Header("Sprite Oculto")]
    public Sprite spriteInterrogacion;

    [Header("Datos del Patrón Actual")]
    public int[] idsPatronActual; 
    public int indiceAAtrapar;
    public string tagCorrectoParaAtrapar;

    void Start()
    {
        if (casillasUI.Length > 0)
        {
            idsPatronActual = new int[casillasUI.Length];
            GenerarNuevoPatron();
        }
        else
        {
            Debug.LogError("Por favor, asigna las imágenes de las casillas UI en el Inspector.");
        }
    }

    public void GenerarNuevoPatron()
    {
        // 1. Elegimos 3 figuras "Base" distintas al azar para que los patrones varíen
        int figuraA = Random.Range(0, 8);
        int figuraB = Random.Range(0, 8);
        while (figuraB == figuraA) figuraB = Random.Range(0, 8); // Evitamos que A y B sean iguales
        
        int figuraC = Random.Range(0, 8);
        while (figuraC == figuraA || figuraC == figuraB) figuraC = Random.Range(0, 8); // Evitamos repetidas

        // 2. Decidimos qué tipo de regla lógica usaremos en este turno (0, 1 o 2)
        int tipoDeRegla = Random.Range(0, 3);

        // 3. Llenamos el arreglo siguiendo la regla estricta
        for (int i = 0; i < casillasUI.Length; i++)
        {
            if (tipoDeRegla == 0) 
            {
                // Regla Alterna (A, B, A, B, A, B)
                idsPatronActual[i] = (i % 2 == 0) ? figuraA : figuraB;
            }
            else if (tipoDeRegla == 1) 
            {
                // Regla en Pares (A, A, B, B, A, A)
                idsPatronActual[i] = ((i / 2) % 2 == 0) ? figuraA : figuraB;
            }
            else 
            {
                // Regla de Tres (A, B, C, A, B, C)
                if (i % 3 == 0) idsPatronActual[i] = figuraA;
                else if (i % 3 == 1) idsPatronActual[i] = figuraB;
                else idsPatronActual[i] = figuraC;
            }
        }

        
        // Dibujamos las figuras, EXCEPTO LA ÚLTIMA
        int ultimaCasilla = casillasUI.Length - 1;

        for (int i = 0; i < ultimaCasilla; i++)
        {
            casillasUI[i].sprite = ObtenerSprite(idsPatronActual[i]);
            casillasUI[i].color = Color.white; 
            casillasUI[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // A la ÚLTIMA casilla le ponemos el signo de interrogación
        casillasUI[ultimaCasilla].sprite = spriteInterrogacion;
        casillasUI[ultimaCasilla].color = Color.white;

        // Lógica de tamaño y Tag final
        indiceAAtrapar = idsPatronActual[ultimaCasilla];
        int tamañoRandom = Random.Range(0, 2); 
        string sufijoTamaño = tamañoRandom == 0 ? "Grande" : "Pequeno";

        if (tamañoRandom == 0) {
            casillasUI[ultimaCasilla].transform.localScale = new Vector3(1f, 1f, 1f); 
        } else {
            casillasUI[ultimaCasilla].transform.localScale = new Vector3(0.5f, 0.5f, 1f); 
        }

        tagCorrectoParaAtrapar = ObtenerNombre(indiceAAtrapar) + sufijoTamaño;
        Debug.Log("REGLA USADA: " + tipoDeRegla + " | El jugador debe adivinar: " + tagCorrectoParaAtrapar);
    }

    Sprite ObtenerSprite(int id)
    {
        switch (id)
        {
            case 0: return spriteEstrella;
            case 1: return spriteCirculo;
            case 2: return spriteTriangulo;
            case 3: return spriteRectangulo;
            case 4: return spriteEstrellaNaranja;
            case 5: return spriteCirculoRosa;
            case 6: return spriteTrianguloTurquesa;
            case 7: return spriteRectanguloMorado;
            default: return spriteEstrella; 
        }
    }

    string ObtenerNombre(int id)
    {
        switch (id)
        {
            case 0: return "Estrella";
            case 1: return "Circulo";
            case 2: return "Triangulo";
            case 3: return "Rectangulo";
            case 4: return "EstrellaNaranja";
            case 5: return "CirculoRosa";
            case 6: return "TrianguloTurquesa";
            case 7: return "RectanguloMorado";
            default: return "Estrella";
        }
    }
}