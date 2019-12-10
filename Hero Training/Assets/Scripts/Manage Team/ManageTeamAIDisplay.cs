using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTeamAIDisplay : MonoBehaviour
{
    [SerializeField] List<ManageTeamSkillData> skillDataList;
    [SerializeField] List<ForgetButton> skillForgetButtons;

    public void UpdateAIDisplayData(CharacterLoadData characterLoadData)
    {
        Character character = characterLoadData.CharacterPrefab.GetComponent<Character>();
        for (int x = 0; x < skillDataList.Count; x++)
        {
            if (x < character.SkillPrefabList.Count)
            {
                //Activate display and button for skill.
                skillDataList[x].gameObject.SetActive(true);
                skillForgetButtons[x].gameObject.SetActive(true);

                Skill skill = character.SkillPrefabList[x].GetComponent<Skill>();

                Sprite sprite = skill.GetComponent<SpriteRenderer>().sprite;
                Color colour = skill.GetComponent<SpriteRenderer>().color;
                string skillName = skill.MenuName;
                int skillRecords = characterLoadData.LinkedAI.CountSkillRecords(skill.SkillID);
                int totalRecords = characterLoadData.LinkedAI.TotalSkillRecords();
                if (totalRecords < 1) totalRecords = 1; //Avoid division by 0

                skillDataList[x].UpdateSkillData(sprite, colour, skillName, totalRecords, skillRecords);
                skillForgetButtons[x].UpdateForgetButtonData(skill.SkillID);                
            }
            else
            {
                skillDataList[x].gameObject.SetActive(false);
                skillForgetButtons[x].gameObject.SetActive(false);
            }
        }
    }    
}