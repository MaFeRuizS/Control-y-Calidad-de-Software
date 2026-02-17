using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorInteligente : MonoBehaviour
{
    [Header("Configuración de Spawning")]
    public List<GameObject> prefabsResiduos; 
    public float tiempoEntreSpawns = 3f;
    public Transform puntoDeSpawn;

    [Header("Área de Caída")]
    public float rangoX = 8f; // Ajustar según el ancho de tu Canvas

    private List<GameObject> bolsaDeTrabajo = new List<GameObject>();
    private GameObject objetoActual;
    private bool puedeGenerar = true;

    void Start() {
        if (puntoDeSpawn == null) puntoDeSpawn = transform;
        PrepararBolsa();
        StartCoroutine(RutinaGeneracion());
    }

    void PrepararBolsa() {
        bolsaDeTrabajo = new List<GameObject>(prefabsResiduos);
        // Mezclado Fisher-Yates para evitar repeticiones
        for (int i = 0; i < bolsaDeTrabajo.Count; i++) {
            GameObject temp = bolsaDeTrabajo[i];
            int randomIndex = Random.Range(i, bolsaDeTrabajo.Count);
            bolsaDeTrabajo[i] = bolsaDeTrabajo[randomIndex];
            bolsaDeTrabajo[randomIndex] = temp;
        }
    }

    IEnumerator RutinaGeneracion() {
        while (puedeGenerar) {
            if (objetoActual == null) {
                if (bolsaDeTrabajo.Count == 0) PrepararBolsa();

                // Cálculo de posición aleatoria en el eje X
                float spawnX = Random.Range(-rangoX, rangoX);
                Vector3 spawnPos = new Vector3(puntoDeSpawn.position.x + spawnX, puntoDeSpawn.position.y, puntoDeSpawn.position.z);

                GameObject prefab = bolsaDeTrabajo[0];
                bolsaDeTrabajo.RemoveAt(0);
                objetoActual = Instantiate(prefab, spawnPos, Quaternion.identity);
            }
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    public void DetenerGeneracion() => puedeGenerar = false;
    public GameObject GetObjetoActual() => objetoActual;

    void OnDrawGizmos() {
        if (puntoDeSpawn != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(puntoDeSpawn.position, new Vector3(rangoX * 2, 0.5f, 0.5f));
        }
    }
}