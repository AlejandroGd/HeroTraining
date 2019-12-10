using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KnowledgeGainedPanel : MonoBehaviour
{
    [SerializeField] Image skillImage;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI levelsGained;
    [SerializeField] TextMeshProUGUI totalLevels;

    public void UpdatePanel(Sprite skillSprite, Color colour, string skillName, int levelsGained, int totalLevels)
    {
        this.skillImage.sprite = skillSprite;
        this.skillImage.color = colour;
        this.skillName.text = skillName;
        this.levelsGained.text = "+ " + levelsGained;
        this.totalLevels.text = totalLevels.ToString();
    }   
}