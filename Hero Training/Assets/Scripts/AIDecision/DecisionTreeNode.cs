using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* The classes TreeNode, ActionTreeNode, DecisionTreeNode, LessOrEqualThreshold_DecisionTreeNode and EqualTo_DecisionTreeNode
 * have been adapted from the book "Artificial Intelligence for games" 2nd Edition (Millington & Funge, 2009).*/

//Base class for a tree node
[Serializable]
public abstract class TreeNode
{
    public abstract int ChooseSkill(FightAIRecord record);
}

//Action node (Leaf node). Returns the skill ID of the action
[Serializable]
public class ActionTreeNode : TreeNode
{
    int chosenID;

    public ActionTreeNode(int skillID) { chosenID= skillID; }

    public override int ChooseSkill(FightAIRecord record)
    {
        return chosenID;
    }
}

//Base class for a Decision node (branch node). Performs a test to keep going down the tree until an action node is found.
//The ChooseBranch method is supossed to implement the test for that node on derived classes.
[Serializable]
public abstract class DecisionTreeNode : TreeNode
{
    //Binary tree, only two children.
    protected TreeNode trueNode, falseNode;

    //Variables for the test. Attribute to test and threshold
    protected FightAIRecordAttributes attrib;

    protected abstract TreeNode ChooseBranch(float testValue);

    //Recursively goes through the tree to find a skill to return given the record passed.
    public override int ChooseSkill(FightAIRecord record)
    {
        float value = record.GetAttribValue(attrib);
        TreeNode child = ChooseBranch(value);
        return child.ChooseSkill(record);
    }

    public void SetTrueNode(TreeNode node) { trueNode = node; }
    public void SetFalseNode(TreeNode node) { falseNode = node; }
}

//Decision node for a "Less or equal than" test
[Serializable]
public class LessOrEqualThreshold_DecisionTreeNode : DecisionTreeNode
{
    float thresholdValue;

    //Constructor
    public LessOrEqualThreshold_DecisionTreeNode(FightAIRecordAttributes attrib, float thresholdValue)
    {
        this.attrib = attrib;
        this.thresholdValue = thresholdValue;
    }

    //Performs the test to get which branch should we go to find an action node.
    protected override TreeNode ChooseBranch(float testValue)
    {
        if (testValue <= thresholdValue) return trueNode;
        else return falseNode;
    }  
}

//Decision node for an equality test
[Serializable]
public class EqualTo_DecisionTreeNode : DecisionTreeNode
{
    float value;

    //Constructor
    public EqualTo_DecisionTreeNode(FightAIRecordAttributes attrib, float value)
    {
        this.attrib = attrib;
        this.value = value;
    }

    //Performs the test to get which branch should we go to find an action node.
    protected override TreeNode ChooseBranch(float testValue)
    {
        if (testValue == value) return trueNode;
        else return falseNode;
    }
}