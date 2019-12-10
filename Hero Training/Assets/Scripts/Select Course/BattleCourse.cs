using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle Course Config")]
public class BattleCourse : ScriptableObject
{
    private enum TrainingObjectives { PHYSIC, MAGIC, ELEMENTAL, HEALING, AILMENT_USE, AILMENT_HEALING, PROTECTION_SKILLS, OVERALL }
    [SerializeField] int courseId;
    [SerializeField] string courseName;
    [SerializeField] Sprite battleSelectionImage; 
    [SerializeField] [TextArea] string courseDescription; 
    [SerializeField] List<TrainingObjectives> mainTrainingObjectives;
    [SerializeField] List<GameObject> enemiesPrefabList;

    public string CourseName { get => courseName; }
    public Sprite BattleSelectionImage { get => battleSelectionImage; }
    public string CourseDescription { get => courseDescription; }
    public List<GameObject> EnemiesPrefabList { get => enemiesPrefabList; }
}