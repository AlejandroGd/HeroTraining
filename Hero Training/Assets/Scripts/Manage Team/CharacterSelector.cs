using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    GameSession gameSession;
    MusicFxController audioPlayer;


    [SerializeField] bool player;
    [SerializeField] Image prevImage;
    [SerializeField] Image selectedImage;
    [SerializeField] Image nextImage;    
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI ChangeListButtonText;
    [SerializeField] TextMeshProUGUI characterTypeLabel;

    [SerializeField] ManageTeamAttributeDisplay attribDisplay;
    [SerializeField] ManageTeamAIDisplay aiDisplay;

  
    List<CharacterLoadData> availableChars;
        
    int currentSelection;
    

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        audioPlayer = FindObjectOfType<MusicFxController>();

        InitialiseLists();

        InitialiseSelector();
    }

    //Initialise lists of trained and untrained characters from gamesession
    private void InitialiseLists()
    {
        if (player) availableChars = gameSession.AvailableUntrainedCharacters;        
        else availableChars = gameSession.AvailableTrainedCharacters;
    }

    private void InitialiseSelector()
    {
        currentSelection = 0;

        prevImage.type = Image.Type.Filled;
        prevImage.preserveAspect = true;
        selectedImage.type = Image.Type.Filled;
        selectedImage.preserveAspect = true;
        nextImage.type = Image.Type.Filled;
        nextImage.preserveAspect = true;

        if (player)
        {
            characterTypeLabel.text = "Player";
            ChangeListButtonText.text = "Add to Allies List";
        }
        else
        {
            characterTypeLabel.text = "Ally";
            ChangeListButtonText.text = "Add to Player List";
        }

        UpdateSelection();
    }

    private void UpdateSelection()
    {      
        //Images (Loop)
        if (currentSelection == 0) prevImage.sprite = availableChars[availableChars.Count - 1].CharacterPrefab.GetComponent<SpriteRenderer>().sprite;
        else prevImage.sprite = availableChars[currentSelection - 1].CharacterPrefab.GetComponent<SpriteRenderer>().sprite;
       
        selectedImage.sprite = availableChars[currentSelection].CharacterPrefab.GetComponent<SpriteRenderer>().sprite;

        nextImage.sprite = availableChars[ (currentSelection + 1) % availableChars.Count ].CharacterPrefab.GetComponent<SpriteRenderer>().sprite;
        
        //Text
        characterName.text = availableChars[currentSelection].CharacterName;

        attribDisplay.UpdateValues(availableChars[currentSelection].CharacterPrefab.GetComponent<Character>());
        aiDisplay.UpdateAIDisplayData(availableChars[currentSelection]);
    }

    public void SelectNextCharacter()
    {
        audioPlayer.PlayScrollButtonFx();
        currentSelection++;
        if (currentSelection >= availableChars.Count) currentSelection -= availableChars.Count;
        UpdateSelection();
    }

    public void SelectPreviousCharacter()
    {
        audioPlayer.PlayScrollButtonFx();
        currentSelection--;
        if (currentSelection < 0 ) currentSelection += availableChars.Count;
        UpdateSelection();
    }

    public CharacterLoadData SelectedFighter { get => GetSelectedFighter(); }

    private CharacterLoadData GetSelectedFighter()
    {
        if (player) return gameSession.AvailableUntrainedCharacters[currentSelection];
        else return gameSession.AvailableTrainedCharacters[currentSelection];        
    }

    public int GetSelectedCharacterIndex() { return currentSelection; }

    public void TrainingListChange()
    {
        if (player)
        {
            if (gameSession.AvailableUntrainedCharacters.Count > 1)
            {
                audioPlayer.PlayScrollButtonFx();
                //Add selected character to trained list and remove from the untrained one.
                gameSession.AvailableTrainedCharacters.Add(gameSession.AvailableUntrainedCharacters[currentSelection]);
                gameSession.AvailableUntrainedCharacters.RemoveAt(currentSelection);

            }
            else
            {
                audioPlayer.PlayMessageFx();
                FindObjectOfType<LevelManager>()?.ShowMessage("At least one Player character should remain available. Create a new player or add more player characters from the ally list before trying again.");
            }
        }
        else
        {
            if (gameSession.AvailableTrainedCharacters.Count > 1)
            {
                audioPlayer.PlayScrollButtonFx();
                //Add selected character to untrained list and remove from the trained one.
                gameSession.AvailableUntrainedCharacters.Add(gameSession.AvailableTrainedCharacters[currentSelection]);
                gameSession.AvailableTrainedCharacters.RemoveAt(currentSelection);
            }
            else
            {
                audioPlayer.PlayMessageFx();
                FindObjectOfType<LevelManager>()?.ShowMessage("At least one Ally character should remain available. Add more trained characters and try again.");
            }
        }
        currentSelection = 0;

        //We need to update both lists.
        foreach (CharacterSelector cs in FindObjectsOfType<CharacterSelector>())
        { cs.UpdateSelection(); }
    }

    public void DeleteCharacter()
    {
        if (player)
        {
            if (gameSession.AvailableUntrainedCharacters.Count > 1)
            {
                audioPlayer.PlayScrollButtonFx();
                //Add selected character to trained list and remove from the untrained one.                
                gameSession.AvailableUntrainedCharacters.RemoveAt(currentSelection);
            }
            else
            {
                audioPlayer.PlayMessageFx();
                FindObjectOfType<LevelManager>()?.ShowMessage("At least one Player character should remain available. Create a new player or add more player characters from the ally list before trying again.");
            }
        }
        else
        {
            if (gameSession.AvailableTrainedCharacters.Count > 1)
            {
                audioPlayer.PlayScrollButtonFx();
                //Add selected character to untrained list and remove from the trained one.                
                gameSession.AvailableTrainedCharacters.RemoveAt(currentSelection);
            }
            else
            {
                audioPlayer.PlayMessageFx();
                FindObjectOfType<LevelManager>()?.ShowMessage("At least one Ally character should remain available. Add more trained characters and try again.");
            }
        }
        currentSelection = 0;
        UpdateSelection();
    }

    public void ForgetSkill(int skillID)
    {
        SelectedFighter.LinkedAI.ForgetSkill(skillID);
        audioPlayer.PlayButtonFx();
        UpdateSelection();
    }
}