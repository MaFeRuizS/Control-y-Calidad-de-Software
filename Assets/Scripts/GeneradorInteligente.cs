using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorInteligente : MonoBehaviour
{
    [Header("Configuración de Spawning")]
    [Tooltip("Lista de prefabs de residuos que aparecerán en el juego.")]
    public List<GameObject> prefabsResiduos;
    
    [Tooltip("Tiempo base entre cada aparición.")]
    public float tiempoEntreSpawns = 3f;
    
    [Tooltip("Punto exacto donde aparecerán los objetos.")]
    public Transform puntoDeSpawn;

    private GameObject objetoActual;
    private bool puedeGenerar = true;

    void Start()
    {
        // Validación de seguridad: Si no se asigna un punto, se usa la posición del script
        if (puntoDeSpawn == null)
        {
            Debug.LogWarning("Punto de Spawn no asignado. Usando posición de: " + gameObject.name);
            puntoDeSpawn = transform;
        }

        // Validación de lista vacía
        if (prefabsResiduos == null || prefabsResiduos.Count == 0)
        {
            Debug.LogError("¡La lista de prefabs está vacía! Añade objetos en el Inspector.");
            puedeGenerar = false;
            return;
        }

        StartCoroutine(RutinaGeneracion());
    }

    IEnumerator RutinaGeneracion()
    {
        while (puedeGenerar)
        {
            // Solo generamos si el objeto anterior ya no está (fue recolectado o destruido)
            if (objetoActual == null)
            {
                GenerarResiduo();
            }

            // Agregamos una pequeña variación de tiempo para que no sea predecible
            float variacion = Random.Range(-0.5f, 0.5f);
            yield return new WaitForSeconds(Mathf.Max(0.1f, tiempoEntreSpawns + variacion));
        }
    }

    void GenerarResiduo()
    {
        int indiceAleatorio = Random.Range(0, prefabsResiduos.Count);
        
        // Instancia el objeto y guarda la referencia en 'objetoActual'
        objetoActual = Instantiate(prefabsResiduos[indiceAleatorio], puntoDeSpawn.position, Quaternion.identity);
    }

    // Método para ser llamado por el GameManager cuando el jugador pierde o gana
    public void DetenerGeneracion()
    {
        puedeGenerar = false;
        StopAllCoroutines();
        Debug.Log("<color=yellow>Sistema de generación detenido.</color>");
    }

    public GameObject GetObjetoActual()
    {
        return objetoActual;
    }
}