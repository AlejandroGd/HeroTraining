using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Deals with the dropdown in the New Hero scene to select the type of hero to create.
public class FighterSelectionDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropDown;
    
    GameSession gameSession;
    

    public CharacterLoadData SelectedFighter { get => GetSelectedFighter(); }

    private CharacterLoadData GetSelectedFighter()
    {       
        return gameSession.AvailableAllies[dropDown.value];       
    }
    public int GetSelectedCharacterIndex() { return dropDown.value; }

    // Start is called before the first frame update
    void Start()
    {    
        gameSession = FindObjectOfType<GameSession>();
        LoadAllies();       
    }
    
    private void LoadAllies()
    {        
        List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>();
        TMP_Dropdown.OptionData newOption;

        dropDown.ClearOptions();

        foreach (CharacterLoadData data in gameSession.AvailableAllies)
        {
            newOption = new TMP_Dropdown.OptionData();
            newOption.text =  data.CharacterPrefab.GetComponent<Character>().MenuName + " - " + data.CharacterName;
            newOption.image = data.CharacterPrefab.GetComponent<SpriteRenderer>().sprite;      
            optionList.Add(newOption);
        }

        dropDown.AddOptions(optionList);
    }
}