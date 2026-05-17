using UnityEngine;

public class TestButtonSound : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        // Sonido UI
        GameAudioManager.instance.PlaySFX(GameAudioManager.SFXType.UISelect);

        Debug.Log("Botón presionado: juego detenido");
    }
}