using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class ValidacionesPartida
{
    private string nombreEscena = "lluvia_ciencias"; 

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(nombreEscena);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_Victoria_AlLlegarA15Aciertos()
    {
        GameManager gm = GameManager.Instance;
        Assert.IsNotNull(gm, "El GameManager no se instanció correctamente.");

        // Simulamos 15 aciertos perfectos
        for (int i = 0; i < 15; i++)
        {
            GameObject basuraSimulada = new GameObject("BasuraTest");
            gm.ProcesarAcierto(basuraSimulada);
        }

        yield return null;

        // Validaciones: El juego debió pausarse, el tiempo detenerse y la fachada mostrarse
        Assert.IsTrue(gm.juegoPausado, "El juego no se pausó al llegar a la meta.");
        Assert.AreEqual(0f, Time.timeScale, "El timeScale no se redujo a 0.");
        Assert.IsTrue(gm.panelRetroalimentacion.gameObject.activeSelf, "La fachada de resultados no apareció.");
    }

    [UnityTest]
    public IEnumerator Test_Derrota_AlPerderTodasLasVidas()
    {
        GameManager gm = GameManager.Instance;

        // Simulamos dejar caer 3 objetos al fondo (límite inferior)
        for (int i = 0; i < 3; i++)
        {
            gm.ProcesarError(true);
        }

        yield return null;

        // Validaciones: Derrota por falta de vidas
        Assert.IsTrue(gm.juegoPausado, "El juego no terminó tras perder las 3 vidas.");
        Assert.AreEqual(0f, Time.timeScale, "El tiempo del motor no se congeló.");
        Assert.IsTrue(gm.panelRetroalimentacion.gameObject.activeSelf, "No se mostró el panel de retroalimentación tras la derrota.");
    }

    [UnityTest]
    public IEnumerator Test_Derrota_PorTiempoAgotado()
    {
        GameManager gm = GameManager.Instance;

        // Aceleramos el reloj
        var campoTiempo = typeof(GameManager).GetField("tiempoRestante", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        campoTiempo.SetValue(gm, 0.5f);

        // Usamos WaitForSecondsRealtime para que la prueba no se congele 
        // cuando tu GameManager ponga el Time.timeScale en 0
        yield return new WaitForSecondsRealtime(1.5f); 

        // Validaciones
        Assert.IsTrue(gm.juegoPausado, "El cronómetro llegó a cero pero el juego continuó.");
        Assert.IsTrue(gm.panelRetroalimentacion.gameObject.activeSelf, "El reporte de la UCP no apareció al agotarse el tiempo.");
    }

    [UnityTest]
    public IEnumerator Test_Fachada_MuestraDatosCorrectos()
    {
        GameManager gm = GameManager.Instance;
        Assert.IsNotNull(gm, "El GameManager no está instanciado.");

        int puntosSimulados = 85;
        int aciertosSimulados = 8;
        
        int errMetales = 2;
        int errOrganicos = 5; // Hacemos que Orgánicos sea deliberadamente el mayor error
        int errInorganicos = 1;
        int errReciclables = 0;

        gm.panelRetroalimentacion.MostrarResultados(
            puntosSimulados, 
            aciertosSimulados, 
            errMetales, 
            errOrganicos, 
            errInorganicos, 
            errReciclables
        );

        yield return null;

        //Validamos que los textos digan EXACTAMENTE lo que deben decir
        
        // Verificamos los contadores
        Assert.AreEqual("8 Aciertos", gm.panelRetroalimentacion.txtAciertosTotales.text, "Los aciertos totales fallaron.");
        Assert.AreEqual("2", gm.panelRetroalimentacion.txtErrMetales.text, "Los errores de metales no se mostraron bien.");
        Assert.AreEqual("5", gm.panelRetroalimentacion.txtErrOrganicos.text, "Los errores orgánicos no se mostraron bien.");
        Assert.AreEqual("1", gm.panelRetroalimentacion.txtErrInorganicos.text, "Los errores inorgánicos no se mostraron bien.");
        Assert.AreEqual("0", gm.panelRetroalimentacion.txtErrReciclables.text, "Los errores reciclables no se mostraron bien.");

        // Verificamos la lógica inteligente del panel (Debe decirnos que nos fijemos en Orgánicos)
        Assert.AreEqual("Mmm... fíjate en: Orgánicos", gm.panelRetroalimentacion.txtTituloFeedback.text, "El título no detectó correctamente la categoría crítica.");
    }
}