using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageTeamAttributeDisplay : MonoBehaviour
{
    //Custom Colours for the text
    Color32 darkGreen = new Color32(15, 60, 5, 255);
    Color32 darkRed = new Color32(180, 10, 10, 255);

    [SerializeField] TextMeshProUGUI physAtkValue;
    [SerializeField] TextMeshProUGUI physDefValue;
    [SerializeField] TextMeshProUGUI magAtkValue;
    [SerializeField] TextMeshProUGUI magDefValue;
    [SerializeField] TextMeshProUGUI fireResValue;
    [SerializeField] TextMeshProUGUI iceResValue;
    [SerializeField] TextMeshProUGUI weakResValue;
    [SerializeField] TextMeshProUGUI blindResValue;
    [SerializeField] TextMeshProUGUI muteResValue;
    
    public void UpdateValues(Character character)
    {
        physAtkValue.text = character.PhysicalAttack.ToString();       
        physDefValue.text = character.PhysicalDefense.ToString();
        physDefValue.color = character.PhysicalDefense < 0 ? darkRed : darkGreen;

        magAtkValue.text = character.MagicAttack.ToString();
        magDefValue.text = character.MagicDefense.ToString();
        magDefValue.color = character.MagicDefense < 0 ? darkRed : darkGreen;

        fireResValue.text = character.FireDefense.ToString();
        fireResValue.color = character.FireDefense < 0 ? darkRed : darkGreen;
        iceResValue.text = character.IceDefense.ToString();        
        iceResValue.color = character.IceDefense < 0 ? darkRed : darkGreen;

        weakResValue.text = character.ResistWeak.ToString();
        weakResValue.color = character.ResistWeak < 0 ? darkRed : darkGreen;
        blindResValue.text = character.ResistBlind.ToString();
        blindResValue.color = character.ResistBlind < 0 ? darkRed : darkGreen;
        muteResValue.text = character.ResistMute.ToString();
        muteResValue.color = character.ResistMute < 0 ? darkRed : darkGreen;
    }    
}