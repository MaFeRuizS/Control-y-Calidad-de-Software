using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TestNavegacionMenu
{
    private string nombreEscenaMenu = "MainMenu"; 

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_MoverJoystick_CambiaDeBoton()
    {
        InputArcadeManager inputManager = InputArcadeManager.Instance;
        Assert.IsNotNull(inputManager, "El InputArcadeManager no se instanció.");

        inputManager.estadoActual.joyX = 2048;
        inputManager.estadoActual.joyY = 2048;
        yield return new WaitForSeconds(0.2f); // Esperamos a que tu Update lea el centro

        GameObject botonInicial = EventSystem.current.currentSelectedGameObject;

        inputManager.estadoActual.joyY = 4000;

        yield return new WaitForSeconds(0.4f);

        // 3. Validaciones
        GameObject botonNuevo = EventSystem.current.currentSelectedGameObject;
        
        Assert.AreNotEqual(botonInicial, botonNuevo, "El joystick se movió hacia abajo pero el botón seleccionado no cambió.");
    }
}