using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameSession : MonoBehaviour
{
    [SerializeField] List<GameObject> allyPrefabs;    

    [SerializeField] List<BattleCourse> availableBattles; public List<BattleCourse> AvailableBattles { get => availableBattles; }

    List<CharacterLoadData> availableAllies; public List<CharacterLoadData> AvailableAllies { get => availableAllies; }  

    List<CharacterLoadData> availableUntrainedCharacters; public List<CharacterLoadData> AvailableUntrainedCharacters { get => availableUntrainedCharacters; }
    List<CharacterLoadData> availableTrainedCharacters; public List<CharacterLoadData> AvailableTrainedCharacters { get => availableTrainedCharacters; }

    //Hold the index of the chosen characters for a fight
    int playerIndex; public int PlayerIndex { set => playerIndex = value; } 
    int allyIndex; public int AllyIndex { set => allyIndex = value; }
    int battleIndex; public int BattleIndex { set => battleIndex = value; }

    public CharacterLoadData Player { get => availableUntrainedCharacters[playerIndex]; } //Available players in training (From a load file eventually)
    public CharacterLoadData Ally { get => availableTrainedCharacters[allyIndex]; } //Available trained allies (From a load file eventually)
    public BattleCourse BattleCourse { get => availableBattles[battleIndex]; }


    // Start is called before the first frame update
    void Awake()
    {
        SetUpSingleton();
    }

    private void Start()
    {        
        LoadDefaultCharacters();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            //Debug.Log("Awake (gamesession): Destroy Gamesession: " + GetInstanceID());
            DestroyImmediate(gameObject);
        }
        else
        {
            // Debug.Log("Awake (gamesession): Setting gamesession ID:" + GetInstanceID());
            DontDestroyOnLoad(gameObject);
        }
    }

    //Harcoded for default game characters
    private void LoadDefaultCharacters()
    {
        availableUntrainedCharacters = new List<CharacterLoadData>();
        availableTrainedCharacters = new List<CharacterLoadData>();

        CharacterLoadData data = new CharacterLoadData("Alex", allyPrefabs[0], new Player_AI()); //Warrior (Trained List)
        availableTrainedCharacters.Add(data);

        data = new CharacterLoadData("Anna", allyPrefabs[1], new Player_AI()); //Mage (Untrained List)
        availableUntrainedCharacters.Add(data);

        data = new CharacterLoadData("Lucy", allyPrefabs[2], new Player_AI()); //Chemist (Trained List)
        availableTrainedCharacters.Add(data);
    }
    
    public void SaveCharacterDataToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/characterSave.dat");
        GameData gData = new GameData();

        foreach (CharacterLoadData cld in AvailableTrainedCharacters)
        {
            CharacterSaveData cData = new CharacterSaveData();
            cData.Name = cld.CharacterName;
            cData.MenuName = cld.CharacterPrefab.GetComponent<Character>().MenuName;
            cData.LinkedAI = cld.LinkedAI;

            gData.TrainedCharacters.Add(cData);            
        }

        foreach (CharacterLoadData cld in AvailableUntrainedCharacters)
        {
            CharacterSaveData cData = new CharacterSaveData();
            cData.Name = cld.CharacterName;
            cData.MenuName = cld.CharacterPrefab.GetComponent<Character>().MenuName;
            cData.LinkedAI = cld.LinkedAI;

            gData.UntrainedCharacters.Add(cData);
        }

        bf.Serialize(file, gData);
        file.Close();
        FindObjectOfType<LevelManager>()?.ShowMessage("Game Saved");
    }

    public void LoadCharacterDataFromFile()
    {
        if (File.Exists(Application.dataPath + "/characterSave.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/characterSave.dat", FileMode.Open);            
            GameData gData = (GameData)bf.Deserialize(file);
            file.Close();

            AvailableTrainedCharacters.Clear();
            AvailableUntrainedCharacters.Clear();

            foreach(CharacterSaveData cData in gData.TrainedCharacters)
            {
                GameObject prefab;
                switch(cData.MenuName)
                {
                    case "Warrior":
                        prefab = allyPrefabs[0];
                        break;
                    case "Mage":
                        prefab = allyPrefabs[1];
                        break;
                    case "Chemist":
                    default:
                        prefab = allyPrefabs[2];
                        break;
                }
                CharacterLoadData cld = new CharacterLoadData(cData.Name, prefab, cData.LinkedAI);
                AvailableTrainedCharacters.Add(cld);
            }
            foreach (CharacterSaveData cData in gData.UntrainedCharacters)
            {
                GameObject prefab;
                switch (cData.MenuName)
                {
                    case "Warrior":
                        prefab = allyPrefabs[0];
                        break;
                    case "Mage":
                        prefab = allyPrefabs[1];
                        break;
                    case "Chemist":
                    default:
                        prefab = allyPrefabs[2];
                        break;
                }
                CharacterLoadData cld = new CharacterLoadData(cData.Name, prefab, cData.LinkedAI);
                AvailableUntrainedCharacters.Add(cld);
            }
            FindObjectOfType<LevelManager>()?.ShowMessage("Game Loaded.");
        }
        else
        {
            //Hardcoded first time load.
            FindObjectOfType<LevelManager>()?.ShowMessage("Save file not found.");
        }
    }
}

//Serializable classes to save and load Game Data
[Serializable]
public class CharacterSaveData
{
    string menuName; public string MenuName { get => menuName; set => menuName = value; }
    string name; public string Name { get => name; set => name = value; }
    CharacterAI linkedAI; public CharacterAI LinkedAI { get => linkedAI; set => linkedAI = value; }
}

[Serializable]
public class GameData
{
    List<CharacterSaveData> untrainedCharacters; public List<CharacterSaveData> UntrainedCharacters {  get =>untrainedCharacters; set => untrainedCharacters = value; }
    List<CharacterSaveData> trainedCharacters; public List<CharacterSaveData> TrainedCharacters { get => trainedCharacters; set => trainedCharacters = value; }

    public GameData()
    {
        untrainedCharacters = new List<CharacterSaveData>();
        trainedCharacters = new List<CharacterSaveData>();
    }    
}