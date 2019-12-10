using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsPanel : MonoBehaviour
{
    [SerializeField] List<KnowledgeGainedPanel> knowledgePanels;
    [SerializeField] KnowledgeGainedPanel rewardPanel;
    [SerializeField] KnowledgeGainedPanel punishmentPanel;

    //Fills the Result panel with the player's skills info after the battle
    public void UpdateResultsPanelInfo(Character player, Dictionary<int, int> skillsUsedInFight, Character ally, Skill rewardSkill, Skill punishmentSkill)
    {
        gameObject.SetActive(true);
        for (int x = 0; x < knowledgePanels.Count; x++)
        {
            if (x < player.SkillPrefabList.Count)
            {
                knowledgePanels[x].gameObject.SetActive(true);
                
                Skill skill = player.SkillPrefabList[x].GetComponent<Skill>();

                Sprite sprite = skill.GetComponent<SpriteRenderer>().sprite;
                Color colour = skill.GetComponent<SpriteRenderer>().color;
                string skillName = skill.MenuName;
                int timesUsedInFight = skillsUsedInFight.ContainsKey(skill.SkillID) ? skillsUsedInFight[skill.SkillID] : 0;
                int totalRecords = player.CharacterAI.CountSkillRecords(skill.SkillID);
                
                knowledgePanels[x].UpdatePanel(sprite, colour, skillName, timesUsedInFight, totalRecords);
            }
            else
            {
                knowledgePanels[x].gameObject.SetActive(false);
            }           
        }

        if (rewardSkill == null) rewardPanel.gameObject.SetActive(false);
        else
        {
            Sprite sprite = rewardSkill.GetComponent<SpriteRenderer>().sprite;
            Color colour = rewardSkill.GetComponent<SpriteRenderer>().color;
            string skillName = rewardSkill.MenuName;
            int totalRecords = ally.CharacterAI.CountSkillRecords(rewardSkill.SkillID);
            rewardPanel.UpdatePanel(sprite, colour, skillName, 1, totalRecords);
        }

        if (punishmentSkill == null) punishmentPanel.gameObject.SetActive(false);
        else
        {
            Sprite sprite = punishmentSkill.GetComponent<SpriteRenderer>().sprite;
            Color colour = punishmentSkill.GetComponent<SpriteRenderer>().color;
            string skillName = punishmentSkill.MenuName;
            int totalRecords = ally.CharacterAI.CountSkillRecords(punishmentSkill.SkillID);
            punishmentPanel.UpdatePanel(sprite, colour, skillName, 1, totalRecords);
        }
    }

    void Update()
    {
        //Go back to Main menu when mouse clicked
        if (Input.GetMouseButtonDown(0)) GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadStartMenu();
    }
}