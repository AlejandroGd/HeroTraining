using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDataDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextMeshProUGUI hpDisplay;
    [SerializeField] Image portraitDisplay;

     BattleManagerStateMachine bmsm;

    // Start is called before the first frame update
    void Start()
    {
        bmsm = GameObject.Find("Battle Manager State Machine").GetComponent<BattleManagerStateMachine>();
    }

    public void UpdateDataDisplay(GameObject charactergameObject)
    {
        //Put player menu                
        portraitDisplay.sprite = charactergameObject.GetComponent<SpriteRenderer>().sprite;
        portraitDisplay.type = Image.Type.Filled;
        portraitDisplay.preserveAspect = true;

        Character character = charactergameObject.GetComponent<Character>();
        nameDisplay.text = character.MenuName;
        string healthText = character.CurrentHealthPoints.ToString() + " / " + character.MaxHealthPoints.ToString();
        hpDisplay.text = healthText;       
    }
}