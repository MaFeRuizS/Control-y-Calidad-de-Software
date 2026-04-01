using UnityEngine;

public class DestructorMatematicas : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        // 1. Verificamos cuál era la figura importante
        string tagNecesario = GameManagerMatematicas.Instance.gestorPatrones.tagCorrectoParaAtrapar;

        // 2. Si la figura que tocó el suelo era justo la que necesitábamos... ¡el jugador la dejó escapar!
        if (col.CompareTag(tagNecesario))
        {
            GameManagerMatematicas.Instance.ProcesarError();
        }
        
        // 3. Si era una figura trampa, no pasa nada. Igual la destruimos para limpiar la pantalla.
        Destroy(col.gameObject);
    }
}