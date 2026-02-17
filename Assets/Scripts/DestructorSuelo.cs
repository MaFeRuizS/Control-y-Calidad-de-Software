using UnityEngine;

public class DestructorSuelo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Avisar al GameManager que el residuo tocó el suelo (Error)
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ProcesarError();
        }

        // 2. Borrar el residuo de la existencia
        Destroy(collision.gameObject);
        
        // El GeneradorInteligente detectará que se destruyó y lanzará el siguiente
    }
}