using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : CharacterAI
{
    //Character skills ordered by type(class)
    protected Dictionary<Skill.SkillType, List<GameObject>> skillsByClass = new Dictionary<Skill.SkillType, List<GameObject>>();

    public EnemyAI() { }

    //Just choose skill randomly from the available ones.
    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null) { return ChooseActionRandomly(skillList); }

    //Required overrides not needed for Enemy AI.
    public override void Learn(FightAIRecord data) { }
    public override void BuildAIProfile() { }
    public override int CountSkillRecords(int skillId) { return 1; }
    public override int TotalSkillRecords() { return 1; }

    //Required for Advanced Enemy AI

    //Constructor
    protected EnemyAI(List<GameObject> skillList)
    {
        skillsByClass = new Dictionary<Skill.SkillType, List<GameObject>>();

        foreach (GameObject skillpref in skillList)
        {
            Skill.SkillType skillType = skillpref.GetComponent<Skill>().SkillClass;
            if (!skillsByClass.ContainsKey(skillType))
            {
                skillsByClass.Add(skillType, new List<GameObject>());
            }
            skillsByClass[skillType].Add(skillpref);
        }
    }
        
    //Just return a random skill from the availables.
    protected GameObject ChooseActionRandomly(List<GameObject> skillList) { return skillList[Random.Range(0, skillList.Count)]; }

    //Returns a random skill from the specified type from the available skills for the character.
    protected GameObject ChooseSkillOfType(Skill.SkillType sType)
    {
        if (!skillsByClass.ContainsKey(sType)) return null;

        int numberOfSkills = skillsByClass[sType].Count;
        return skillsByClass[sType][Random.Range(0, numberOfSkills)];
    }

    //Assign a score to the available physical and magical skills the character has and returns the one that deals more damage to the player
    //(The OWN un the fight record) (Each character has at least one Physical or Magical attack)
    protected GameObject ChooseAttackSkill(FightAIRecord record)
    {
        GameObject skillPrefabToReturn = null;
        float highestScore = -500;

        float playerPhyDef = record.GetAttribValue(FightAIRecordAttributes.OWN_PHYSIC_DEFENSE);
        float playerMagDef = record.GetAttribValue(FightAIRecordAttributes.OWN_MAGIC_DEFENSE);
        float playerFirDef = record.GetAttribValue(FightAIRecordAttributes.OWN_FIRE_DEFENSE);
        float playerIceDef = record.GetAttribValue(FightAIRecordAttributes.OWN_ICE_DEFENSE);

        if (skillsByClass.ContainsKey(Skill.SkillType.PHYSIC))
        {
            foreach (GameObject skPref in skillsByClass[Skill.SkillType.PHYSIC])
            {
                Skill skill = skPref.GetComponent<Skill>();
                float score = 0;
                score -= playerPhyDef; //Higher overall points if player physical defense is low or negative;
                if (skill.Element == Skill.EElement.FIRE) score -= playerFirDef;
                if (skill.Element == Skill.EElement.ICE) score -= playerIceDef;

                if (highestScore < score)
                {
                    highestScore = score;
                    skillPrefabToReturn = skPref;
                }
            }
        }

        if (skillsByClass.ContainsKey(Skill.SkillType.MAGIC))
        {
            foreach (GameObject skPref in skillsByClass[Skill.SkillType.MAGIC])
            {
                Skill skill = skPref.GetComponent<Skill>();
                float score = 0;
                score -= playerMagDef; //Higher overall points if player physical defense is low or negative;
                if (skill.Element == Skill.EElement.FIRE) score -= playerFirDef;
                if (skill.Element == Skill.EElement.ICE) score -= playerIceDef;

                if (highestScore < score)
                {
                    highestScore = score;
                    skillPrefabToReturn = skPref;
                }
            }
        }

        return skillPrefabToReturn;
    }

    public override List<FightAIRecord> GetRecords()
    {
        return new List<FightAIRecord>();
    }

    public override void ForgetSkill(int skillID) { }
}


//Only heals periodically when health is too low. Gives preference to Attack skills.
public class EnemyAI_Aggresive : EnemyAI
{
    int healingTurns = 2;
    public EnemyAI_Aggresive(List<GameObject> skillList) : base(skillList) { }

    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null)
    {
        GameObject skillPrefabToReturn = null;
        if (record.GetAttribValue(FightAIRecordAttributes.ENEMY_HEALTH) < 0.25f)
        {
            if (healingTurns >= 2)
            {
                healingTurns = 0;
                skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.HEAL);
            }
            else healingTurns++;
        }

        if (skillPrefabToReturn == null)
        {
            //70% Attack / 10% Buff / 10% Debuff / 10% Random
            float choice = Random.Range(0.0f, 1.0f);
            if (choice < 0.7f)
            {
                skillPrefabToReturn = ChooseAttackSkill(record);
            }
            else if (choice < 0.8f) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.BUFF);
            else if (choice < 0.9f) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.DEBUFF);            
        }

        //If there is no action from the selected class, choose randomly.
        if (skillPrefabToReturn != null) return skillPrefabToReturn;
        else return ChooseActionRandomly(skillList);
    }    
}

//Heals on medium Health every other turn. Priority to Attacks and Buffs when not.
public class EnemyAI_Cautious : EnemyAI
{
    int healingTurns = 1;
    public EnemyAI_Cautious(List<GameObject> skillList) : base(skillList) { }

    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null)
    {
        GameObject skillPrefabToReturn = null;
        if (record.GetAttribValue(FightAIRecordAttributes.ENEMY_HEALTH) < 0.50f)
        {
            if (healingTurns >= 1)
            {
                healingTurns = 0;
                skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.HEAL);
            }
            else healingTurns++;
        }

        if (skillPrefabToReturn == null)
        {
            //40% Attack / 30% Buff / 20% Debuff / 10% Random
            float choice = Random.Range(0.0f, 1.0f);
            if (choice < 0.4f)
            {
                skillPrefabToReturn = ChooseAttackSkill(record);
            }
            else if (choice < 0.7) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.BUFF);
            else if (choice < 0.9) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.DEBUFF);
        }

        if (skillPrefabToReturn != null) return skillPrefabToReturn;
        else return ChooseActionRandomly(skillList);
    }
}

//Heals on medium to low health. Priority to debuffs and attacks.
public class EnemyAI_StatusDriven : EnemyAI
{
    int healingTurns = 2;
    public EnemyAI_StatusDriven(List<GameObject> skillList) : base(skillList) { }

    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null)
    {
        GameObject skillPrefabToReturn = null;
        if (record.GetAttribValue(FightAIRecordAttributes.ENEMY_HEALTH) < 0.40f)
        {
            if (healingTurns >= 2)
            {
                healingTurns = 0;
                skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.HEAL);
            }
            else healingTurns++;
        }

        if (skillPrefabToReturn == null)
        {
            //50% Attack / 30% Debuff / 10% Debuff / 10% Random
            float choice = Random.Range(0.0f, 1.0f);
            if (choice < 0.5f)
            {
                skillPrefabToReturn = ChooseAttackSkill(record);
            }
            else if (choice < 0.8) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.DEBUFF);
            else if (choice < 0.9) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.BUFF);
        }

        if (skillPrefabToReturn != null) return skillPrefabToReturn;
        else return ChooseActionRandomly(skillList);
    }
}

//Heals on medium low health. Balanced, but with a slight priority to attacks.
public class EnemyAI_Normal : EnemyAI
{
    int healingTurns = 2;
    public EnemyAI_Normal(List<GameObject> skillList) : base(skillList) { }

    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record = null)
    {
        GameObject skillPrefabToReturn = null;
        if (record.GetAttribValue(FightAIRecordAttributes.ENEMY_HEALTH) < 0.40f)
        {
            if (healingTurns >= 2)
            {
                healingTurns = 0;
                skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.HEAL);
            }
            else healingTurns++;
        }

        if (skillPrefabToReturn == null)
        {
            //30% Attack / 25% Debuff / 25% Debuff / 20% Random
            float choice = Random.Range(0.0f, 1.0f);
            if (choice < 0.3f)
            {
                skillPrefabToReturn = ChooseAttackSkill(record);
            }
            else if (choice < 0.55) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.DEBUFF);
            else if (choice < 0.8) skillPrefabToReturn = ChooseSkillOfType(Skill.SkillType.BUFF);
        }

        if (skillPrefabToReturn != null) return skillPrefabToReturn;
        else return ChooseActionRandomly(skillList);
    }
}