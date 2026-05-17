using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager instance;

    [Header("Music")]
    public AudioClip musicClip;

    private AudioSource musicSource;

    [Header("Toggle Sounds")]
    public AudioClip onSound;
    public AudioClip offSound;

    [Header("SFX Clips")]
    public AudioClip errorSound;
    public AudioClip correctSound;
    public AudioClip dropSound;
    public AudioClip buttonClickSound;
    public AudioClip uiSelectSound;

    public bool sfxOn = true;

    //ENUM DE SONIDOS
    public enum SFXType
    {
        Error,
        Correct,
        Drop,
        ButtonClick,
        UISelect
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //AudioSource de música (creado por código)
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
            musicSource.clip = musicClip;
            musicSource.volume = 0.5f;
            musicSource.Play();

            //Aplicar configuración guardada
            int musicPref = PlayerPrefs.GetInt("music", 1);
            SetMusic(musicPref == 1);

            int sfxPref = PlayerPrefs.GetInt("sfx", 1);
            SetSFX(sfxPref == 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //CONTROL DE MÚSICA
    public void SetMusic(bool value)
    {
        if (musicSource != null)
            musicSource.mute = !value;
    }

    //CONTROL DE SFX
    public void SetSFX(bool value)
    {
        sfxOn = value;
    }

    //SONIDO DE TOGGLE
    public void PlayToggleSound(bool isOn)
    {
        if (!sfxOn) return;

        AudioClip clip = isOn ? onSound : offSound;

        if (clip != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    //FUNCIÓN GENERAL DE SFX
    public void PlaySFX(SFXType type)
    {
        if (!sfxOn) return;

        AudioClip clip = null;

        switch (type)
        {
            case SFXType.Error:
                clip = errorSound;
                break;

            case SFXType.Correct:
                clip = correctSound;
                break;

            case SFXType.Drop:
                clip = dropSound;
                break;

            case SFXType.ButtonClick:
                clip = buttonClickSound;
                break;

            case SFXType.UISelect:
                clip = uiSelectSound;
                break;
        }

        if (clip != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}