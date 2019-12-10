using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicFxController : MonoBehaviour
{
    [SerializeField] AudioClip menuMusicClip;
    [SerializeField] AudioClip fightMusicClip;
    [SerializeField] AudioClip menuButtonClip;
    [SerializeField] AudioClip scrollButtonClip;
    [SerializeField] AudioClip messageClip;

    //Fx should play on top of music, so I need 2 audio sources
    AudioSource musicPlayer;
    AudioSource fxPlayer;

    bool menuMusicPlaying = false;
    private void Awake()
    {
        SetUpSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        musicPlayer = audioSources[0];
        fxPlayer = audioSources[1]; fxPlayer.volume = 0.5f;
        PlayMenuMusic();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            //Debug.Log("Awake (gamesession): Destroy Gamesession: " + GetInstanceID());
            DestroyImmediate(gameObject);
        }
        else
        {
            // Debug.Log("Awake (gamesession): Setting gamesession ID:" + GetInstanceID());
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        //We only start menu music if it is not playing already.
        if (!menuMusicPlaying )
        {
            menuMusicPlaying = true;
            musicPlayer.clip = menuMusicClip;
            musicPlayer.volume = 0.5f;
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
    }

    public void PlayFightMusic()
    {
        menuMusicPlaying = false;
        musicPlayer.clip = fightMusicClip;
        musicPlayer.volume = 0.1f;
        musicPlayer.loop = true;
        musicPlayer.Play();        
    }

    public void PlayButtonFx()
    {
        fxPlayer.clip = menuButtonClip;
        fxPlayer.Play();
    }

    public void PlayScrollButtonFx()
    {
        fxPlayer.clip = scrollButtonClip;
        fxPlayer.Play();
    }

    public void PlayMessageFx()
    {
        fxPlayer.clip = messageClip;
        fxPlayer.Play();
    }    
}