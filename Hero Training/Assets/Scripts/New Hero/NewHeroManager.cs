using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Deals with the New Hero Manager screen. 
public class NewHeroManager : MonoBehaviour
{
    [SerializeField] List<GameObject> allyPrefabs;

    [SerializeField] TMP_Dropdown classDropdown;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Image portrait;

    GameSession gameSession;
    LevelManager levelManager;

    void Start()
    {
        //References
        gameSession = FindObjectOfType<GameSession>();
        levelManager = FindObjectOfType<LevelManager>();

        //Load dropdown
        LoadDropdown();

        //Update portrait with 1st option
        UpdatePortrait();
    }

    //Load dropdown with the classes that can be used to create new characters
    private void LoadDropdown()
    {        
        List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>();
        TMP_Dropdown.OptionData newOption;

        classDropdown.ClearOptions();

        foreach (GameObject prefab in allyPrefabs)
        {
            newOption = new TMP_Dropdown.OptionData();
            newOption.text = prefab.GetComponent<Character>().MenuName;
            newOption.image = prefab.GetComponent<SpriteRenderer>().sprite;
            optionList.Add(newOption);
        }

        classDropdown.AddOptions(optionList);        
    }

    //Updates the image in the portrait based on the dropdown selection. Changes image options to not show a stretched image.
    public void UpdatePortrait()
    {
        portrait.sprite = classDropdown.options[classDropdown.value].image;
        portrait.type = Image.Type.Filled;
        portrait.preserveAspect = true;
    }

    //Creates the character and adds it to the available fighters list in game session
    public void CreateCharacter()
    {
        CharacterLoadData data = new CharacterLoadData(nameInput.text, allyPrefabs[classDropdown.value], new Player_AI()); //Untrained player AI
        //CharacterLoadData data = new CharacterLoadData(nameInput.text, allyPrefabs[classDropdown.value], new Player_AI(CreateCharacterMageTestAI()), false); //AI Testing
        gameSession.AvailableUntrainedCharacters.Add(data);
        FindObjectOfType<MusicFxController>().PlayButtonFx();
        levelManager.LoadStartMenu();
    }

    public List<FightAIRecord> CreateCharacterMageTestAI()
    {
        List<FightAIRecord> temp = new List<FightAIRecord>();
        
        { float[] array = { 21, 1, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 1, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 1, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 21, 0.9142857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 1, 0, 0.982f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.85f, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 21, 0.8457143f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.892f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.675f, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 21, 0.7771429f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.802f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.5f, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 21, 0.7771429f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.802f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.3625f, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 21, 0.7771429f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.802f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.225f, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 21, 0.6914285f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.826f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.075f, 75, 75, 75, 75, 75, 75, -50, -60, 60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.6914285f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.826f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 1, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.6057143f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 1, 0, 0.808f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.85f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.7342857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.94f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.825f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.6657143f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.85f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.65f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.88f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 1, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.65625f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.88f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 1, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.51875f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.7942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.964f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.36875f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.7942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.964f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.23125f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.7942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.964f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.09375f, 75, 75, 75, 75, 75, 75, -50, 60, -60, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 20, 0.7942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.964f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 1, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 20, 0.7942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.888f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.75f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.58f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.844f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.46f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 32, 0.7942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 1, 1, 0, 0.994f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.46f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.58f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 1, 0, 1, 0.972f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 1, 0.41f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.3657143f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.956f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.37f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.4942857f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.888f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.33f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.6028572f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 1, 0, 0.978f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.33f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.7114286f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.978f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 1, 0, 0.29f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 31, 0.5828571f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 0.956f, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.09f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }
        { float[] array = { 22, 0.7971429f, 100, 25, 0, 20, 0, 100, 60, 20, 20, 0, 0, 0, 0, 0, 1, 25, 100, 25, 100, 70, 0, 25, 0, 0, 0, 0, 0, 0, 0, 0.05f, 50, 50, 50, 75, 75, 75, -25, -50, -50, 0, 0, 0, 0, 0 }; temp.Add(new FightAIRecord(array)); }

        
        return temp;
    }

}