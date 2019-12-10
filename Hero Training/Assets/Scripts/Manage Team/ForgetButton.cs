using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgetButton : MonoBehaviour
{
    int skillIDToForget = 0;
    [SerializeField] CharacterSelector charSel;
    
    public void UpdateForgetButtonData(int skillID)
    {
        skillIDToForget = skillID;
    }

    public void ForgetLinkedSkill()
    {
        charSel.ForgetSkill(skillIDToForget);
    }    
}