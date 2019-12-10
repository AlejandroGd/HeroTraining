using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class container for actions. Specifies character attacking, target and skill used.
//For skills that may result in status (Weak, Barrier, etc.) it also includes a flag indicating if the status will be applied or not.
public class BattleAction
{
    public GameObject Attacker;
    public GameObject Target;
    public GameObject Action;
    public bool SkillSucceded = false;
}