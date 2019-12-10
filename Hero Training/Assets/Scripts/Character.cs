using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{ 
    //Reference to the BattleManager State Machine
    BattleManagerStateMachine bsStateMachine;
    
    public enum Status
    {
        SHIELD = 0,
        BARRIER = 1,
        BLIND = 2,
        MUTE = 3,
        WEAK = 4,
        NONE = 5
    }

    [Header("Main")]
    [SerializeField] string menuName = "";
    [SerializeField] string characterName = "";
    [SerializeField] [TextArea] string characterDescription = "";
    [SerializeField] [Range(0, 3000)] int maxHealthPoints = 300;
    
    [SerializeField] private int currentHealthPoints = 300;
    [SerializeField] List<GameObject> skillPrefabList;
    [Header("Attack/Defense")]
    [SerializeField] [Range(0, 100)] int physicalAttack = 0;
    [SerializeField] [Range(0, 100)] int magicAttack = 0;
    [SerializeField] [Range(-100, 100)] int physicalDefense = 0;    
    [SerializeField] [Range(-100, 100)] int magicDefense = 0;
    [SerializeField] [Range(-100, 100)] int fireDefense = 0;
    [SerializeField] [Range(-100, 100)] int iceDefense = 0;
    [Header("Resistances")]
    [SerializeField] [Range(0, 100)] float resistWeak = 0;    
    [SerializeField] [Range(0, 100)] float resistMute = 0;
    [SerializeField] [Range(0, 100)] float resistBlind = 0;   

    //Accessors
    public string MenuName { get => menuName; }
    public string CharacterName { get => characterName; }
    public string CharacterDescription { get => characterDescription; }
    public int MaxHealthPoints { get => maxHealthPoints; }
    public int CurrentHealthPoints { get => currentHealthPoints; }
    public List<GameObject> SkillPrefabList { get => skillPrefabList; }
    public int PhysicalAttack { get => physicalAttack; }
    public int PhysicalDefense { get => physicalDefense; }
    public int MagicAttack { get => magicAttack; }
    public int MagicDefense { get => magicDefense; }
    public int FireDefense { get => fireDefense; }
    public int IceDefense { get => iceDefense; }
    public float ResistWeak { get => resistWeak; }
    public float ResistMute { get => resistMute; }
    public float ResistBlind { get => resistBlind; }


    //Indicates if buffs/debuffs are active or not    
    [SerializeField] int[] characterStatus;    
    
    //Player controlled / Ally flags
    [SerializeField] bool playerControlled = false;
    public bool PlayerControlled { get => playerControlled; set => playerControlled = value; }
    [SerializeField] bool isAlly = false;
    public bool IsAlly { get => isAlly; set => isAlly = value; }


    //AI (Random choice so far)
    CharacterAI characterAI; public CharacterAI CharacterAI { get => characterAI; set => characterAI = value; }
        
    //Buff/Debuff
    public bool GetStatus(Status status)
    {        
        return characterStatus[(int)status] > 0;
    }
    public bool ApplyStatus(Status status, int turns)
    {
        //Status counters decrement when the character turn becomes active. I.E: Having the counter at 3 will keep the status active for 2 full turns, and the 3rd will become 0 (inactive). 
        //However, to clear the statuses (turns = 0) we just put it to 0;
        if (turns != 0) characterStatus[(int)status] = turns + 1;
        else characterStatus[(int)status] = 0;
        return true;
    }
    public void DiscountTurnForStatus()
    {
       for (int x =0; x < characterStatus.Length; x++)
        {
            if (characterStatus[x] > 0) characterStatus[x]--;
        }
    }

   
    // Start is called before the first frame update    
    void Start()
    {
        bsStateMachine = GameObject.Find("Battle Manager State Machine").GetComponent<BattleManagerStateMachine>();
        InitialiseParameters();
    }

    public bool IsDead() { return currentHealthPoints <= 0; }

    public void ChooseAction()
    {
        GameObject skillPrefab;
        FightAIRecord stateRecord = null;

        if (gameObject.tag == "Ally")
        {
            stateRecord = new FightAIRecord(bsStateMachine.GetCharacterComponent_Ally(), bsStateMachine.GetCharacterComponent_Player(), bsStateMachine.GetCharacterComponent_Enemy());            
        }
        else
        {
            //Enemy AI tailor its attacks to the resistances of the 1st character passed in a fightRecord.
            if(!bsStateMachine.GetCharacterComponent_Player().IsDead())
            {
                stateRecord = new FightAIRecord(bsStateMachine.GetCharacterComponent_Player(), bsStateMachine.GetCharacterComponent_Ally(), bsStateMachine.GetCharacterComponent_Enemy());
            }
            else
            {
                stateRecord = new FightAIRecord(bsStateMachine.GetCharacterComponent_Ally(), bsStateMachine.GetCharacterComponent_Player(), bsStateMachine.GetCharacterComponent_Enemy());
            }
            
        }

        skillPrefab = characterAI.ChooseAction(skillPrefabList, stateRecord);

        Skill skill = skillPrefab.GetComponent<Skill>();

        //If the skill is a buff apply to its team (ally to all allies, enemy to all enemies).
        //If not, apply to the opponet team.
        if (   (gameObject.tag == "Ally" && (skill.SkillClass == Skill.SkillType.BUFF || skill.SkillClass == Skill.SkillType.HEAL) )
            || (gameObject.tag == "Enemy" && (skill.SkillClass != Skill.SkillType.BUFF && skill.SkillClass != Skill.SkillType.HEAL)))
        {
            ApplySkillToAllies(skillPrefab);
        }
        else
        {
            ApplySkillToEnemies(skillPrefab);
        }        

        bsStateMachine.currentBattleState = BattleState.PROCESS;      
    }

    //Add a BattleAction from this character for all of the allies.
    public void ApplySkillToAllies(GameObject skill)
    {
        foreach ( GameObject ally in bsStateMachine.AlliesList)
        {
            //If target is not dead
            if (ally.GetComponent<Character>().CurrentHealthPoints > 0)
            {
                //Create the action data
                BattleAction battleAction = new BattleAction();
                battleAction.Attacker = gameObject;
                battleAction.Action = skill;
                battleAction.Target = ally;

                //Add to the action queue
                bsStateMachine.ActionQueue.Add(battleAction);
            }
        }
    }

    //Add a BattleAction from this character for all of the enemies.
    public void ApplySkillToEnemies(GameObject skill)
    {
        foreach (GameObject enemy in bsStateMachine.EnemiesList)
        {
            //If target is not dead
            if (enemy.GetComponent<Character>().CurrentHealthPoints > 0)
            {
                //Create the action data
                BattleAction battleAction = new BattleAction();
                battleAction.Attacker = gameObject;
                battleAction.Action = skill;
                battleAction.Target = enemy;

                //Add to the action queue
                bsStateMachine.ActionQueue.Add(battleAction);
            }
        }
    }



    //Initialise the status array.
    public void InitialiseParameters()
    {
        characterStatus = new int[5];// From the Status enum,  SHIELD = 0, BARRIER = 1, BLIND = 2, MUTE = 3, WEAK = 4
    }   

    private void DealDamage(int amount) { currentHealthPoints = Mathf.Clamp(currentHealthPoints - amount, 0, maxHealthPoints); }
    public void Heal(int amount) { currentHealthPoints = Mathf.Clamp(currentHealthPoints + amount, 0, maxHealthPoints); }

    
    public bool ApplySkill(Character attacker, Skill skill)
    {
        switch (skill.SkillClass)
        {
            case Skill.SkillType.PHYSIC:   return ApplyPhisicalSkill(attacker, skill);                
            case Skill.SkillType.MAGIC:    return ApplyMagicalSkill(attacker, skill);                
            case Skill.SkillType.BUFF:     return ApplyBuffSkill(skill);
            case Skill.SkillType.DEBUFF:   return ApplyDebuffSkill(skill);
            case Skill.SkillType.HEAL:     return ApplyHealSkill(skill);
            default:                        return false;                
        }
    }

    //Physical params used
    private bool ApplyPhisicalSkill(Character attacker, Skill skill)
    {
        //Blind status on attacker may result in no damage.
        if (attacker.GetStatus(Status.BLIND))
        {
            int miss = Random.Range(1, 10);
            if (miss <= 5) return true;
        }

        //Damage depends on attacker physical damage.
        float damageToDeal = (float)skill.Damage * (float)attacker.PhysicalAttack / 100f;

        //Damage can be reduced / increased depending on the physical defense of the attacked character.
        damageToDeal -= damageToDeal * ((float)physicalDefense /100f);

        //Element also can reduce / increase damage.      
        switch (skill.Element)
        {
            case Skill.EElement.FIRE:
                damageToDeal -= damageToDeal * ((float)fireDefense / 100f); // Positive defense decreases attack, negative defense increases it
                break;
            case Skill.EElement.ICE:
                damageToDeal -= damageToDeal * ((float)iceDefense / 100f); // Positive defense decreases attack, negative defense increases it
                break;
            default:
                break;
        }

        //A shield on this character halves the damage
        if (GetStatus(Status.SHIELD)) damageToDeal /= 2;

        //Weak status on this character doubles damage.
        if (GetStatus(Status.WEAK)) damageToDeal *= 2;
        
        this.DealDamage((int)damageToDeal);

        return true;
    }

    //Magical params used
    private bool ApplyMagicalSkill(Character attacker, Skill skill)
    {
        //Mute status on attacker may result in no damage.
        if (attacker.GetStatus(Status.MUTE))
        {
            int miss = Random.Range(1, 10);
            if (miss <= 5) return true;
        }

        //Damage depends on attacker physical damage.
        float damageToDeal = (float)skill.Damage * (float)attacker.MagicAttack / 100f;

        //Damage can be reduced / increased depending on the physical defense of the attacked character.
        damageToDeal -= damageToDeal * ((float)magicDefense / 100f);

        //Element also can reduce / increase damage.       
        switch (skill.Element)
        {
            case Skill.EElement.FIRE:
                damageToDeal -= damageToDeal * ((float)fireDefense / 100f); // Positive defense decreases attack, negative defense increases it
                break;
            case Skill.EElement.ICE:
                damageToDeal -= damageToDeal * ((float)iceDefense / 100f); // Positive defense decreases attack, negative defense increases it
                break;
            default:
                break;
        }
               
        //A barrier halves the damage
        if (GetStatus(Status.BARRIER)) damageToDeal /= 2;

        //Weak status doubles damage.
        if (GetStatus(Status.WEAK)) damageToDeal *= 2;

        this.DealDamage((int)damageToDeal);

        return true;
    }

    //Buffs always apply
    private bool ApplyBuffSkill(Skill skill)
    {
        ApplyStatus(skill.Status, skill.Turns);
        return true;
    }

    //Debuffs check resistances before applying
    private bool ApplyDebuffSkill(Skill skill)
    {
        bool statusApplied = false;
        int randomNumber = UnityEngine.Random.Range(0, 100);
        switch (skill.Status)
        {
            case Status.WEAK:
                if (randomNumber > resistWeak) { statusApplied = true; }
                break;            
            case Status.MUTE:
                if (randomNumber > resistMute) { statusApplied = true; }
                break;
            case Status.BLIND:
                if (randomNumber > resistBlind) { statusApplied = true; }
                break;
            default:
                Debug.Log("Invalid status to apply for skill");
                break;
        }

        if (statusApplied)
        {
            ApplyStatus(skill.Status, skill.Turns);
        }

        return statusApplied;
    }

    //Healing skills
    private bool ApplyHealSkill(Skill skill)
    {
        if (skill.SkillID == 43) //Remedy skill, clear negative statuses
        {
            ApplyStatus(Status.BLIND, 0);
            ApplyStatus(Status.MUTE, 0);
            ApplyStatus(Status.WEAK, 0);
        }
        else this.Heal(Mathf.Abs(skill.Damage));
        
        return true;
    }

    
   
    private void OnMouseEnter()
    {
        if (bsStateMachine.currentBattleState == BattleState.WAITING_FOR_PLAYER_INPUT)
            bsStateMachine.CharacterStatsDisplay.ShowInfo(this);
    }

    private void OnMouseExit()
    {
        if (bsStateMachine.currentBattleState == BattleState.WAITING_FOR_PLAYER_INPUT)
            bsStateMachine.CharacterStatsDisplay.ClearInfo();
    }    
}