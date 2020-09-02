using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Deals with a single skill data in the display of AI skills in the character selector.
public class ManageTeamSkillData : MonoBehaviour
{
    [SerializeField] Image skillImage;
    [SerializeField] Image percentageBar;
    [SerializeField] TextMeshProUGUI percentageAndNameText;
    [SerializeField] TextMeshProUGUI numRecordsText;
 
    public void UpdateSkillData(Sprite sprite, Color colour, string skillName, int totalRecords, int skillRecords)
    {
        skillImage.sprite = sprite;
        skillImage.color = colour;

        int total = totalRecords;
        int records = skillRecords;

        float percentage = Mathf.Clamp(records, 0, total) / (float)total;

        //Scaling bar
        percentageBar.transform.localScale = new Vector3(percentage, percentageBar.transform.localScale.y, percentageBar.transform.localScale.z);

        percentageAndNameText.text = skillName + " (" + (percentage * 100) + " %)";
        numRecordsText.text = records.ToString();                
    }
}