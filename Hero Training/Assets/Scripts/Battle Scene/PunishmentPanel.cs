using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishmentPanel : MonoBehaviour
{
    [SerializeField] List<PunishmentSkillChoice> punishmentSkillChoicePanels;
    [SerializeField] PunishmentSkillChoice actionPerformedPanel;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    //Fills the Result panel with the player's skills info after the battle
    public void UpdatePunishmentSkillPanelInfo(Character ally, Skill skillPerformed)
    {
        gameObject.SetActive(true);

        actionPerformedPanel.UpdatePanel(skillPerformed);

        for (int x = 0; x < punishmentSkillChoicePanels.Count; x++)
        {
            if (x < ally.SkillPrefabList.Count)
            {
                punishmentSkillChoicePanels[x].gameObject.SetActive(true);

                Skill skill = ally.SkillPrefabList[x].GetComponent<Skill>();

                Sprite sprite = skill.GetComponent<SpriteRenderer>().sprite;
                Color colour = skill.GetComponent<SpriteRenderer>().color;
                string skillName = skill.MenuName;                

                punishmentSkillChoicePanels[x].UpdatePanel(skill);
            }
            else
            {
                punishmentSkillChoicePanels[x].gameObject.SetActive(false);
            }
        }
    }
}