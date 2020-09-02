using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   
    private int skillIndex;     //To know which skill in the active character skill list to use.
    private string help;        //To display when hoovering over the button.

    //References
    private SkillButtonsGUI gui;            //We need to be able to call the ButtonSkillClicked on the GUI main script.
    TextMeshProUGUI buttonTextComponent;    //Saves time to not find a reference every time the button needs to be updated.
    Button buttonComponent;                 //As above.

    //Assigned on inspector.
    [SerializeField] TextMeshProUGUI helpText;

    private void Start()
    {
        gui = FindObjectOfType<SkillButtonsGUI>();
        buttonTextComponent = GetComponentInChildren<TextMeshProUGUI>();
        buttonComponent = GetComponent<Button>();
    }

    public void ReloadSkillInfo(Skill skill, int indexInList)
    {
        skillIndex = indexInList;   
        help = skill.HelpText;

        buttonTextComponent.text = skill.MenuName;    //Updates button text with the skill name.
        buttonComponent.enabled = true;
    }

    public void DeactivateButton()
    {
        skillIndex = -1;        
        help = "";
        buttonTextComponent.text = "";
        buttonComponent.enabled = false;
    }

    public void OnClickButton()
    {
        gui.ButtonSkillClicked(skillIndex);
    }      

    public void OnPointerExit(PointerEventData eventData)
    {
        helpText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        helpText.text = help;
    }
}