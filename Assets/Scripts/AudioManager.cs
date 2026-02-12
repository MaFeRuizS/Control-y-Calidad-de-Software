using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Sound Effects")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip buttonClick;
    public AudioClip levelComplete;
    public AudioClip gameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCorrectSound()
    {
        if (correctSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(correctSound);
        }
    }

    public void PlayWrongSound()
    {
        if (wrongSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(wrongSound);
        }
    }

    public void PlayButtonClick()
    {
        if (buttonClick != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClick);
        }
    }

    public void PlayLevelComplete()
    {
        if (levelComplete != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(levelComplete);
        }
    }

    public void PlayGameOver()
    {
        if (gameOver != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(gameOver);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
}
