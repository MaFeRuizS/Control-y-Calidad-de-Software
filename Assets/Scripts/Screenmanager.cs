
 using UnityEngine;
using UnityEngine.SceneManagement; 

public class NavegacionEscenas : MonoBehaviour
{
    
    public void IrAEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); 
        Application.Quit();
    }
}