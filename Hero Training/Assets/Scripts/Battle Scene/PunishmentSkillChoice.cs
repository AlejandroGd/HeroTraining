using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PunishmentSkillChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Skill skill;
    [SerializeField] Image skillImage;
    [SerializeField] Image panelImage;
    [SerializeField] TextMeshProUGUI skillName;
        
    public void UpdatePanel(Skill skill)
    {
        this.skill = skill;
        this.skillImage.sprite = skill.GetComponent<SpriteRenderer>().sprite;
        this.skillImage.color = skill.GetComponent<SpriteRenderer>().color; ;
        this.skillName.text = skill.MenuName;
        panelImage.color = new Color(255, 255, 255, 100); ;
    }
       
    public void OnPointerEnter(PointerEventData eventData)
    {
        panelImage.color = new Color(0.2f, 0.95f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panelImage.color = new Color(255, 255, 255, 100);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //This happens only once per battle, no overhead if FindObjectOfType is used here.
        FindObjectOfType<BattleManagerStateMachine>().AddPunishmentRecord(skill);
        GameObject.Find("Punishment Panel").SetActive(false);
    }
}