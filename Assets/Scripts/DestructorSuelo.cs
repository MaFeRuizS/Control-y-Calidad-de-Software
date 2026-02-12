using UnityEngine;

public class DestructorSuelo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cuando cualquier residuo toca el suelo, lo borramos de la existencia
        Destroy(collision.gameObject);
        
        // Como el objeto se destruye, el GeneradorInteligente detectará 
        // que ya no hay nada en pantalla y lanzará el siguiente
    }
}