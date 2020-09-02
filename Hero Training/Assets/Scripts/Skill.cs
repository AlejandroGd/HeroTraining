using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum SkillType { PHYSIC, MAGIC, BUFF, DEBUFF, HEAL}
    public enum EElement { FIRE, ICE, NEUTRAL }   

    [Header("GUI")]
    [SerializeField] string menuName = "";
    [SerializeField] int id = 0;
    [SerializeField] string helpText = "";

    [Header("Attack Parameters")]
    [SerializeField] SkillType skillClass;
    [SerializeField] EElement element;    
    [SerializeField] int damage = 0;

    [Header("Buff/Debuff Parameters")]
    [SerializeField] Character.Status status = Character.Status.NONE;
    [SerializeField] int turns = 0;

    [Header("Animation Parameters")]
    [SerializeField] float animationTime = 1.2f;

    //Animation flags
    bool animationFinished = false; public bool AnimationFinished { get => animationFinished; }

    //Accessors
    public string MenuName { get => menuName; }
    public int SkillID { get => id; }
    public string HelpText { get => helpText; }
    public SkillType SkillClass { get => skillClass; }
    public EElement Element { get => element; }
    public Character.Status Status { get => status; }    
    public int Damage { get => damage; set => damage = value; }
    public float AnimationPlayTime { get => animationTime; }

    
    public int Turns { get => turns; }
    
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}