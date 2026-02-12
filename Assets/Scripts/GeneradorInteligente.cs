using UnityEngine;

public class GeneradorInteligente : MonoBehaviour
{
    [Header("Configuración de Prefabs")]
    // Arrastra aquí tus 5 o 6 residuos desde la carpeta de Prefabs
    public GameObject[] listaDePrefabs; 

    [Header("Configuración del Canvas")]
    // Arrastra aquí tu objeto Canvas para que los residuos sean visibles
    public Transform parentCanvas;      

    [Header("Ajustes de la Lluvia")]
    // Define qué tan ancho es el pasillo por donde caen los objetos
    public float rangoDeAncho = 500f; 

    // Referencia interna para controlar que solo caiga uno a la vez
    private GameObject objetoActual;    

    void Update()
    {
        // El corazón de tu mecánica: si no hay objeto en pantalla, creamos uno
        if (objetoActual == null)
        {
            SpawnNuevoObjeto();
        }
    }

    void SpawnNuevoObjeto()
    {
        // Validación de seguridad: evita que el juego se rompa si la lista está vacía
        if (listaDePrefabs == null || listaDePrefabs.Length == 0)
        {
            Debug.LogWarning("¡Omar! Te falta llenar la lista de prefabs en el Inspector.");
            return;
        }

        // Elegimos un residuo al azar de tu lista de 5 o 6
        int indiceAleatorio = Random.Range(0, listaDePrefabs.Length);
        
        // Calculamos la posición X aleatoria centrada en el SpawnPoint
        float randomX = Random.Range(-rangoDeAncho, rangoDeAncho); 
        Vector3 posicion = new Vector3(transform.position.x + randomX, transform.position.y, 0);

        // Instanciamos el objeto y guardamos la referencia para el Update
        objetoActual = Instantiate(listaDePrefabs[indiceAleatorio], posicion, Quaternion.identity);
        
        // Lo emparentamos al Canvas para que respete las capas de la interfaz
        objetoActual.transform.SetParent(parentCanvas, false);
    }
}