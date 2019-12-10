using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{       
    GameObject messBox;
    MusicFxController audioPlayer;

    private void Start()
    {
        audioPlayer = FindObjectOfType<MusicFxController>();
        messBox = GameObject.Find("Message Box");
        messBox?.SetActive(false);
    
    }

    //Show message is a Message box is available in the scene.
    public void ShowMessage(string message)
    {
        messBox?.SetActive(true);
        messBox?.GetComponent<MessageBox>()?.UpdateMessage(message);        
    }

    public void LoadStartMenu()
    {
        audioPlayer.PlayMenuMusic();
        audioPlayer.PlayButtonFx();
        StartCoroutine(ChangeLevel(0));      
    }

    public void LoadCourseSelection() 
    {
        //Here instead of serialised because gamesession is singleton throughout all scenes 
        GameSession gameSession = FindObjectOfType<GameSession>();
        CharacterSelector playerDropdown = GameObject.Find("Character Selector Player").GetComponent<CharacterSelector>();
        CharacterSelector allyDropdown = GameObject.Find("Character Selector Ally").GetComponent<CharacterSelector>();
        
   
        //Load info in game session
        gameSession.PlayerIndex = playerDropdown.GetSelectedCharacterIndex();
        gameSession.AllyIndex = allyDropdown.GetSelectedCharacterIndex();


        //Load Course Selection
        audioPlayer.PlayButtonFx();
        StartCoroutine(ChangeLevel(4));
        //SceneManager.LoadScene(4);        
    }

    public void LoadTeamManagement()
    {
        //Load Team Management Scene
        GameSession gameSession = FindObjectOfType<GameSession>();

        if (gameSession.AvailableUntrainedCharacters.Count <= 0)
        {
            audioPlayer.PlayMessageFx();
            ShowMessage("Go to New Hero to create at least one untrained character.");
        }
        else
        {
            audioPlayer.PlayButtonFx();
            StartCoroutine(ChangeLevel(1));                    
        }
    }    

    public void LoadFightScene() 
    {
        
        //Here instead of serialised because gamesession is singleton throughout all scenes     
        //Load Battle course in game session
        GameSession gameSession = FindObjectOfType<GameSession>();
        BattleSelector battleSelector = FindObjectOfType<BattleSelector>();

        gameSession.BattleIndex = battleSelector.GetSelectedBattleCourseIndex();


        //Load fight scene
        audioPlayer.PlayButtonFx();
        audioPlayer.PlayFightMusic();
        StartCoroutine(ChangeLevel(2));
        //SceneManager.LoadScene(2);        
    }
       
    public void LoadNewHero()
    {
        audioPlayer.PlayButtonFx();
        StartCoroutine(ChangeLevel(3));
        //SceneManager.LoadScene(3);        
    }

    public IEnumerator ChangeLevel(int scene)
    {
        float fadeTime = FindObjectOfType<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(scene);
    }

    public void SaveToFile()
    {
        GameSession gs = FindObjectOfType<GameSession>();
        gs.SaveCharacterDataToFile();
        audioPlayer.PlayButtonFx();
        
    }

    public void LoadFromFile()
    {
        GameSession gs = FindObjectOfType<GameSession>();
        gs.LoadCharacterDataFromFile();
        audioPlayer.PlayButtonFx();        

    }

    public void QuitGame()
    {
        Application.Quit();
    }    
}