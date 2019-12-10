using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Class for Basic Player AI
 * It just records the number of times a skill has been used and chooses an action based 
 * on a wheighted probabilty proportional to that data.
 */
 [System.Serializable]
public class AllyBasicAI : CharacterAI
{
    int totalRecords = 0;
    Dictionary<int, int> knowledge; //Pairs of < Skill ID , Times used >

    //Constructor
    public AllyBasicAI() { ResetKnowledge(); }

    //Initialisation.
    public void ResetKnowledge() { knowledge = new Dictionary<int, int>(); }

    //Basic Ally AI does not need to build an AI profile
    public override void BuildAIProfile() { }

    //The Basic AI will count the number of times a skill has been used by the player.
    public override void Learn(FightAIRecord data)
    {
        if (knowledge.ContainsKey((int)data.GetAttribValue(FightAIRecordAttributes.SKILL_USED)))
        {
            knowledge[(int)data.GetAttribValue(FightAIRecordAttributes.SKILL_USED)] += 1;
        }
        else
        {
            knowledge.Add((int)data.GetAttribValue(FightAIRecordAttributes.SKILL_USED), 1);
        }
        totalRecords++;
    }

    public override int TotalSkillRecords() { return totalRecords; }

    //Choose a skill with a weighted probability based on the number of times each skill has been used. 
    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null)
    {
        GameObject skill = null;
        int totalKnowledgeCases = 0;

        //Count how many cases of the available skills does the AI has. 
        //(Avoid errors due to Skill reassignation or change in prefabs with the inspector)
        foreach (GameObject skillObject in skillList)
        {
            int skillID = skillObject.GetComponent<Skill>().SkillID;
            if (knowledge.ContainsKey(skillID)) totalKnowledgeCases += knowledge[skillID];
        }

        //If there is no knowledge of the skills passed, just choose randomly.
        if (totalKnowledgeCases == 0) return skillList[Random.Range(0, skillList.Count)];

        //Weighted Random from the skills both in knowledge and the skill list.
        int randomValue = Random.Range(1, totalKnowledgeCases + 1);
        int skillIDChosen = -1;
        foreach (KeyValuePair<int, int> idTimesPair in knowledge)
        {
            randomValue -= idTimesPair.Value;
            if (randomValue <= 0)
            {
                skillIDChosen = idTimesPair.Key;
                break;
            }
        }

        //Now the Skill ID is known just find the skill with that ID
        foreach (GameObject skillObject in skillList)
        {
            int skillID = skillObject.GetComponent<Skill>().SkillID;
            if (skillID == skillIDChosen)
            {
                skill = skillObject;
                break;
            }
        }

        return skill;
    }

    //Number of times used
    public override int CountSkillRecords(int skillId) { return knowledge.ContainsKey(skillId) ? knowledge[skillId] : 0; }

    public override List<FightAIRecord> GetRecords()
    {
        return new List<FightAIRecord>();
    }

    public override void ForgetSkill(int skillID)
    {
        if (knowledge.ContainsKey(skillID)) knowledge.Remove(skillID);
    }
}