using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{ 
    //Reference to the BattleManager State Machine
    //BattleManagerStateMachine bsStateMachine;
    BattleManager bsStateMachine;

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
    public string CharacterName { get => characterName; set => characterName = value; }
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

    //Initialise the status array.
    public void InitialiseParameters()
    {
        characterStatus = new int[5];// From the Status enum,  SHIELD = 0, BARRIER = 1, BLIND = 2, MUTE = 3, WEAK = 4
    }

    //AI
    CharacterAI characterAI; public CharacterAI CharacterAI { get => characterAI; set => characterAI = value; }
        
    //Buff/Debuff
    public bool GetStatus(Status status)
    {        
        return characterStatus[(int)status] > 0;
    }

    public bool ApplyStatus(Status status, int turns)
    {
        //Status counters should decrement when the character turn becomes active.
        if (turns != 0) characterStatus[(int)status] = turns;
        else characterStatus[(int)status] = 0;
        return true;
    }

    public void DiscountTurnForAllStatus()
    {
       for (int x =0; x < characterStatus.Length; x++)
        {
            if (characterStatus[x] > 0) characterStatus[x]--;
        }
    }
    
    public bool IsDead() { return currentHealthPoints <= 0; }

    public GameObject ChooseAction(FightAIRecord stateRecord)
    {
        GameObject skillPrefab = characterAI.ChooseAction(skillPrefabList, stateRecord);
        return skillPrefab;
    }

    public bool ApplySkill(Character attacker, Skill skill)
    {
        switch (skill.SkillClass)
        {
            case Skill.SkillType.PHYSIC: return ApplyPhisicalSkill(attacker, skill);
            case Skill.SkillType.MAGIC: return ApplyMagicalSkill(attacker, skill);
            case Skill.SkillType.BUFF: return ApplyBuffSkill(skill);
            case Skill.SkillType.DEBUFF: return ApplyDebuffSkill(skill);
            case Skill.SkillType.HEAL: return HealAllStatus(skill);
            default: return false;
        }
    }


    // Start is called before the first frame update    
    void Start()
    {
        bsStateMachine = FindObjectOfType<BattleManager>();       
    }

    //Battle manager instantiate character objects on start.
    //We initialize on awake so the status data is availanble straight away and not on the next loop the gameObject is active, which is when start would be called.
    private void Awake()
    {
        InitialiseParameters();
    }
       

    private void DealDamage(int amount) { currentHealthPoints = Mathf.Clamp(currentHealthPoints - amount, 0, maxHealthPoints); }    
    
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
    private bool HealAllStatus(Skill skill)
    {
        if (skill.SkillID == 43) //Remedy skill, clear negative statuses
        {
            ApplyStatus(Status.BLIND, 0);
            ApplyStatus(Status.MUTE, 0);
            ApplyStatus(Status.WEAK, 0);
        }
        else
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + Mathf.Abs(skill.Damage), 0, maxHealthPoints);
        }       
        
        return true;
    }
      
    private void OnMouseEnter()
    {     
        bsStateMachine.CharacterStatsDisplay.ShowInfo(this);
    }

    private void OnMouseExit()
    {       
        bsStateMachine.CharacterStatsDisplay.ClearInfo();
    }    
}