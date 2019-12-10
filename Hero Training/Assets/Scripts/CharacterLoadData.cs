using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//Hold  information about available characters to choose in the game
[Serializable]
public class CharacterLoadData
{
    GameObject characterPrefab; public GameObject CharacterPrefab { get => characterPrefab; }    
    string characterName; public string CharacterName { get => characterName; }
    CharacterAI linkedAI; public CharacterAI LinkedAI { get => linkedAI; set => linkedAI = value; }

    public CharacterLoadData(string name, GameObject prefab, CharacterAI ai)
    {
        characterName = name;
        characterPrefab = prefab;
        linkedAI = ai;
    }
}