using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillButtonsGUI : MonoBehaviour
{

    BattleManagerStateMachine bsStateMachine;
   
    [SerializeField] List<Button> skillButtonList;
    
    private Character activeCharacter;

    private void Start()
    {
        //Reference to the Turn Based Fight System
        bsStateMachine = GameObject.Find("Battle Manager State Machine").GetComponent<BattleManagerStateMachine>();
    }


    public void ReloadSkillsInfo(Character activeCharacter)
    {
        //Sets the active character
        this.activeCharacter = activeCharacter;

        //Loads info in buttons or deactivate them if there are not enough skills for them.
        for (int i = 0; i < 8; i++) //8 buttons max (No character will have more than 8 skills)
        {
            if (i < activeCharacter.SkillPrefabList.Count)
            {
                skillButtonList[i].GetComponent<SkillButton>().ReloadSkillInfo(activeCharacter.SkillPrefabList[i].GetComponent<Skill>(), i);               
            }
            else
            {
                skillButtonList[i].GetComponent<SkillButton>().DeactivateButton();
            }
        }
    }   

    private void ClearSkillButtonGUI()
    {
        foreach(Button button in skillButtonList)
        {
            button.GetComponent<SkillButton>().DeactivateButton();
        }
    }      

    public void ButtonSkillClicked(int index)
    {       
        if (index < activeCharacter.SkillPrefabList.Count)
        {
            GameObject skillPrefabChosen = activeCharacter.SkillPrefabList[index];
            Skill skill = skillPrefabChosen.GetComponent<Skill>();            
            //Only player uses GUI, so if the skill is a buff apply to all allies. If not, apply to the enemy.
            if (skill.SkillClass == Skill.SkillType.BUFF || skill.SkillClass == Skill.SkillType.HEAL)
            {
                activeCharacter.ApplySkillToAllies(skillPrefabChosen);
            }
            else
            {
                activeCharacter.ApplySkillToEnemies(skillPrefabChosen);
            }

            FightAIRecord aiRecord = new FightAIRecord( bsStateMachine.GetCharacterComponent_Player(),
                                                        bsStateMachine.GetCharacterComponent_Ally(),
                                                        bsStateMachine.GetCharacterComponent_Enemy(), 
                                                        skill);
            if (activeCharacter.PlayerControlled) activeCharacter.CharacterAI.Learn(aiRecord);
            bsStateMachine.UpdateSkillsUsedCount(skill.SkillID);

            //Clear the Buttons text and skill for next character.
            ClearSkillButtonGUI();

            bsStateMachine.currentBattleState = BattleState.PROCESS;
        }
        else { Debug.Log("No skill assigned"); }      
    }
}