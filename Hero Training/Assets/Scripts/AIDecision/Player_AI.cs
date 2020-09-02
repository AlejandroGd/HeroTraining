using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Record actions and build a ID3 decision tree to use as AI for action choosing on trained characters.
//The functions to build the decision tree (MakeTree) is based on the pseudo-code in Chapter 7.6 
//of the book "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009).
//The tree nodes code is based on Chapter 5.2 of the same book.
[Serializable]
public class Player_AI : CharacterAI
{   
    List<FightAIRecord> fightHistoryRecords;    //Hold training records
    TreeNode decisionTree;          //Root node for Decision tree
    bool needsToRebuild;            //Tree gets rebuilt when new instances are added to the records.

    //Constructor
    public Player_AI()
    {
        needsToRebuild = true;
        decisionTree = null;
        fightHistoryRecords = new List<FightAIRecord>();
    }

    //Constructor with predefined AI records
    public Player_AI(List<FightAIRecord> aiRecords)
    {
        needsToRebuild = true;
        decisionTree = null;
        fightHistoryRecords = new List<FightAIRecord>(aiRecords);
    }

    public override int TotalSkillRecords() { return fightHistoryRecords.Count; }

    public override GameObject ChooseAction(List<GameObject> skillList, FightAIRecord record)
    {
        //If there is no knowledge, just choose the first one, which should be a basic attack.
        if (fightHistoryRecords.Count == 0) return skillList[0];
    
        int skillID = decisionTree.ChooseSkill(record);
        foreach(GameObject skillObject in skillList)
        {
            if (skillObject.GetComponent<Skill>().SkillID == skillID) return skillObject;
        }

        //In case a skill is removed on inspector but AI profile still have records on it.
        throw new System.Exception("Invalid skill ID " + skillID + " chosen by Player_AI");
    }

    public override void Learn(FightAIRecord data)
    {
        fightHistoryRecords.Add(data);
        needsToRebuild = true;
    }
    
    //Build the decision tree for the available records.
    public override void BuildAIProfile()
    {
        //Make the tree in the decisionTree

        if (needsToRebuild)
        {
            //PrintToConsole(); //Print to console for test purposes.
            if (fightHistoryRecords.Count > 0)
            {
                decisionTree = MakeTree(fightHistoryRecords);
            }
            needsToRebuild = false;

        }        
    }    

    //Count number of records with that skill ID
    public override int CountSkillRecords(int skillId)
    {
        int count = 0;
        foreach(FightAIRecord rec in fightHistoryRecords)
        {
            if (rec.GetAttribValue_Int(FightAIRecordAttributes.SKILL_USED) == skillId) count++;
        }
        return count;
    }


    //Recursive function to build the tree.
    //Adapted from "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009)
    private TreeNode MakeTree(List<FightAIRecord> records)//, DecisionNode node)
    {
        //Calculate this node's entropy
        float initialEntropy = Entropy(records);

        //If entropy = 0 no more divisions are needed. This is an action node.
        if (initialEntropy == 0)
        {
            //Entropy = 0 means all action are the same in all records, we just pick the skill ID from the first one.
            return new ActionTreeNode(records[0].GetAttribValue_Int(FightAIRecordAttributes.SKILL_USED));
        }

        //Number of records for this node
        int totalRecordCount = records.Count;

        //Info about the best split so far.
        float bestInformationGain = 0;

        FightAIRecordAttributes bestSplitAttribute = FightAIRecordAttributes.OWN_HEALTH;
        float bestThreshold = 0;
        List<FightAIRecord> bestSetTrue = new List<FightAIRecord>();
        List<FightAIRecord> BestSetFalse = new List<FightAIRecord>();
        
        //For each attribute (There are 45 attributres in total, getting advantage of the enum to int casting for FightAIRecordAttributes)
        for (int ai = 1; ai <= 45; ai++)
        {
            //Split
            FightAIRecordAttributes currentAttribute = (FightAIRecordAttributes)ai;
            List<FightAIRecord> setTrue;
            List<FightAIRecord> setFalse;
            float thresholdValue = 0;
            if (FightAIRecord.IsAttributeBoolean(currentAttribute))
            {
                SplitByBooleanAttribute(records, out setTrue, out setFalse, currentAttribute);
            }
            else
            {
                thresholdValue = SplitByContinuousAttribute(records, out setTrue, out setFalse, currentAttribute);
            }

            //Calculate entropy and information gain
            float overallEntropy = EntropyOfSets(setTrue, setFalse, totalRecordCount);
            float informationGain = initialEntropy - overallEntropy;

            //Check if beats the best so far
            if (informationGain > bestInformationGain )
            {
                bestInformationGain = informationGain;
                bestSplitAttribute = currentAttribute;
                bestSetTrue = new List<FightAIRecord>(setTrue);
                BestSetFalse = new List<FightAIRecord>(setFalse);
                bestThreshold = thresholdValue;
            }            
        }

        DecisionTreeNode thisNode;
        
        //Once the best split attribute is found, set the Decision's node test.
        if (FightAIRecord.IsAttributeBoolean(bestSplitAttribute))
        {
            thisNode = new EqualTo_DecisionTreeNode(bestSplitAttribute, bestSetTrue[0].GetAttribValue(bestSplitAttribute));
        }
        else
        {
            thisNode = new LessOrEqualThreshold_DecisionTreeNode(bestSplitAttribute, bestThreshold);
        }

        //Now assign the children nodes (After the recursive split)
        thisNode.SetTrueNode(MakeTree(bestSetTrue));
        thisNode.SetFalseNode(MakeTree(BestSetFalse));

        return thisNode;
    }

    // Calculates the entropy (measurement of how actions are distributed) for one set of records with the formula E = -Sum[Px *log(Px)]
    // Where Px is the proportion of action x in the records.
    //Adapted from "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009)
    private float Entropy(List<FightAIRecord> records)
    {
        Dictionary<int, int> actionCount = new Dictionary<int, int>(); // HashMap <action ID, count>
        float entropy = 0;

        if (records.Count <= 1) return entropy; //No actions or all actions the same is entropy = 0.

        //Count records for each action.
        for (int i = 0; i < records.Count; i++)
        {
            int skillID = records[i].GetAttribValue_Int(FightAIRecordAttributes.SKILL_USED);
            if (actionCount.ContainsKey(skillID)) actionCount[skillID]++;
            else actionCount.Add(skillID, 1);
        }

        if (actionCount.Count <= 1) return entropy; //Only one action for all records is entropy = 0;

        //Calculate entropy with the formula E = Sum[-Px log(Px)]. The real formula uses Log2, but we just need to compare
        //entropies, not finding the exact value, so this saves computation time.
        foreach(KeyValuePair<int, int> action in actionCount)
        {
            float proportion = (float)action.Value / (float)records.Count;
            entropy += -(proportion * Mathf.Log(proportion));
        }

        return entropy;
    }

    // Calculates the total entropy for more than one set of records.
    //Adapted from "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009)
    private float EntropyOfSets(List<FightAIRecord> set1, List<FightAIRecord> set2, int recordCount)
    {
        float entropy = 0;

        //Calculate proportion and entropy contribution for set1
        float proportion = (float)set1.Count / (float)recordCount;        
        entropy += proportion * Entropy(set1);

        //Calculate proportion and entropy contribution for set2
        proportion = (float)set2.Count / (float)recordCount;
        entropy += proportion * Entropy(set2);
  
        //Return total entropy
        return entropy;
    }

    //Function to split the records in one node in 2 sets, given the best information gain possible when dividing by the specified attribute. Being the attribute continuous, 
    //the division is placed in several places and the 2 groups with best information gained are returned in the bestSetTrue and bestSetFalse parameters (in/out). 
    //The method returns a float (the threshold value to make that division) bestSetTrue records have all a value less or equal than the threshold on the passed attribute   
    //Adapted from "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009).
    private float SplitByContinuousAttribute(List<FightAIRecord> records, out List<FightAIRecord> bestSetTrue, out List<FightAIRecord> bestSetFalse, FightAIRecordAttributes attribute)
    {        
        float bestGain = 0;                
        int totalRecordCount = records.Count;
        float initialEntropy = Entropy(records);

        bestSetTrue = new List<FightAIRecord>();
        bestSetFalse = new List<FightAIRecord>();

        //Initialise the 2 list that will be returned and copy the records into the false one.
        List<FightAIRecord> recordsTrue = new List<FightAIRecord>();            // The true set will contain records which attribute value is less or equal than the threshold.
        List<FightAIRecord> recordsFalse = new List<FightAIRecord>(records);    // The false set will contain records which attribute value is more than the threshold.

        //Sort in descendent order 
        recordsFalse.Sort(new FightAIRecord_SortByAtttribDescendent(attribute));
                
        //We check information gain for all combination
        while (recordsFalse.Count > 1)
        {
            //Pass the lowest attribute value example from records to records2
            recordsTrue.Add(recordsFalse[recordsFalse.Count - 1]);
            recordsFalse.RemoveAt(recordsFalse.Count - 1);

            //Find overall entropy and information gain
            float overallEntropy = EntropyOfSets(recordsTrue, recordsFalse, totalRecordCount);
            float informationGain = initialEntropy - overallEntropy;

            //Check against best so far.
            if (informationGain >= bestGain)
            {
                bestGain = informationGain;
                bestSetTrue = new List<FightAIRecord>(recordsTrue);
                bestSetFalse = new List<FightAIRecord>(recordsFalse);
            }
        }

        //Now that best one has been found, calculate threshold for division
        float threshold = bestSetTrue[bestSetTrue.Count - 1].GetAttribValue(attribute);
        threshold += bestSetFalse[bestSetFalse.Count - 1].GetAttribValue(attribute);
        threshold /= 2;

        return threshold;
    }

    //Split a node in 2 groups based on the value on a boolean attribute.
    //Adapted from "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009).
    private void SplitByBooleanAttribute(List<FightAIRecord> records, out List<FightAIRecord> bestSetTrue, out List<FightAIRecord> bestSetFalse, FightAIRecordAttributes attribute)
    {
        bestSetFalse = new List<FightAIRecord>();
        bestSetTrue = new List<FightAIRecord>();

        foreach (FightAIRecord record in records)
        {
            if (record.GetAttribValue_Bool(attribute)) bestSetTrue.Add(record);
            else bestSetFalse.Add(record);
        }
    }

    public void PrintToConsole()
    {
        string output = "List<FightAIRecord> temp = new List<FightAIRecord>();\n";
        for (int x = 0; x < fightHistoryRecords.Count; x++) //fightHistoryRecords.Count - value
        {            
            output += "{float[] array = " + fightHistoryRecords[x].PrintToString() + ";temp.Add(new FightAIRecord(array));}\n";
        }

        Debug.Log(output);        
    }

    public override List<FightAIRecord> GetRecords()
    {
        return fightHistoryRecords;
    }

    public override void ForgetSkill(int skillID)
    {
        bool removed = false;
        for (int x = 0; x < fightHistoryRecords.Count; x++)
        {
            if (fightHistoryRecords[x].GetAttribValue_Int(FightAIRecordAttributes.SKILL_USED) == skillID)
            {
                removed = true;
                fightHistoryRecords.RemoveAt(x);
                x--;

            }
        }
        if (removed) needsToRebuild = true;
    }
}