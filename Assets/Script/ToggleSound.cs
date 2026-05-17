using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public Toggle toggle;
    public bool isMusic;

    void Start()
    {
        if (isMusic)
            toggle.isOn = PlayerPrefs.GetInt("music", 1) == 1;
        else
            toggle.isOn = PlayerPrefs.GetInt("sfx", 1) == 1;

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (isMusic)
        {
            GameAudioManager.instance.SetMusic(isOn);
            PlayerPrefs.SetInt("music", isOn ? 1 : 0);
        }
        else
        {
            GameAudioManager.instance.SetSFX(isOn);
            PlayerPrefs.SetInt("sfx", isOn ? 1 : 0);
        }

        PlayerPrefs.Save();

        GameAudioManager.instance.PlayToggleSound(isOn);
    }
}