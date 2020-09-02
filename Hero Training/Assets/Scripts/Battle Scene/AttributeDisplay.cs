using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Attached to attribute Display Objects, deals with hp bars and status icons for specific characters
public class AttributeDisplay : MonoBehaviour
{
    Character character;
    [SerializeField] Image healthValueBar;
    [SerializeField] Image shieldStatusIcon;
    [SerializeField] Image barrierStatusIcon;
    [SerializeField] Image blindStatusIcon;
    [SerializeField] Image muteStatusIcon;
    [SerializeField] Image weakStatusIcon;

    //Gets the character whose attributes will be displayed
    public void SetCharacterPrefab(GameObject characterPrefab)
    {
        character = characterPrefab.GetComponent<Character>();
        InitialiseDisplay();
    }

    //Updates the bar/icons given the current state of the character
    public void UpdateValues()
    {
        UpdateHp();
        UpdateStatus();
    }

    //Get Display ready to be used (updates the HP bar and tint all status icons grey).
    private void InitialiseDisplay()
    {
        UpdateHp();
        shieldStatusIcon.color = Color.grey;
        barrierStatusIcon.color = Color.grey;
        blindStatusIcon.color = Color.grey;
        muteStatusIcon.color = Color.grey;
        weakStatusIcon.color = Color.grey;
    }    

    //Update HP bar.
    private void UpdateHp()
    {
        float scaledHpVal = Mathf.Clamp(character.CurrentHealthPoints, 0, character.MaxHealthPoints) / (float)character.MaxHealthPoints;
        healthValueBar.transform.localScale = new Vector3(scaledHpVal, healthValueBar.transform.localScale.y, healthValueBar.transform.localScale.z);

        if (scaledHpVal > 0.5f) healthValueBar.color = Color.green;
        else if (scaledHpVal > 0.25f) healthValueBar.color = Color.yellow;
        else healthValueBar.color = Color.red;
    }

    //Update status icons.
    private void UpdateStatus()
    {   
        if (character.GetStatus(Character.Status.SHIELD)) shieldStatusIcon.color = Color.white;
        else shieldStatusIcon.color = Color.grey;

        if (character.GetStatus(Character.Status.BARRIER)) barrierStatusIcon.color = Color.white;
        else barrierStatusIcon.color = Color.grey;

        if (character.GetStatus(Character.Status.BLIND)) blindStatusIcon.color = Color.white;
        else blindStatusIcon.color = Color.grey;

        if (character.GetStatus(Character.Status.MUTE)) muteStatusIcon.color = Color.white;
        else muteStatusIcon.color = Color.grey;

        if (character.GetStatus(Character.Status.WEAK)) weakStatusIcon.color = Color.white;
        else weakStatusIcon.color = Color.grey;        
    }
}