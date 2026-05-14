using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PruebasCiencias
{
    // TEST CASE 1: Escenario Positivo
    [UnityTest]
    public IEnumerator Validar_Pausa_En_Escena_Ciencias()
    {
        // ARRANGE
        Time.timeScale = 1f; 
        yield return SceneManager.LoadSceneAsync("lluvia_ciencias"); 

        // Le damos 0.5 segundos reales a la escena para que se configure sola
        yield return new WaitForSecondsRealtime(0.5f); 

        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        Assert.IsNotNull(manager, "Error: No se encontró el GameManager.");

        // ACT
        manager.PausarJuego();
        yield return new WaitForSecondsRealtime(0.1f); 

        // ASSERT
        Assert.IsTrue(manager.pausePanel.activeSelf, "Fallo: El panel de pausa no es visible.");
        Assert.AreEqual(0f, Time.timeScale, "Fallo: El tiempo no se detuvo.");
    }

    // TEST CASE 2: Escenario Negativo
    [UnityTest]
    public IEnumerator Validar_Pausa_Es_Ignorada_Si_Juego_Terminado()
    {
        // ARRANGE
        Time.timeScale = 1f;
        yield return SceneManager.LoadSceneAsync("lluvia_ciencias"); 

        yield return new WaitForSecondsRealtime(0.5f); 

        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        Assert.IsNotNull(manager, "Error: No se encontró el GameManager.");

        // ACT: Forzamos la derrota
        manager.TerminarJuego(false); 
        // Esperamos 0.1s para que a Unity le dé tiempo de encender físicamente el Game Over
        yield return new WaitForSecondsRealtime(0.1f);
        
        // Intentamos pausar
        manager.PausarJuego();
        yield return new WaitForSecondsRealtime(0.1f);

        // ASSERT: Verificamos que el escudo nos defendió
        Assert.IsFalse(manager.pausePanel.activeSelf, "FALLO GRAVE: El menú de pausa se sobrepuso al Game Over.");
    }
}