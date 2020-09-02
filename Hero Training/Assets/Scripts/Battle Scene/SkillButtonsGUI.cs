using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillButtonsGUI : MonoBehaviour
{
    BattleManager bsStateMachine;

    [SerializeField] List<Button> skillButtonList;
    
    private Character activeCharacter;

    private void Start()
    {
        //Reference to the Turn Based Fight System
        bsStateMachine = FindObjectOfType<BattleManager>();
    }
    
    public void ReloadSkillsInfo(Character character)
    {
        //Sets the active character
        this.activeCharacter = character;

        //Loads info in buttons or deactivate them if there are not enough skills for them.
        for (int i = 0; i < 8; i++) //8 buttons max (No character will have more than 8 skills)
        {
            if (i < character.SkillPrefabList.Count)
            {
                skillButtonList[i].GetComponent<SkillButton>().ReloadSkillInfo(character.SkillPrefabList[i].GetComponent<Skill>(), i);               
            }
            else
            {
                skillButtonList[i].GetComponent<SkillButton>().DeactivateButton();
            }
        }
    }   

    public void ClearSkillButtonGUI()
    {
        foreach(Button button in skillButtonList)
        {
            button.GetComponent<SkillButton>().DeactivateButton();
        }
    }      

    public void ButtonSkillClicked(int index)
    {
        if (index < bsStateMachine.PlayerCharacter.SkillPrefabList.Count)
        {
            GameObject skillPrefabChosen = activeCharacter.SkillPrefabList[index];
            Skill skill = skillPrefabChosen.GetComponent<Skill>();

            bsStateMachine.ApplyPlayerChoice(skillPrefabChosen);            
        }
        else { Debug.Log("No skill assigned"); }
    }
}