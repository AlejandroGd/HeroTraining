using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class CharacterAI  
{    
    public abstract GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null);
    public abstract void BuildAIProfile();
    public abstract void Learn(FightAIRecord data);
    public abstract int CountSkillRecords(int skillId);
    public abstract int TotalSkillRecords();
    public abstract List<FightAIRecord> GetRecords();
    public abstract void ForgetSkill(int skillID);
}