using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestLocation {
    public enum Location { bar, apartment, hotel}

    [HideInInspector]public string name = "hi";
    public Location location;
    
}

[System.Serializable]
public class Quest {
    [System.Serializable]
    public class Objective
    {
        public enum Type {generic, kill, location}
        [Header("required")]
        public string name;
        [Tooltip("generic objectives aren't completed automatically, and must be called from a script")]
        public Type type;
        [Tooltip("mark true if completing this 'objctive' should fail the quest (like killing your ward in an escort quest)")]
        public bool failureCondition;       

        [Header("Optional")]
        public int stepNum;     //for quests that have specific steps, objectives can only be completed once the previous ones are done. steps with num 0 can be done in any order
        public int numToKill;
        public string targetType;       //eventually replace this with a nice enum
        public Vector3 postion;
        public float positionRadius = 5;
        public string hudDisplayText;       //make this fancy, with some clever text to mark checkboxes that get filled n stuff 
    }
    [System.Serializable]
    public class Condition
    {
        public List<QuestLocation.Location> locations = new List<QuestLocation.Location>();
        public Vector2 dayRange = new Vector2();
        public Vector2 minMaxLevel = new Vector2(-1, -1);
        public List<Fact> requiredFacts = new List<Fact>();
        [Tooltip("number of times this quest will appear before it's gone forever. set to -1 if you don't want it to disapear")]
        public int numOppourtunities = -1;
    }

    public enum Type { deliver, investigate, advance }

    public string name;
    [TextArea(3, 5)]
    public string description;
    public List<Objective> objectives;
    public Condition requirements;
    //some way to specify what kind of quest marker should be used and what should display in the HUD

}


[ExecuteAlways]
public class QuestManager : MonoBehaviour {

    public static QuestManager instance;

    public List<QuestLocation> locations = new List<QuestLocation>();
    public List<Quest> quests = new List<Quest>();

    private void Awake()
    {
        instance = this;
    }

    private void OnValidate()
    {
        foreach (var location in locations) {
            if (location.name != location.location.ToString()) {
                location.name = location.location.ToString();
            }
        }
    }
}
