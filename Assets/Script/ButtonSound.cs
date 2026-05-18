using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public GameAudioManager.SFXType soundType = GameAudioManager.SFXType.ButtonClick;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        GameAudioManager.instance.PlaySFX(soundType);
    }
}