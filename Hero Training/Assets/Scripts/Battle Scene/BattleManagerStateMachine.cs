using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Possible states
public enum BattleState
{
    SETUP_INITIAL,
    SETUP_NEXT_MONSTER,
    PROCESS,
    PROCESS_REWARD,
    PROCESS_PUNISHMENT,
    ACTION_PERFORM,
    CHECK_END_FIGHT_CONDITION,
    CHOOSE_ACTION_AI,
    CHOOSE_ACTION_PLAYER,
    WAITING_FOR_PLAYER_INPUT,
    IDLE,
    END_FIGHT
}

/*
 * Main State Machine that controls the battle flow.
 */
public class BattleManagerStateMachine : MonoBehaviour
{    
    public BattleState currentBattleState;  //Current state
    GameSession gameSession;                //Game Session reference.

    //Battle Related (Result, current enemy, etc.)
    enum BattleResult { ONGOING, WIN, LOST };
    BattleResult fightOutcome = BattleResult.ONGOING;
    int currentEnemyIndex = 0;

    //Turn queue and active character related variables.    
    int turnQueueSize = 8;
    private struct TurnIndex
    {
        TurnIndex(string t, int i) { type = t; index = i; }
        public string type;    //a-allie, e-enemy
        public int index;      //index in list
    }    
    List<TurnIndex> turnQueue; 
    TurnIndex currentTurn;

    //Turn should be between 1 and 8 (turn - 1 index in the turn queue). 
    //Active character does not count.
    public GameObject GetCharacterAtTurn(int turn)
    {
        TurnIndex turnIndex = turnQueue[turn - 1];
        if (turnIndex.type == "a") return alliesList[turnIndex.index];
        else return enemiesList[turnIndex.index];
    }

    //Only the active turn is saved to know whose turn it is.
    //Accessor created to return active character GameObject when needed.
    public GameObject ActiveCharacter { get => GetActiveCharacter(); }
    private GameObject GetActiveCharacter()
    {
        if (currentTurn.type == "a") return alliesList[currentTurn.index];
        else return enemiesList[currentTurn.index];
    }
           
    //Pending actions queue
    List<BattleAction> actionQueue; public List<BattleAction> ActionQueue { get => actionQueue; } 

    // Allies/Enemies in fight
    List<GameObject> alliesList; public List<GameObject> AlliesList { get => alliesList; }
    List<GameObject> enemiesList; public List<GameObject> EnemiesList { get => enemiesList; }

    public Character GetCharacterComponent_Player() { return alliesList[0].GetComponent<Character>(); }
    public Character GetCharacterComponent_Ally() { return alliesList[1].GetComponent<Character>(); }
    public Character GetCharacterComponent_Enemy() { return enemiesList[0].GetComponent<Character>(); }

    //Other References for quick access
    TextMeshProUGUI helpText;
    SkillButtonsGUI skillsButtonsGUI;
    CharacterStatsDisplay characterStatsDisplay; public CharacterStatsDisplay CharacterStatsDisplay { get => characterStatsDisplay; }

    //AI data for results panel at the end.
    Dictionary<int, int> skillsUsedInBattle;

    //Punishment/Reward variables
    FightAIRecord lastAllyRecord = null;    //Last ally action record.
    Skill lastAllySkill = null;             //Last ally action.

    bool processReward = false;     public bool ShouldProcessReward { set => processReward = value; }
    bool processPunishment = false; public bool ShouldProcessPunishment { set => processPunishment = value; }

    FightAIRecord rewardRecord = null;
    FightAIRecord punishmentRecord = null;
    Skill rewardSkill = null;
    Skill punishmentSkill = null;

    //Serialized to assign through inspector
    [SerializeField] CharacterDataDisplay activeCharacterDataDisplay;    
    [SerializeField] TurnDisplay turnDisplay;
    [SerializeField] AttributeDisplay attributeDisplayPlayer;
    [SerializeField] AttributeDisplay attributeDisplayAlly;
    [SerializeField] AttributeDisplay attributeDisplayEnemy;

    [SerializeField] GameObject playerPosition;
    [SerializeField] GameObject allyPosition;
    [SerializeField] GameObject enemyPosition;

    [SerializeField] ResultsPanel resultsPanel;
    [SerializeField] PunishmentPanel punishmentPanel;

    [SerializeField] GameObject rewardButton;
    [SerializeField] GameObject punishmentButton;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();

        currentBattleState = BattleState.SETUP_INITIAL;
               
        actionQueue = new List<BattleAction>();
        alliesList = new List<GameObject>();
        enemiesList = new List<GameObject>();

        skillsUsedInBattle = new Dictionary<int, int>();

        helpText = GameObject.Find("Action Text").GetComponent<TextMeshProUGUI>();
        skillsButtonsGUI = FindObjectOfType<SkillButtonsGUI>();
        characterStatsDisplay = FindObjectOfType<CharacterStatsDisplay>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentBattleState)
        {
            case BattleState.SETUP_INITIAL:
                SetupFight_Initial();
                break;
            case BattleState.SETUP_NEXT_MONSTER:
                SetupFight_NextMonster();
                break;
            case BattleState.PROCESS_REWARD:
                ProcessReward();
                break;
            case BattleState.PROCESS_PUNISHMENT:
                ProcessPunishment();
                break;
            case BattleState.PROCESS:
                Process();
                break;
            case BattleState.ACTION_PERFORM:
                //coroutines run asynchronously, we put the state machine in wait until this method finishes
                currentBattleState = BattleState.IDLE;
                StartCoroutine(PerformActions());
                break;
            case BattleState.CHOOSE_ACTION_AI:
                ChooseActionAI();
                break;
            case BattleState.CHOOSE_ACTION_PLAYER:
                ChooseActionPlayer();
                break;
            case BattleState.CHECK_END_FIGHT_CONDITION:
                CheckEndFightConditions();
                break;
            case BattleState.WAITING_FOR_PLAYER_INPUT:
            case BattleState.IDLE:
                //Both states wait, but certain actions only can be performed in one or another.
                break;
            case BattleState.END_FIGHT:
                StartCoroutine(EndFight());
                currentBattleState = BattleState.IDLE;
                break;
            default:
                Debug.Log("Wrong battle state");
                break;
        }
    }

    //Keeps count of the used skills during the battle for the results panel at the end.
    public void UpdateSkillsUsedCount(int skillId)
    {
        if (skillsUsedInBattle.ContainsKey(skillId)) skillsUsedInBattle[skillId]++;
        else skillsUsedInBattle.Add(skillId, 1);
    }

    //Fill list of Allies and enemy according to gamesession choices.
    //Initialise Turn Queue
    public void SetupFight_Initial()
    {
        //Set fighters (allies and enemies lists)
        //For each character in gamesession:
            //1.Pick the right prefab
            //2.Specify if it is player controlled or not.
            //3.Initialise or Attach AI supplied
            //4.Add to the right character list
            //5.Attach it to its attribute display (Life and status)

        //Player 
        GameObject playerPrefab = Instantiate(gameSession.Player.CharacterPrefab, playerPosition.transform.position, Quaternion.identity);
        playerPrefab.GetComponent<Character>().PlayerControlled = true;
        playerPrefab.GetComponent<Character>().IsAlly = false;
        playerPrefab.GetComponent<Character>().CharacterAI = gameSession.Player.LinkedAI;     

        alliesList.Add(playerPrefab);
        attributeDisplayPlayer.SetCharacterPrefab(playerPrefab);

        //Ally
        GameObject allyPrefab = Instantiate(gameSession.Ally.CharacterPrefab, allyPosition.transform.position, Quaternion.identity);
        allyPrefab.GetComponent<Character>().PlayerControlled = false;
        allyPrefab.GetComponent<Character>().IsAlly = true;
        gameSession.Ally.LinkedAI.BuildAIProfile(); 
        allyPrefab.GetComponent<Character>().CharacterAI = gameSession.Ally.LinkedAI;
        alliesList.Add(allyPrefab);
        attributeDisplayAlly.SetCharacterPrefab(allyPrefab);
        
        //First enemy of the Selected Battle Course list.
        GameObject enemyPrefab = Instantiate(gameSession.BattleCourse.EnemiesPrefabList[currentEnemyIndex], enemyPosition.transform.position, Quaternion.identity);
        enemyPrefab.GetComponent<Character>().PlayerControlled = false;
        enemyPrefab.GetComponent<Character>().IsAlly = false;
        enemyPrefab.GetComponent<Character>().CharacterAI = ChooseEnemyAI(enemyPrefab.GetComponent<Character>().SkillPrefabList);
        enemiesList.Add(enemyPrefab);
        attributeDisplayEnemy.SetCharacterPrefab(enemyPrefab);
                     
        //Build initial turn queue.
        InitialiseTurnQueue();

        //Hide results and punishment panels.
        resultsPanel.gameObject.SetActive(false);
        punishmentPanel.gameObject.SetActive(false);

        currentBattleState = BattleState.PROCESS;
    }

    //Randomly returns an Enemy profile.
    private EnemyAI ChooseEnemyAI(List<GameObject> skillList)
    {
        float choice = Random.Range(0, 4);
        if (choice < 1) return new EnemyAI_Aggresive(skillList);
        if (choice < 2) return new EnemyAI_Cautious(skillList);
        if (choice < 3) return new EnemyAI_StatusDriven(skillList);

        return new EnemyAI_Normal(skillList);
    }    

    //Build the turn queue. Order is: player controlled, allies and then enemies. Then repeat.
    private void InitialiseTurnQueue()
    {
        turnQueue = new List<TurnIndex>();

        currentTurn.type = "a";
        currentTurn.index = 0;
        turnQueue.Add(currentTurn);
        for (int x=1;x< turnQueueSize; x++) { AddNextCharacterTurnToQueue(); }
    }

    //Add another character turn to the turn queue
    private void AddNextCharacterTurnToQueue()
    {
        if (turnQueue.Count > 0)
        {
            TurnIndex t = turnQueue[turnQueue.Count - 1];
            if (t.type =="a")
            {
                if (t.index < alliesList.Count - 1) t.index++;
                else
                {
                    t.type = "e"; t.index = 0;
                }
            }
            else
            {
                if (t.index < enemiesList.Count - 1) t.index++;
                else
                {
                    t.type = "a"; t.index = 0;
                }
            }
            turnQueue.Add(t);
        }
    }

    //Executed in the PROCESS_REWARD State
    private void ProcessReward()
    {
        //Copy the record
        rewardRecord = new FightAIRecord(lastAllyRecord);
        rewardSkill = lastAllySkill;
        currentBattleState = BattleState.PROCESS;
    }

    //Executed in the PROCESS_PUNISHMENT state
    private void ProcessPunishment()
    {
        punishmentPanel.UpdatePunishmentSkillPanelInfo(GetCharacterComponent_Ally(), lastAllySkill);
        currentBattleState = BattleState.IDLE;
    }

    //Creates and sets a record as a reward record for the ally.
    public void AddPunishmentRecord(Skill skill)
    {
        //Copy the record but change the skill.
        punishmentRecord = new FightAIRecord(lastAllyRecord);
        punishmentRecord.SkillUsed = skill.SkillID;
        
        punishmentSkill = skill;
        currentBattleState = BattleState.PROCESS;
    }

    //PROCESS state. Check for actions or gets next player turn if no actions are available.
    public void Process()
    {
        //If there are still BattleActions pending in the action queue change to a state to perform them.
        //If not, check the turn queue to see who goes next.
        if (rewardRecord == null && processReward ) currentBattleState = BattleState.PROCESS_REWARD;
        else if (punishmentRecord == null && processPunishment) currentBattleState = BattleState.PROCESS_PUNISHMENT;
        else if (actionQueue.Count > 0)
        {
            currentBattleState = BattleState.ACTION_PERFORM;
        }
        else
        {
            SetNextActivePlayer();
        }
    }

    //Fetches the next character in the turn queue and sets it as the active one.
    //transitions to next state depends on the character being Player or AI controlled.
    private void SetNextActivePlayer()
    {
        currentTurn = turnQueue[0];     //Set next character in turn queue as active.
        turnQueue.RemoveAt(0);          //Remove it from the queue.
        AddNextCharacterTurnToQueue();  //Add another turn to queue.

        Character activeCharacter = GetActiveCharacter().GetComponent<Character>();

        //Put name of active character in the help text and update their data in the GUI
        if (activeCharacter.PlayerControlled) helpText.text = activeCharacter.MenuName;
        activeCharacterDataDisplay.UpdateDataDisplay(GetActiveCharacter());

        //Update the turn display in the GUI.
        turnDisplay.UpdateTurnDisplay();

        //Decrement one turn all the active status effects for that character.
        activeCharacter.DiscountTurnForStatus();

        //Change to state to choose an action depending on being player or AI controlled
        if (activeCharacter.PlayerControlled) currentBattleState = BattleState.CHOOSE_ACTION_PLAYER;
        else currentBattleState = BattleState.CHOOSE_ACTION_AI;
    }

    //Choose action according to AI
    public void ChooseActionAI()
    {
        Character activeChar = ActiveCharacter.GetComponent<Character>();

        if (!activeChar.IsDead()) activeChar.ChooseAction();
        else currentBattleState = BattleState.PROCESS; //If character is dead just skip it
    }

    //Set the GUI for the Player to choose an action and WAIT (WAITING_FOR_PLAYER_INPUT state) for the player input
    public void ChooseActionPlayer()
    {
        Character activeChar = ActiveCharacter.GetComponent<Character>();
        //Only alive characters can perform actions.
        if (!activeChar.IsDead())
        {  
            skillsButtonsGUI.ReloadSkillsInfo(GetActiveCharacter().GetComponent<Character>());
            currentBattleState = BattleState.WAITING_FOR_PLAYER_INPUT;
        }
        else currentBattleState = BattleState.PROCESS;//If character is dead just skip it
    }
      
    //Gets all the actions in the action queue and play the animations. (all actions at a determined time come from same character)
    public IEnumerator PerformActions()
    {        
        //All actions available in the queue at some point are the same, but with different 
        //targets (if more than one) so we just take the common things from the first.
        
        //Update help text with skill name
        helpText.text = actionQueue[0].Attacker.GetComponent<Character>().MenuName + " - " + actionQueue[0].Action.GetComponent<Skill>().MenuName;

        //Wait a bit before animation (Giving time to the player to read the action)
        yield return new WaitForSeconds(1f);

        //Play the attack animation for the attacker. (Only one attacker, take it from first action) 
        actionQueue[0].Attacker.GetComponent<CharacterAnimator>().PlayAttack();

        //If the attacker is the ally, save last action.
        //Punishment / reward buttons should activate the first time an ally chooses a skill.
        if (actionQueue[0].Attacker.GetComponent<Character>().IsAlly)
        {
            if (lastAllySkill == null)
            {
                rewardButton.SetActive(true);
                punishmentButton.SetActive(true);
            }
            lastAllySkill = actionQueue[0].Action.GetComponent<Skill>();
            lastAllyRecord = new FightAIRecord(GetCharacterComponent_Ally(),
                                               GetCharacterComponent_Player(),
                                               GetCharacterComponent_Enemy(),
                                               lastAllySkill);
        }

        //For each action in the actionQueue instantiate the skill object. (Which plays its animation)
        //And play the hurt animation on target at the same time
        List<GameObject> skillsInstantiated = new List<GameObject>();
        foreach (BattleAction bAction in actionQueue)
        {
            //Instantiate skill
            GameObject o = Instantiate(bAction.Action, bAction.Target.transform.position, Quaternion.identity);
            skillsInstantiated.Add(o);

            //At the same time the "Hurt" or "Buff" animation will play depending on the skill.
            //if (bAction.Target.tag == "Ally" || bAction.Target.GetComponent<Character>().MenuName == "Fire Mage Elf")
            {
                Skill.SkillType skillClass = bAction.Action.GetComponent<Skill>().SkillClass;

                //Heal or buff that succeeds plays the Buff animation, the rest play the Hurt animation.
                if (skillClass == Skill.SkillType.HEAL || skillClass == Skill.SkillType.BUFF)
                {
                    bAction.Target.GetComponent<CharacterAnimator>().PlayBuff();
                }
                else
                {
                    if (skillClass != Skill.SkillType.DEBUFF || bAction.SkillSucceded)
                    { bAction.Target.GetComponent<CharacterAnimator>().PlayHurt(); }
                }                
            }            
        }
        
        //Wait until the animations finish 
        yield return new WaitForDestroy(skillsInstantiated);
               
        
        //For each action in the actionQueue
        //apply action effects (Damage, healing, apply statuses, etc...)
        foreach (BattleAction bAction in actionQueue)
        {
            Character target = bAction.Target.GetComponent<Character>();
            Character attacker = bAction.Attacker.GetComponent<Character>();
            Skill skill = bAction.Action.GetComponent<Skill>();
            bAction.SkillSucceded = target.ApplySkill(attacker, skill);

            //Check for dead characters and play dead animation if needed.           
            if (target.CurrentHealthPoints <= 0)
            {
                bAction.Target.GetComponent<CharacterAnimator>().PlayDead();                    
            }            
        }

        //Update Hp and Sp bars
        attributeDisplayPlayer.UpdateValues();
        attributeDisplayAlly.UpdateValues();
        attributeDisplayEnemy.UpdateValues();        

        //Clear help text and action queue.
        helpText.text = "";
        actionQueue.Clear();
        
        //Wait a bit before next turn.
        yield return new WaitForSeconds(1.5f);

        //Check battle conditions after the skill effects.
        currentBattleState = BattleState.CHECK_END_FIGHT_CONDITION;                      
    }

    //Executed in the CHECK_END_FIGHT_CONDITION state
    public void CheckEndFightConditions()
    {
        //If any of the allies is alive, fight is not lost yet.
        bool allAlliesDead = true;
        foreach (GameObject charObj in alliesList)
        {           
            if (!charObj.GetComponent<Character>().IsDead()) allAlliesDead = false;
        }
        if (allAlliesDead) fightOutcome = BattleResult.LOST;

        //If any of the enemies is alive, fight is not won yet.
        bool allenemiesDead = true;
        foreach (GameObject charObj in enemiesList)
        {            
            if (!charObj.GetComponent<Character>().IsDead()) allenemiesDead = false;
        }
        
        if (allenemiesDead)
        {
            //Enemies are dead. If there is more in the battle course, bring the next one. Else, battle is won.
            if (currentEnemyIndex < gameSession.BattleCourse.EnemiesPrefabList.Count - 1)
            {                
                SetupFight_NextMonster();
            }
            else fightOutcome = BattleResult.WIN;
        }
        
        if (fightOutcome != BattleResult.ONGOING) currentBattleState = BattleState.END_FIGHT;
        else currentBattleState = BattleState.PROCESS;        
    }
          

    //Kill animation for monster and set next one.
    public void SetupFight_NextMonster()
    {   
        //Destroy prefab for current monster.
        Destroy(enemiesList[0]);
        enemiesList.Clear();

        //Set next monster index
        currentEnemyIndex++;

        //Set next monster prefab
        GameObject enemyPrefab = Instantiate(gameSession.BattleCourse.EnemiesPrefabList[currentEnemyIndex], enemyPosition.transform.position, Quaternion.identity);
        enemyPrefab.GetComponent<Character>().PlayerControlled = false;
        enemyPrefab.GetComponent<Character>().CharacterAI = ChooseEnemyAI(enemyPrefab.GetComponent<Character>().SkillPrefabList);
        enemiesList.Add(enemyPrefab);
        attributeDisplayEnemy.SetCharacterPrefab(enemyPrefab);

        //Update Queue.       
        InitialiseTurnQueue();

        //Back to process state.
        currentBattleState = BattleState.PROCESS;
    }  


    //Finish everything and go back to menu.
    public IEnumerator EndFight()
    {      
        //Show result panel only if battle is won.
        if (fightOutcome == BattleResult.WIN)
        {
            helpText.text = "BATTLE WON";
            
            //Update Player AI with this fight choices (Player controlled character is always on availableAllies[0])
            gameSession.UpdatePlayerAI(alliesList[0].GetComponent<Character>().CharacterAI);

            //If punishments / rewards have been assigned, update the ally AI.
            if (rewardRecord != null)       alliesList[1].GetComponent<Character>().CharacterAI.Learn(rewardRecord);
            if (punishmentRecord != null)   alliesList[1].GetComponent<Character>().CharacterAI.Learn(punishmentRecord);

            if (rewardRecord != null || punishmentRecord != null)
                    gameSession.UpdateAllyAI(alliesList[1].GetComponent<Character>().CharacterAI);
                       
            //Show Result Panel       
            resultsPanel.UpdateResultsPanelInfo(GetCharacterComponent_Player(), skillsUsedInBattle,
                                                GetCharacterComponent_Ally(), rewardSkill, punishmentSkill);
        }
        else if (fightOutcome == BattleResult.LOST)
        {
            helpText.text = "BATTLE LOST";
            yield return new WaitForSeconds(2);
            GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadStartMenu();
        };
    }
}


//Creating our own Yield instruction to wait for animations to finish before keep going.
public class WaitForDestroy : CustomYieldInstruction
{
    private List<GameObject> objects;

    //Override the condition to keep waiting
    public override bool keepWaiting
    {
        get { return ObjectsNotDestroyed(); }
    }

    //If all objects are not destroyed returns true.
    private bool ObjectsNotDestroyed()
    {
        foreach (GameObject o in objects)
        {
            if (o != null) return true;
        }
        return false;
    }

    //Passing the list of active animations to check for destroy
    public WaitForDestroy(List<GameObject> objects)
    {
        this.objects = objects;
    }
}