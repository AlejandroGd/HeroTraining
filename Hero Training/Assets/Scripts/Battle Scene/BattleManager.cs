using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum FightState
{    
    PLAYER_TURN,
    ALLY_TURN,
    ENEMY_TURN,
    WON,
    LOST
}

public class BattleManager : MonoBehaviour
{
    public FightState currentBattleState;  //Current state
    GameSession gameSession;                //Game Session reference.
        
    GameObject playerInstance;
    GameObject allyInstance;
    GameObject enemyInstance;

    //Quick access references used frequently
    Character playerCharacter; public Character PlayerCharacter { get => playerCharacter; }
    Character allyCharacter;   
    Character enemyCharacter;  
    CharacterAnimator playerAnimations;
    CharacterAnimator allyAnimations;
    CharacterAnimator enemyAnimations;
    LevelManager levelManager;

    int currentEnemyIndex = 0;

    [SerializeField] TextMeshProUGUI helpText;
    SkillButtonsGUI skillsButtonsGUI;
    Dictionary<int, int> skillsUsedInBattle;

    [SerializeField] CharacterDataDisplay activeCharacterDataDisplay;
    [SerializeField] AttributeDisplay attributeDisplayPlayer;
    [SerializeField] AttributeDisplay attributeDisplayAlly;
    [SerializeField] AttributeDisplay attributeDisplayEnemy;

    [SerializeField] Transform playerPosTransform;
    [SerializeField] Transform allyPosTransform;
    [SerializeField] Transform enemyPosTransform;

    [SerializeField] ResultsPanel resultsPanel;
    CharacterStatsDisplay characterStatsDisplay; public CharacterStatsDisplay CharacterStatsDisplay { get => characterStatsDisplay; }
           
    // Start is called before the first frame update
    void Start()
    {
        skillsUsedInBattle = new Dictionary<int, int>();

        levelManager = FindObjectOfType<LevelManager>();
        gameSession = FindObjectOfType<GameSession>();        
        skillsButtonsGUI = FindObjectOfType<SkillButtonsGUI>();
        characterStatsDisplay = FindObjectOfType<CharacterStatsDisplay>();

        SetupFight();

        currentBattleState = FightState.PLAYER_TURN;
        PlayerTurn();
    }       

    //Updates player displays and action buttons to wait for player input.
    void PlayerTurn()
    {
        //IF player is dead THEN change to ally turn
        if (playerCharacter.IsDead())
        {
            currentBattleState = FightState.ALLY_TURN;
            StartCoroutine(AllyTurn());
        }
        else
        {
           //Discount any possible active statuses.
            playerCharacter.DiscountTurnForAllStatus();

            //Update GUI and wait for input
            attributeDisplayPlayer.UpdateValues();
            helpText.text = playerCharacter.CharacterName + " - Choose an Action";
            activeCharacterDataDisplay.UpdateDataDisplay(playerInstance);
            skillsButtonsGUI.ReloadSkillsInfo(playerCharacter);
        }
    }

    //Called from the buttons GUI, Starts Coroutine to execute player action.
    public void ApplyPlayerChoice(GameObject skillPrefab)
    {        
        skillsButtonsGUI.ClearSkillButtonGUI(); //Clear and deactivate GUI
        StartCoroutine(SelectPlayerAction(skillPrefab)); //Starts Coroutine to execute player action.
    }

    //Deals with the actions for the player's turn and starts coroutine for ally's turn (or the EndGame if applies).
    IEnumerator SelectPlayerAction(GameObject skillPrefab)
    {        
        Skill skill = skillPrefab.GetComponent<Skill>(); //Gets skill component reference as it will be used often.     
       
        helpText.text = playerCharacter.CharacterName + " - " + skill.MenuName; //Update GUI with skill name.
        yield return new WaitForSeconds(0.5f);                                  //Give some time to read action.
        float waitingTime = skill.AnimationPlayTime;                            //Check how long do we wait for the skill animation.
        playerAnimations.PlayAttack();                                          //Play attack animation.
        yield return new WaitForSeconds(0.5f);                                  //Give some time for animation to finish.

        //Buffs and heals apply to player and ally. Other attacks apply only to the enemy.
        if (skill.SkillClass == Skill.SkillType.BUFF || skill.SkillClass == Skill.SkillType.HEAL) 
        {
            //Execute action on itself and ally (if alive)
            ExecuteAction(playerCharacter, skill, skillPrefab, playerCharacter, playerPosTransform.position, playerAnimations, attributeDisplayPlayer);
            if (!allyCharacter.IsDead()) ExecuteAction(playerCharacter, skill, skillPrefab, allyCharacter, allyPosTransform.position, allyAnimations, attributeDisplayAlly);
            yield return new WaitForSeconds(waitingTime); //Wait until the animations finish.
        }
        else
        {
            //Execute action on enemy
            ExecuteAction(playerCharacter, skill, skillPrefab, enemyCharacter, enemyPosTransform.position, enemyAnimations, attributeDisplayEnemy);
            yield return new WaitForSeconds(waitingTime); //Wait until the animations finish.
        }

        
        //Add record of action chosen to player's AI
        FightAIRecord aiRecord = new FightAIRecord(playerCharacter, allyCharacter, enemyCharacter, skill);
        playerCharacter.CharacterAI.Learn(aiRecord);

        //Add skill used to the dictionary to show on result panel at the end of the fight.
        UpdateSkillsUsedCount(skill.SkillID);

        //Clear help text.
        helpText.text = "";       

        //Check for enemy death if the skill can lower hp
        if ((skill.SkillClass == Skill.SkillType.PHYSIC || skill.SkillClass == Skill.SkillType.MAGIC)
            && enemyCharacter.IsDead())
        {
            //Play death animation and wait for it to finish.
            enemyAnimations.PlayDead();
            yield return new WaitForSeconds(1);

            DealWithEnemyDeath();
        }
        else
        {
            //Change to ally turn
            currentBattleState = FightState.ALLY_TURN;
            StartCoroutine(AllyTurn());
        }
    }   

    //Deals with the actions for the ally's turn and starts coroutine for enemy's turn (or the EndGame if applies).
    IEnumerator AllyTurn()
    {
        //IF ally is dead THEN change to enemy turn
        if (allyCharacter.IsDead())
        {
            currentBattleState = FightState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            
            allyCharacter.DiscountTurnForAllStatus(); //Discount any possible active statuses.

            //Build a FightAIRecord (Describes to the character AI the state of the battle)
            FightAIRecord stateRecord = new FightAIRecord(allyCharacter, playerCharacter, enemyCharacter);

            //AI chooses a skill
            GameObject skillPrefab = allyCharacter.ChooseAction(stateRecord);
            Skill skill = skillPrefab.GetComponent<Skill>();
                       
            //Update GUI
            attributeDisplayAlly.UpdateValues();
            helpText.text = allyCharacter.CharacterName + " - " + skill.MenuName;
            activeCharacterDataDisplay.UpdateDataDisplay(allyInstance);

            
            yield return new WaitForSeconds(0.5f); //Give some time to read action chosen by AI.
           
            float waitingTime = skill.AnimationPlayTime; //Check how long do we wait for the skill animation.

            //Play attack animation and wait for it to finish.
            allyAnimations.PlayAttack();
            yield return new WaitForSeconds(0.5f);

            //Apply effects
            //Buffs and heals apply to player and ally
            if (skill.SkillClass == Skill.SkillType.BUFF || skill.SkillClass == Skill.SkillType.HEAL)
            {
                //Execute action on player (if alive) and itself.                
                if (!PlayerCharacter.IsDead()) ExecuteAction(allyCharacter, skill, skillPrefab, playerCharacter, playerPosTransform.position, playerAnimations, attributeDisplayPlayer);
                ExecuteAction(allyCharacter, skill, skillPrefab, allyCharacter, allyPosTransform.position, allyAnimations, attributeDisplayAlly);
                //Wait until the animations finish 
                yield return new WaitForSeconds(waitingTime);
            }
            else
            {               
                //Execute action on enemy
                ExecuteAction(allyCharacter, skill, skillPrefab, enemyCharacter, enemyPosTransform.position, enemyAnimations, attributeDisplayEnemy);

                //Wait until the animations finish 
                yield return new WaitForSeconds(waitingTime);
            }

            //Clear help text.
            helpText.text = "";

            //IF enemy dies THEN:
            if (enemyCharacter.IsDead())
            {
                //Play death animation.
                enemyAnimations.PlayDead();
                yield return new WaitForSeconds(1);

                DealWithEnemyDeath();
            }
            else //ELSE
            {
                //Change to ENEMY turn.
                currentBattleState = FightState.ENEMY_TURN;
                StartCoroutine(EnemyTurn());
            }
        }
    }

    //Deals with the actions for the enemy's turn and starts coroutine for player's turn (or the EndGame if applies).
    IEnumerator EnemyTurn()
    {        
        enemyCharacter.DiscountTurnForAllStatus(); // Discount any possible active statuses.

        //Build a FightAIRecord (Describes to the character AI the state of the battle)
        FightAIRecord stateRecord = null;

        //Enemies choose actions according to the fight record 1st character attributes
        if (playerCharacter.IsDead()) stateRecord = new FightAIRecord(allyCharacter, playerCharacter, enemyCharacter);
        else stateRecord = new FightAIRecord(playerCharacter, allyCharacter, enemyCharacter);
        
        //AI chooses an action.
        GameObject skillPrefab = enemyCharacter.ChooseAction(stateRecord);
        Skill skill = skillPrefab.GetComponent<Skill>();

        //Update GUI
        attributeDisplayEnemy.UpdateValues();        
        helpText.text = enemyCharacter.CharacterName + " - " + skill.MenuName;
        activeCharacterDataDisplay.UpdateDataDisplay(enemyInstance);
                
        yield return new WaitForSeconds(0.5f); //Give some time to read action
                
        float waitingTime = skill.AnimationPlayTime; //Check how long do we wait for the skill animation.

        //Play attack animation and wait for it to finish.
        enemyAnimations.PlayAttack();
        yield return new WaitForSeconds(0.5f);

        //Apply effects
        //Buffs and heals apply to enemy
        if (skill.SkillClass == Skill.SkillType.BUFF || skill.SkillClass == Skill.SkillType.HEAL)
        {            
            //Execute action on itself         
            ExecuteAction(enemyCharacter, skill, skillPrefab, enemyCharacter, enemyPosTransform.position, enemyAnimations, attributeDisplayEnemy);            
            yield return new WaitForSeconds(waitingTime); //Wait until the animations finish.
        }
        else
        {
            //Check if player/ally are alive in this turn. 
            bool playerWasDead = playerCharacter.IsDead();
            bool allyWasDead = allyCharacter.IsDead();

            //Execute action on player and ally if they are alive.                
            if (!playerWasDead) ExecuteAction(enemyCharacter, skill, skillPrefab, playerCharacter, playerPosTransform.position, playerAnimations, attributeDisplayPlayer);
            if (!allyWasDead) ExecuteAction(enemyCharacter, skill, skillPrefab, allyCharacter, allyPosTransform.position, allyAnimations, attributeDisplayAlly);
           
            yield return new WaitForSeconds(waitingTime);  //Wait until the animations finish.

            //If player or ally died in this turn, play their death animation.
            if (!playerWasDead && playerCharacter.IsDead()) playerAnimations.PlayDead();
            if (!allyWasDead && allyCharacter.IsDead()) allyAnimations.PlayDead();
        }

        //Clear help text.
        helpText.text = "";

        //IF player AND ally die THEN game is over:  
        if (playerCharacter.IsDead() && allyCharacter.IsDead())
        {
            //Change to LOST State
            currentBattleState = FightState.LOST;
            StartCoroutine(EndFight());
        }
        else //ELSE 
        {
            //Change to Player Turn. 
            currentBattleState = FightState.PLAYER_TURN;
            PlayerTurn();
        }
    }
    
    //Finish everything and go back to Main Menu.
    IEnumerator EndFight()
    {
        //Show result panel only if battle is won.
        if (currentBattleState == FightState.WON)
        {
            helpText.text = "BATTLE WON";                                               //Update Help Text.
            resultsPanel.UpdateResultsPanelInfo(playerCharacter, skillsUsedInBattle);   //Show Results Panel.
        }
        else
        {
            //Update Help Text and goes back to main menu.
            helpText.text = "BATTLE LOST";
            yield return new WaitForSeconds(2);
            GoToMainMenu();
        };
    }

    //Executes an action on a character. (Instantiate skill prefab, apply effect to character, play animations and update hp and status displays)
    void ExecuteAction(Character attacker, Skill skill, GameObject skillPrefab, Character receiver, Vector3 receiverPosition, CharacterAnimator receiverAnimator, AttributeDisplay receiverAttributeDisplay)
    {
        Instantiate(skillPrefab, receiverPosition, Quaternion.identity); //Instantiate skill
        //Play positive or negative effect animation on receiver depending on the type of skill used.
        if (skill.SkillClass == Skill.SkillType.BUFF || skill.SkillClass == Skill.SkillType.HEAL) receiverAnimator.PlayPositivEffect();
        else receiverAnimator.PlayNegativeEffect();
        receiver.ApplySkill(attacker, skill);       //Apply effects.                    
        receiverAttributeDisplay.UpdateValues();    //Update Displays (Hp and Status)  
    }

    //Check if there is another enemy to setup or starts EndFight Coroutine if all enemies have been defeated.
    void DealWithEnemyDeath()
    {
        //IF no more enemies THEN Change to WON State
        if (currentEnemyIndex >= gameSession.BattleCourse.EnemiesPrefabList.Count - 1)
        {
            currentBattleState = FightState.WON;
            StartCoroutine(EndFight());
        }
        else //ELSE (there is more enemies)
        {
            //Setup new enemy               
            SetupFight_NextMonster();

            //There is a new enemy so the player acts first. Change to Player Turn. 
            currentBattleState = FightState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    //Instantiate characters according to gamesession data.
    public void SetupFight()
    {
        //Instantiate fighters 
        //For each fighter:
        //1.Pick the right prefab
        //2.Initialise or Attach AI supplied
        //3.Attach it to its attribute display (Life and status)

        //Player 
        playerInstance = Instantiate(gameSession.Player.CharacterPrefab, playerPosTransform.position, Quaternion.identity);
        playerCharacter = playerInstance.GetComponent<Character>();
        playerAnimations = playerInstance.GetComponent<CharacterAnimator>();

        playerCharacter.CharacterName = gameSession.Player.CharacterName;
        playerCharacter.CharacterAI = gameSession.Player.LinkedAI;        
        attributeDisplayPlayer.SetCharacterPrefab(playerInstance);

        //Ally
        allyInstance = Instantiate(gameSession.Ally.CharacterPrefab, allyPosTransform.position, Quaternion.identity);
        allyCharacter = allyInstance.GetComponent<Character>();
        allyAnimations = allyInstance.GetComponent<CharacterAnimator>();

        allyCharacter.CharacterName = gameSession.Ally.CharacterName;
        gameSession.Ally.LinkedAI.BuildAIProfile();
        allyCharacter.CharacterAI = gameSession.Ally.LinkedAI;       
        attributeDisplayAlly.SetCharacterPrefab(allyInstance);

        //First enemy of the Selected Battle Course list.
        enemyInstance = Instantiate(gameSession.BattleCourse.EnemiesPrefabList[currentEnemyIndex], enemyPosTransform.position, Quaternion.identity);
        enemyCharacter = enemyInstance.GetComponent<Character>();
        enemyAnimations = enemyInstance.GetComponent<CharacterAnimator>();

        enemyCharacter.CharacterName = enemyCharacter.MenuName;
        enemyCharacter.CharacterAI = ChooseEnemyAI(enemyCharacter.SkillPrefabList);        
        attributeDisplayEnemy.SetCharacterPrefab(enemyInstance);
               
        //Hide results panel.
        resultsPanel.gameObject.SetActive(false);       
    }

    //Destroy current monster GameObject and Instantiate next one.
    public void SetupFight_NextMonster()
    {
        //Destroy GameObject Instance for current monster.
        Destroy(enemyInstance);

        //Set next monster index
        currentEnemyIndex++;

        //Set next monster prefab
        enemyInstance = Instantiate(gameSession.BattleCourse.EnemiesPrefabList[currentEnemyIndex], enemyPosTransform.position, Quaternion.identity);
        enemyCharacter = enemyInstance.GetComponent<Character>();
        enemyAnimations = enemyInstance.GetComponent<CharacterAnimator>();

        enemyCharacter.CharacterName = enemyCharacter.MenuName;
        enemyCharacter.CharacterAI = ChooseEnemyAI(enemyCharacter.SkillPrefabList);
        attributeDisplayEnemy.SetCharacterPrefab(enemyInstance);
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

    //Keeps count of the used skills during the battle for the results panel at the end.
    public void UpdateSkillsUsedCount(int skillId)
    {
        if (skillsUsedInBattle.ContainsKey(skillId)) skillsUsedInBattle[skillId]++;
        else skillsUsedInBattle.Add(skillId, 1);
    }

    public void GoToMainMenu()
    {
        levelManager.LoadStartMenu();
    }
}
