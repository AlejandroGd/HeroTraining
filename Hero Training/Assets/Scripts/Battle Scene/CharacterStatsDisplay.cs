using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatsDisplay : MonoBehaviour
{
    //Custom Colours for the text
    Color32 darkGreen = new Color32(15,60,5,255);
    Color32 darkRed = new Color32(180,10,10,255);
           
    Image Image_P_Atk;
    Image Image_P_Def;
    Image Image_M_Atk;
    Image Image_M_Def;
    Image Image_Ice_Res;
    Image Image_Fire_Res;
    Image Image_Blind_Res;
    Image Image_Mute_Res;
    Image Image_Weak_Res;

    TextMeshProUGUI Text_P_Atk;
    TextMeshProUGUI Text_P_Def;
    TextMeshProUGUI Text_M_Atk;
    TextMeshProUGUI Text_M_Def;
    TextMeshProUGUI Text_Ice_Res;
    TextMeshProUGUI Text_Fire_Res;
    TextMeshProUGUI Text_Blind_Res;
    TextMeshProUGUI Text_Mute_Res;
    TextMeshProUGUI Text_Weak_Res;

    void Start()
    {
        Image_P_Atk = GameObject.Find("P_Attack_Image").GetComponent<Image>();
        Image_P_Def = GameObject.Find("P_Defense_Image").GetComponent<Image>();
        Image_M_Atk = GameObject.Find("M_Attack_Image").GetComponent<Image>();
        Image_M_Def = GameObject.Find("M_Defense_Image").GetComponent<Image>();
        Image_Fire_Res = GameObject.Find("Fire_Res_Image").GetComponent<Image>();
        Image_Ice_Res = GameObject.Find("Ice_Res_Image").GetComponent<Image>();
        Image_Blind_Res = GameObject.Find("Blind_Res_Image").GetComponent<Image>();
        Image_Mute_Res = GameObject.Find("Mute_Res_Image").GetComponent<Image>();
        Image_Weak_Res = GameObject.Find("Weak_Res_Image").GetComponent<Image>();

        Text_P_Atk = GameObject.Find("P_Attack_Value").GetComponent<TextMeshProUGUI>();
        Text_P_Def = GameObject.Find("P_Defense_Value").GetComponent<TextMeshProUGUI>();
        Text_M_Atk = GameObject.Find("M_Attack_Value").GetComponent<TextMeshProUGUI>();
        Text_M_Def = GameObject.Find("M_Defense_Value").GetComponent<TextMeshProUGUI>();
        Text_Fire_Res = GameObject.Find("Fire_Res_Value").GetComponent<TextMeshProUGUI>();
        Text_Ice_Res = GameObject.Find("Ice_Res_Value").GetComponent<TextMeshProUGUI>();
        Text_Blind_Res = GameObject.Find("Blind_Res_Value").GetComponent<TextMeshProUGUI>();
        Text_Mute_Res = GameObject.Find("Mute_Res_Value").GetComponent<TextMeshProUGUI>();
        Text_Weak_Res = GameObject.Find("Weak_Res_Value").GetComponent<TextMeshProUGUI>();

        ClearInfo();
    }

    //Shows values for an specific character stats,
    public void ShowInfo(Character character)
    {
       
        //Darken the image a bit
        Image_P_Atk.color = new Color(255, 255, 255, 255);
        Image_P_Def.color = new Color(255, 255, 255, 255);
        Image_M_Atk.color = new Color(255, 255, 255, 255);
        Image_M_Def.color = new Color(255, 255, 255, 255);
        Image_Ice_Res.color = new Color(255, 255, 255, 255);
        Image_Fire_Res.color = new Color(255, 255, 255, 255);
        Image_Blind_Res.color = new Color(255, 255, 255, 255);
        Image_Mute_Res.color = new Color(255, 255, 255, 255);
        Image_Weak_Res.color = new Color(255, 255, 255, 255);

        //Values
        Text_P_Atk.text = character.PhysicalAttack.ToString();
        Text_P_Def.text = ((int)Mathf.Abs(character.PhysicalDefense)).ToString();
        Text_M_Atk.text = character.MagicAttack.ToString();
        Text_M_Def.text = ((int)Mathf.Abs(character.MagicDefense)).ToString();        
        Text_Ice_Res.text = ((int)Mathf.Abs(character.IceDefense)).ToString();
        Text_Fire_Res.text = ((int)Mathf.Abs(character.FireDefense)).ToString();
        Text_Blind_Res.text = character.ResistBlind.ToString();
        Text_Mute_Res.text = character.ResistMute.ToString();
        Text_Weak_Res.text = character.ResistWeak.ToString();

        //Elemental resistances can be negative (red)
        if (character.IceDefense < 0) Text_Ice_Res.color = darkRed;
        else Text_Ice_Res.color = darkGreen;
        if (character.FireDefense < 0) Text_Fire_Res.color = darkRed;
        else Text_Fire_Res.color = darkGreen;
        if (character.MagicDefense < 0) Text_M_Def.color = darkRed;
        else Text_M_Def.color = darkGreen;
        if (character.PhysicalDefense < 0) Text_P_Def.color = darkRed;
        else Text_P_Def.color = darkGreen;
    }

    //Clear images and text (So it looks nothing is there)
    public void ClearInfo()
    {       
        //Make images transparent
        Image_P_Atk.color = new Color(0, 0, 0, 0);
        Image_P_Def.color = new Color(0, 0, 0, 0);
        Image_M_Atk.color = new Color(0, 0, 0, 0);
        Image_M_Def.color = new Color(0, 0, 0, 0);
        Image_Ice_Res.color = new Color(0, 0, 0, 0);
        Image_Fire_Res.color = new Color(0, 0, 0, 0);
        Image_Blind_Res.color = new Color(0, 0, 0, 0);
        Image_Mute_Res.color = new Color(0, 0, 0, 0);
        Image_Weak_Res.color = new Color(0, 0, 0, 0);

        //Delete Texts
        Text_P_Atk.text = "";
        Text_P_Def.text = "";
        Text_M_Atk.text = "";
        Text_M_Def.text = "";
        Text_Ice_Res.text = "";
        Text_Fire_Res.text = "";
        Text_Blind_Res.text = "";
        Text_Mute_Res.text = "";
        Text_Weak_Res.text = "";
    }    
}