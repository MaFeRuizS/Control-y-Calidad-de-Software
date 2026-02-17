using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorInteligente : MonoBehaviour
{
    [Header("Configuración de Spawning")]
    public List<GameObject> prefabsResiduos; // Tu lista original de 20
    public float tiempoEntreSpawns = 3f;
    public Transform puntoDeSpawn;

    // LISTA AUXILIAR PARA EL BARAJEO
    private List<GameObject> residuosDisponibles = new List<GameObject>();
    private GameObject objetoActual;
    private bool puedeGenerar = true;

    void Start()
    {
        if (puntoDeSpawn == null) puntoDeSpawn = transform;
        
        // Llenamos y barajamos la lista por primera vez
        PrepararNuevaTanda();
        StartCoroutine(RutinaGeneracion());
    }

    void PrepararNuevaTanda()
    {
        // Creamos una copia de la lista original
        residuosDisponibles = new List<GameObject>(prefabsResiduos);
        
        // Algoritmo de barajeo (Fisher-Yates)
        for (int i = 0; i < residuosDisponibles.Count; i++)
        {
            GameObject temp = residuosDisponibles[i];
            int randomIndex = Random.Range(i, residuosDisponibles.Count);
            residuosDisponibles[i] = residuosDisponibles[randomIndex];
            residuosDisponibles[randomIndex] = temp;
        }
        Debug.Log("Nueva tanda de " + residuosDisponibles.Count + " residuos barajada.");
    }

    IEnumerator RutinaGeneracion()
    {
        while (puedeGenerar)
        {
            if (objetoActual == null)
            {
                GenerarResiduo();
            }
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    void GenerarResiduo()
    {
        if (prefabsResiduos.Count == 0) return;

        // Si se nos acabaron los objetos de la tanda actual, barajamos de nuevo
        if (residuosDisponibles.Count == 0)
        {
            PrepararNuevaTanda();
        }

        // Tomamos el primer objeto de la lista barajada y lo eliminamos de "disponibles"
        GameObject prefabAElegir = residuosDisponibles[0];
        residuosDisponibles.RemoveAt(0);

        objetoActual = Instantiate(prefabAElegir, puntoDeSpawn.position, Quaternion.identity);
    }

    public void DetenerGeneracion() => puedeGenerar = false;
    public GameObject GetObjetoActual() => objetoActual;
}