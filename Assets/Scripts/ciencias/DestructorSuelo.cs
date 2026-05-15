using UnityEngine;

public class DestructorSuelo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance != null) 
        {
            // Solo avisamos al GameManager que se cayó. Él hará el resto.
            GameManager.Instance.ProcesarError(true);
        }
    }
}