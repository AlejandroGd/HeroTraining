using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnDisplay : MonoBehaviour
{
    //Portraits for the turn images
    [SerializeField] List<Image> turnPortraits;

    BattleManagerStateMachine bmsm;

    private void Start()
    {
        bmsm = FindObjectOfType<BattleManagerStateMachine>();    
    }

    public void UpdateTurnDisplay()
    {
        for (int x=0; x < turnPortraits.Count; x++)
        {
            turnPortraits[x].sprite = bmsm.GetCharacterAtTurn(x + 1).GetComponent<SpriteRenderer>().sprite;
            turnPortraits[x].type = Image.Type.Filled;
            turnPortraits[x].preserveAspect = true;
        }
    }
}