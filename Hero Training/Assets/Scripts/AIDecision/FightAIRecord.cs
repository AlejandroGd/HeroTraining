using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Implementation of IComparer to order lists of records by attribute (Descendent)
public class FightAIRecord_SortByAtttribDescendent : IComparer<FightAIRecord>
{
    FightAIRecordAttributes attrib;

    public FightAIRecord_SortByAtttribDescendent(FightAIRecordAttributes attrib)
    {
        this.attrib = attrib;
    }

    public int Compare(FightAIRecord c1, FightAIRecord c2)
    {       
        if (c1.GetAttribValue(attrib) < c2.GetAttribValue(attrib)) return 1;    //x is more than y (so y go first when it is bigger)
        if (c1.GetAttribValue(attrib) > c2.GetAttribValue(attrib)) return -1;   //x is less than y (so x go first when it is bigger)
        else return 0;                                                          //equals
    }
}

//Attributes for a full Fight state record.
public enum FightAIRecordAttributes
{
    SKILL_USED = 0,
    OWN_HEALTH,
    OWN_RESIST_BLIND,
    OWN_RESIST_MUTE,
    OWN_RESIST_WEAK,
    OWN_PHYSIC_ATTACK,
    OWN_PHYSIC_DEFENSE,
    OWN_MAGIC_ATTACK,
    OWN_MAGIC_DEFENSE,
    OWN_FIRE_DEFENSE,
    OWN_ICE_DEFENSE,
    OWN_STATUS_BLIND,
    OWN_STATUS_MUTE,
    OWN_STATUS_WEAK,
    OWN_STATUS_SHIELD,
    OWN_STATUS_BARRIER,

    ALLY_HEALTH,
    ALLY_RESIST_BLIND,
    ALLY_RESIST_MUTE,
    ALLY_RESIST_WEAK,
    ALLY_PHYSIC_ATTACK,
    ALLY_PHYSIC_DEFENSE,
    ALLY_MAGIC_ATTACK,
    ALLY_MAGIC_DEFENSE,
    ALLY_FIRE_DEFENSE,
    ALLY_ICE_DEFENSE,
    ALLY_STATUS_BLIND,
    ALLY_STATUS_MUTE,
    ALLY_STATUS_WEAK,
    ALLY_STATUS_SHIELD,
    ALLY_STATUS_BARRIER,

    ENEMY_HEALTH,
    ENEMY_RESIST_BLIND,
    ENEMY_RESIST_MUTE,
    ENEMY_RESIST_WEAK,
    ENEMY_PHYSIC_ATTACK,
    ENEMY_PHYSIC_DEFENSE,
    ENEMY_MAGIC_ATTACK,
    ENEMY_MAGIC_DEFENSE,
    ENEMY_FIRE_DEFENSE,
    ENEMY_ICE_DEFENSE,
    ENEMY_STATUS_BLIND,
    ENEMY_STATUS_MUTE,
    ENEMY_STATUS_WEAK,
    ENEMY_STATUS_SHIELD,
    ENEMY_STATUS_BARRIER
}

//class to hold fight state (3 characters state + action)
[Serializable]
public class FightAIRecord
{
    //Class to hold one character state
    [Serializable]
    private class CharacterState
    {
        public CharacterState(Character character)
        {
            health = (float)character.CurrentHealthPoints / (float)character.MaxHealthPoints;

            resistBlind = character.ResistBlind;
            resistMute = character.ResistMute;
            resistWeak = character.ResistWeak;

            physicAttack = character.PhysicalAttack;
            physicDefense = character.PhysicalDefense;
            magicAttack = character.MagicAttack;
            magicDefense = character.MagicDefense;
            fireDefense = character.FireDefense;
            iceDefense = character.IceDefense;

            statusBlind = character.GetStatus(Character.Status.BLIND) ? 1f : 0f;
            statusMute = character.GetStatus(Character.Status.MUTE) ? 1f : 0f;
            statusWeak = character.GetStatus(Character.Status.WEAK) ? 1f : 0f;
            statusShield = character.GetStatus(Character.Status.SHIELD) ? 1f : 0f;
            statusBarrier = character.GetStatus(Character.Status.BARRIER) ? 1f : 0f;
        }
        public CharacterState() { }

        private float health; public float Health { get => health; set => health = value; }

        private float resistBlind; public float ResistBlind { get => resistBlind; set => resistBlind = value; }
        private float resistMute; public float ResistMute { get => resistMute; set => resistMute = value; }
        private float resistWeak; public float ResistWeak { get => resistWeak; set => resistWeak = value; }

        private float physicAttack; public float PhysicAttack { get => physicAttack; set => physicAttack = value; }
        private float physicDefense; public float PhysicDefense { get => physicDefense; set => physicDefense = value; }
        private float magicAttack; public float MagicAttack { get => magicAttack; set => magicAttack = value; }
        private float magicDefense; public float MagicDefense { get => magicDefense; set => magicDefense = value; }
        private float fireDefense; public float FireDefense { get => fireDefense; set => fireDefense = value; }
        private float iceDefense; public float IceDefense { get => iceDefense; set => iceDefense = value; }

        private float statusBlind; public float StatusBlind { get => statusBlind; set => statusBlind = value; }
        private float statusMute; public float StatusMute { get => statusMute; set => statusMute = value; }
        private float statusWeak; public float StatusWeak { get => statusWeak; set => statusWeak = value; }
        private float statusShield; public float StatusShield { get => statusShield; set => statusShield = value; }
        private float statusBarrier; public float StatusBarrier { get => statusBarrier; set => statusBarrier = value; }
    }

    private int skillUsed; public int SkillUsed { get => skillUsed; set => skillUsed = value; }

    private CharacterState ownState; 
    private CharacterState allyState;
    private CharacterState enemyState;

    public FightAIRecord(Character trainee, Character ally, Character enemy, Skill skill = null)
    {
        if (skill != null) skillUsed = skill.SkillID;
        else skillUsed = 0;
        ownState = new CharacterState(trainee);
        allyState = new CharacterState(ally);
        enemyState = new CharacterState(enemy);
    }

    
    public static bool IsAttributeBoolean(FightAIRecordAttributes attrib)
    {
        bool result = false;
        switch (attrib)
        {
            case FightAIRecordAttributes.OWN_STATUS_BLIND:
            case FightAIRecordAttributes.OWN_STATUS_MUTE:
            case FightAIRecordAttributes.OWN_STATUS_WEAK:
            case FightAIRecordAttributes.OWN_STATUS_SHIELD:
            case FightAIRecordAttributes.OWN_STATUS_BARRIER:

            case FightAIRecordAttributes.ALLY_STATUS_BLIND:
            case FightAIRecordAttributes.ALLY_STATUS_MUTE:
            case FightAIRecordAttributes.ALLY_STATUS_WEAK:
            case FightAIRecordAttributes.ALLY_STATUS_SHIELD:
            case FightAIRecordAttributes.ALLY_STATUS_BARRIER:

            case FightAIRecordAttributes.ENEMY_STATUS_BLIND:
            case FightAIRecordAttributes.ENEMY_STATUS_MUTE:
            case FightAIRecordAttributes.ENEMY_STATUS_WEAK:
            case FightAIRecordAttributes.ENEMY_STATUS_SHIELD:
            case FightAIRecordAttributes.ENEMY_STATUS_BARRIER:
                result = true;
                break;
        }
        return result;
    }
 
    //Returns the float value of the passed attribute
    public float GetAttribValue(FightAIRecordAttributes attrib)
    {
        switch (attrib)
        {
            case FightAIRecordAttributes.OWN_HEALTH: return ownState.Health; break;
            case FightAIRecordAttributes.OWN_RESIST_BLIND: return ownState.ResistBlind; break;
            case FightAIRecordAttributes.OWN_RESIST_MUTE: return ownState.ResistMute; break;
            case FightAIRecordAttributes.OWN_RESIST_WEAK: return ownState.ResistWeak; break;
            case FightAIRecordAttributes.OWN_PHYSIC_ATTACK: return ownState.PhysicAttack; break;
            case FightAIRecordAttributes.OWN_MAGIC_ATTACK: return ownState.MagicAttack; break;
            case FightAIRecordAttributes.OWN_PHYSIC_DEFENSE: return ownState.PhysicDefense; break;
            case FightAIRecordAttributes.OWN_MAGIC_DEFENSE: return ownState.MagicDefense; break;
            case FightAIRecordAttributes.OWN_ICE_DEFENSE: return ownState.IceDefense; break;
            case FightAIRecordAttributes.OWN_FIRE_DEFENSE: return ownState.FireDefense; break;
            case FightAIRecordAttributes.OWN_STATUS_BLIND: return ownState.StatusBlind; break;
            case FightAIRecordAttributes.OWN_STATUS_MUTE: return ownState.StatusMute; break;
            case FightAIRecordAttributes.OWN_STATUS_WEAK: return ownState.StatusWeak; break;
            case FightAIRecordAttributes.OWN_STATUS_SHIELD: return ownState.StatusShield; break;
            case FightAIRecordAttributes.OWN_STATUS_BARRIER: return ownState.StatusBarrier; break;


            case FightAIRecordAttributes.ALLY_HEALTH: return allyState.Health; break;
            case FightAIRecordAttributes.ALLY_RESIST_BLIND: return allyState.ResistBlind; break;
            case FightAIRecordAttributes.ALLY_RESIST_MUTE: return allyState.ResistMute; break;
            case FightAIRecordAttributes.ALLY_RESIST_WEAK: return allyState.ResistWeak; break;
            case FightAIRecordAttributes.ALLY_PHYSIC_ATTACK: return allyState.PhysicAttack; break;
            case FightAIRecordAttributes.ALLY_MAGIC_ATTACK: return allyState.MagicAttack; break;
            case FightAIRecordAttributes.ALLY_PHYSIC_DEFENSE: return allyState.PhysicDefense; break;
            case FightAIRecordAttributes.ALLY_MAGIC_DEFENSE: return allyState.MagicDefense; break;
            case FightAIRecordAttributes.ALLY_ICE_DEFENSE: return allyState.IceDefense; break;
            case FightAIRecordAttributes.ALLY_FIRE_DEFENSE: return allyState.FireDefense; break;
            case FightAIRecordAttributes.ALLY_STATUS_BLIND: return allyState.StatusBlind; break;
            case FightAIRecordAttributes.ALLY_STATUS_MUTE: return allyState.StatusMute; break;
            case FightAIRecordAttributes.ALLY_STATUS_WEAK: return allyState.StatusWeak; break;
            case FightAIRecordAttributes.ALLY_STATUS_SHIELD: return allyState.StatusShield; break;
            case FightAIRecordAttributes.ALLY_STATUS_BARRIER: return allyState.StatusBarrier; break;

            case FightAIRecordAttributes.ENEMY_HEALTH: return enemyState.Health; break;
            case FightAIRecordAttributes.ENEMY_RESIST_BLIND: return enemyState.ResistBlind; break;
            case FightAIRecordAttributes.ENEMY_RESIST_MUTE: return enemyState.ResistMute; break;
            case FightAIRecordAttributes.ENEMY_RESIST_WEAK: return enemyState.ResistWeak; break;
            case FightAIRecordAttributes.ENEMY_PHYSIC_ATTACK: return enemyState.PhysicAttack; break;
            case FightAIRecordAttributes.ENEMY_MAGIC_ATTACK: return enemyState.MagicAttack; break;
            case FightAIRecordAttributes.ENEMY_PHYSIC_DEFENSE: return enemyState.PhysicDefense; break;
            case FightAIRecordAttributes.ENEMY_MAGIC_DEFENSE: return enemyState.MagicDefense; break;
            case FightAIRecordAttributes.ENEMY_ICE_DEFENSE: return enemyState.IceDefense; break;
            case FightAIRecordAttributes.ENEMY_FIRE_DEFENSE: return enemyState.FireDefense; break;
            case FightAIRecordAttributes.ENEMY_STATUS_BLIND: return enemyState.StatusBlind; break;
            case FightAIRecordAttributes.ENEMY_STATUS_MUTE: return enemyState.StatusMute; break;
            case FightAIRecordAttributes.ENEMY_STATUS_WEAK: return enemyState.StatusWeak; break;
            case FightAIRecordAttributes.ENEMY_STATUS_SHIELD: return enemyState.StatusShield; break;
            case FightAIRecordAttributes.ENEMY_STATUS_BARRIER: return enemyState.StatusBarrier; break;

            default: return skillUsed; break; //0 or other value return class value (skill id)
        }
    }
    private void SetAttribValue(FightAIRecordAttributes attrib, float value)
    {
        switch (attrib)
        {
            case FightAIRecordAttributes.SKILL_USED: skillUsed = (int)value; break;

            case FightAIRecordAttributes.OWN_HEALTH: ownState.Health = value; break;
            case FightAIRecordAttributes.OWN_RESIST_BLIND: ownState.ResistBlind = value; break;
            case FightAIRecordAttributes.OWN_RESIST_MUTE: ownState.ResistMute = value; break;
            case FightAIRecordAttributes.OWN_RESIST_WEAK: ownState.ResistWeak = value; break;
            case FightAIRecordAttributes.OWN_PHYSIC_ATTACK: ownState.PhysicAttack = value; break;
            case FightAIRecordAttributes.OWN_MAGIC_ATTACK: ownState.MagicAttack = value; break;
            case FightAIRecordAttributes.OWN_PHYSIC_DEFENSE: ownState.PhysicDefense = value; break;
            case FightAIRecordAttributes.OWN_MAGIC_DEFENSE: ownState.MagicDefense = value; break;
            case FightAIRecordAttributes.OWN_ICE_DEFENSE: ownState.IceDefense = value; break;
            case FightAIRecordAttributes.OWN_FIRE_DEFENSE: ownState.FireDefense = value; break;
            case FightAIRecordAttributes.OWN_STATUS_BLIND: ownState.StatusBlind = value; break;
            case FightAIRecordAttributes.OWN_STATUS_MUTE: ownState.StatusMute = value; break;
            case FightAIRecordAttributes.OWN_STATUS_WEAK: ownState.StatusWeak = value; break;
            case FightAIRecordAttributes.OWN_STATUS_SHIELD: ownState.StatusShield = value; break;
            case FightAIRecordAttributes.OWN_STATUS_BARRIER: ownState.StatusBarrier = value; break;


            case FightAIRecordAttributes.ALLY_HEALTH: allyState.Health = value; break;
            case FightAIRecordAttributes.ALLY_RESIST_BLIND: allyState.ResistBlind = value; break;
            case FightAIRecordAttributes.ALLY_RESIST_MUTE: allyState.ResistMute = value; break;
            case FightAIRecordAttributes.ALLY_RESIST_WEAK: allyState.ResistWeak = value; break;
            case FightAIRecordAttributes.ALLY_PHYSIC_ATTACK: allyState.PhysicAttack = value; break;
            case FightAIRecordAttributes.ALLY_MAGIC_ATTACK: allyState.MagicAttack = value; break;
            case FightAIRecordAttributes.ALLY_PHYSIC_DEFENSE: allyState.PhysicDefense = value; break;
            case FightAIRecordAttributes.ALLY_MAGIC_DEFENSE: allyState.MagicDefense = value; break;
            case FightAIRecordAttributes.ALLY_ICE_DEFENSE: allyState.IceDefense = value; break;
            case FightAIRecordAttributes.ALLY_FIRE_DEFENSE: allyState.FireDefense = value; break;
            case FightAIRecordAttributes.ALLY_STATUS_BLIND: allyState.StatusBlind = value; break;
            case FightAIRecordAttributes.ALLY_STATUS_MUTE: allyState.StatusMute = value; break;
            case FightAIRecordAttributes.ALLY_STATUS_WEAK: allyState.StatusWeak = value; break;
            case FightAIRecordAttributes.ALLY_STATUS_SHIELD: allyState.StatusShield = value; break;
            case FightAIRecordAttributes.ALLY_STATUS_BARRIER: allyState.StatusBarrier = value; break;

            case FightAIRecordAttributes.ENEMY_HEALTH: enemyState.Health = value; break;
            case FightAIRecordAttributes.ENEMY_RESIST_BLIND: enemyState.ResistBlind = value; break;
            case FightAIRecordAttributes.ENEMY_RESIST_MUTE: enemyState.ResistMute = value; break;
            case FightAIRecordAttributes.ENEMY_RESIST_WEAK: enemyState.ResistWeak = value; break;
            case FightAIRecordAttributes.ENEMY_PHYSIC_ATTACK: enemyState.PhysicAttack = value; break;
            case FightAIRecordAttributes.ENEMY_MAGIC_ATTACK: enemyState.MagicAttack = value; break;
            case FightAIRecordAttributes.ENEMY_PHYSIC_DEFENSE: enemyState.PhysicDefense = value; break;
            case FightAIRecordAttributes.ENEMY_MAGIC_DEFENSE: enemyState.MagicDefense = value; break;
            case FightAIRecordAttributes.ENEMY_ICE_DEFENSE: enemyState.IceDefense = value; break;
            case FightAIRecordAttributes.ENEMY_FIRE_DEFENSE: enemyState.FireDefense = value; break;
            case FightAIRecordAttributes.ENEMY_STATUS_BLIND: enemyState.StatusBlind = value; break;
            case FightAIRecordAttributes.ENEMY_STATUS_MUTE: enemyState.StatusMute = value; break;
            case FightAIRecordAttributes.ENEMY_STATUS_WEAK: enemyState.StatusWeak = value; break;
            case FightAIRecordAttributes.ENEMY_STATUS_SHIELD: enemyState.StatusShield = value; break;
            case FightAIRecordAttributes.ENEMY_STATUS_BARRIER: enemyState.StatusBarrier = value; break;            
        }
    }

    public int GetAttribValue_Int(FightAIRecordAttributes attrib) { return (int)GetAttribValue(attrib); }
    public bool GetAttribValue_Bool(FightAIRecordAttributes attrib) { return GetAttribValue(attrib) == 1f; }

    //Testing and automatic Player AI Creation
    private float[] PrintToArray()
    {
        float[] array = new float[46];
        for (int x = 0; x < 46; x++)
        {
            array[x] = GetAttribValue((FightAIRecordAttributes)x);
        }
        return array;
    }
    public string PrintToString()
    {
        float[] array = PrintToArray();
        string temp = "{" + array[0];
        for (int x = 1; x < array.Length; x++)
        {
            temp += ", " + array[x] + "f";
        }
        temp += "}";

        return temp;
    }
    
    public FightAIRecord (float[] array)
    {
        ownState = new CharacterState();
        allyState = new CharacterState();
        enemyState = new CharacterState();
        for (int x = 0; x < 46; x++)
        {
           SetAttribValue((FightAIRecordAttributes)x, array[x]);
        }       
    }

    //Copy constructor
    public FightAIRecord(FightAIRecord aiRecord)
    {
        ownState = new CharacterState();
        allyState = new CharacterState();
        enemyState = new CharacterState();
        for (int x = 0; x < 46; x++)
        {
            SetAttribValue((FightAIRecordAttributes)x, aiRecord.GetAttribValue((FightAIRecordAttributes)x));
        }
    }
}