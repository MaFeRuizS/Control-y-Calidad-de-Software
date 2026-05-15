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
    public IEnumerator Test_MoverJoystickAbajo_CambiaDeBoton()
    {
        
        InputArcadeManager inputManager = InputArcadeManager.Instance;
        Assert.IsNotNull(inputManager, "El InputArcadeManager no se instanció en la escena.");

        GameObject botonInicial = EventSystem.current.currentSelectedGameObject;
        Assert.IsNotNull(botonInicial, "El EventSystem no tiene un botón 'First Selected' configurado.");

        inputManager.estadoActual.joyY = 4000; // Valor que supera tu umbralAlto
        inputManager.estadoActual.joyX = 2048; // Eje X en reposo
        inputManager.estadoActual.joyBtn = 0;  // Botón sin presionar

        yield return new WaitForSeconds(0.4f);

        // 4. Validaciones
        GameObject botonNuevo = EventSystem.current.currentSelectedGameObject;
        
        Assert.AreNotEqual(botonInicial, botonNuevo, "El joystick se movió hacia abajo pero el botón seleccionado no cambió.");
        
    }
}