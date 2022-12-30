using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest")]
public class Quest : ScriptableObject
{
    public enum QuestType {type, deliver, investigate, sabatoge, ambush }
    public QuestType questType = QuestType.type;

    
    new public string name;
    [TextArea(3, 5)]
    public string description;
    public List<Objective> objectives;
    public Condition preCondition;

    [System.Serializable]
    public class Objective {
        public enum Type { generic, kill, destination, collect }
        public enum DisplayType { checkBox, ProgressBar }

        public string name;
        [Tooltip("mark true if completing this 'objective' should fail the quest (like killing your ward in an escort quest)")]
        public bool failureCondition;
        public float stepNum;     //for quests that have specific steps, objectives can only be completed once the previous ones are done. 0 is default. decimals are all displayed together
        public string hudDisplayText;       //make this fancy, with some clever text to mark checkboxes that get filled n stuff 

        [Tooltip("generic objectives aren't completed automatically, and must be called from a script")]
        public Type type;

        [ConditionalEnumHide(nameof(type), (int)Type.kill)]
        public EntityType targetType;
        [ConditionalEnumHide(nameof(type), (int)Type.kill)]
        public int numToKill;

        [ConditionalEnumHide(nameof(type), (int)Type.destination)]
        public Vector3 destination;
        [ConditionalEnumHide(nameof(type), (int)Type.destination)]
        public float positionRadius = 5;

        [ConditionalEnumHide(nameof(type), (int)Type.collect)]
        public Item item;
        [ConditionalEnumHide(nameof(type), (int)Type.collect)]
        public int numToCollect = 1;

        public DisplayType display;

        [ConditionalEnumHide(nameof(display), (int)DisplayType.ProgressBar)]
        public int maxPoints = 10, points;

        public int completionTriggerID;
        public string triggerString;

        [Space(10)]
        public Reward reward;

        public void FetchTriggerString() {
            if (!Application.isPlaying) return;
            triggerString = QuestManager.instance.FetchTriggerString(completionTriggerID);
        }
    }
    [System.Serializable]
    public class Condition
    {
        public List<Location> locations = new List<Location>();
        public Vector2 dayRange = new Vector2();
        public Vector2 minMaxLevel = new Vector2(-1, -1);
        public List<Fact> requiredFacts = new List<Fact>();
        [Tooltip("number of times this quest will appear before it's gone forever. set to -1 if you don't want it to disapear")]
        public int numOppourtunities = -1;
    }

    [System.Serializable]
    public class Reward
    {
        public enum Type {none, item, conversation, money}
        public Type type;

        [ConditionalEnumHide(nameof(type), (int)Type.item)]
        public Item item;
        [ConditionalEnumHide(nameof(type), (int)Type.item)]
        public int numToCollect = 1;

        [ConditionalEnumHide(nameof(type), (int)Type.conversation)]
        public Conversation conversation;

        [ConditionalEnumHide(nameof(type), (int)Type.money)]
        public float amount;
    }

    public void Init() {
        for (int i = 0; i < objectives.Count; i++) {
            objectives[i].FetchTriggerString();
        }
    }
}
