using UnityEngine;

public class GeneradorMatematicas : MonoBehaviour
{
    [Header("Conexiones")]
    public GestorPatrones gestorPatrones; 

    [Header("Configuración de Caída")]
    public float rangoX = 7f; 
    public float alturaY = 5.5f; 

    [Header("Los 16 Prefabs de Matemáticas")]
    public GameObject[] prefabsFiguras; 

    [Header("Configuración de Tiempo")]
    public float tiempoEntreSpawns = 2.5f;
    private float tiempoSiguienteSpawn;

    // --- NUEVA LÓGICA DE CONTROL ---
    private int figurasSoltadasEnEstePatron = 0;
    private bool yaSalioLaCorrecta = false;

    void Update()
    {
        if (Time.time >= tiempoSiguienteSpawn)
        {
            SpawnearFigura();
            tiempoSiguienteSpawn = Time.time + tiempoEntreSpawns;
        }
    }

    void SpawnearFigura()
    {
        figurasSoltadasEnEstePatron++; // Contamos cada figura que cae
        GameObject prefabLanzar = null;
        string tagNecesario = gestorPatrones.tagCorrectoParaAtrapar;

        // REGLA DE ORO: Si es la quinta figura y no ha salido la correcta, la FORZAMOS
        if (figurasSoltadasEnEstePatron >= 5 && !yaSalioLaCorrecta)
        {
            prefabLanzar = BuscarPrefabPorTag(tagNecesario);
            yaSalioLaCorrecta = true; 
            Debug.Log("SISTEMA DE SEGURIDAD: Forzando salida de la respuesta correcta.");
        }
        else
        {
            // Probabilidad normal del 40%
            if (Random.Range(0f, 100f) < 40f) 
            {
                prefabLanzar = BuscarPrefabPorTag(tagNecesario);
                if(prefabLanzar != null) yaSalioLaCorrecta = true;
            }
        }

        // Si no se forzó y no salió por probabilidad, sale una al azar
        if (prefabLanzar == null) 
        {
            int indiceAleatorio = Random.Range(0, prefabsFiguras.Length);
            prefabLanzar = prefabsFiguras[indiceAleatorio];
        }

        // Instanciar
        float posicionXAleatoria = Random.Range(-rangoX, rangoX); 
        Vector3 posicionAparicion = new Vector3(posicionXAleatoria, alturaY, 0f);
        Instantiate(prefabLanzar, posicionAparicion, Quaternion.identity);
    }

    // Función auxiliar para no repetir código
    GameObject BuscarPrefabPorTag(string tag)
    {
        foreach(GameObject prefab in prefabsFiguras) 
        {
            if (prefab.CompareTag(tag)) return prefab;
        }
        return null;
    }

    // ESTA FUNCIÓN ES VITAL: El GameManager la llamará cada vez que cambie el patrón
    public void ResetearContador()
    {
        figurasSoltadasEnEstePatron = 0;
        yaSalioLaCorrecta = false;
    }
}