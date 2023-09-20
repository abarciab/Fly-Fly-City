using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing;
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
    public float hoursReqired;
    public Location destination;
    [ConditionalEnumHide(nameof(QuestType), (int)QuestType.ambush, (int)QuestType.type, Inverse = true)]
    public Sprite ListingImg;
    [ConditionalEnumHide(nameof(QuestType), (int)QuestType.deliver, Inverse = true)]
    public float payment;
    [ConditionalEnumHide(nameof(QuestType), (int)QuestType.deliver, Inverse = true)]
    public SpeakerType client;
    [HideInInspector] public bool questCompleted;

    //[HideInInspector] float objectiveStep = -1;     //as we complete objectives, update this value to match the last step completed
    [SerializeField] float objectiveStep = -1;     //as we complete objectives, update this value to match the last step completed

    [System.Serializable]
    public class Objective {
        public enum Type { generic, kill, destination, collect }
        public enum DestinationType { type, location, position}
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
        [HideInInspector] public int leftToKill;


        [ConditionalEnumHide(nameof(type), (int)Type.destination)]
        public DestinationType destinationType = DestinationType.type;
        [ConditionalEnumHide(nameof(destinationType), (int)DestinationType.position)]
        public Vector3 destination;
        [ConditionalEnumHide(nameof(destinationType), (int)DestinationType.position)]
        public float positionRadius = 5;
        [ConditionalEnumHide(nameof(destinationType), (int)DestinationType.location)]
        public Location location;

        [ConditionalEnumHide(nameof(type), (int)Type.collect)]
        public ItemNameOLD item;
        [ConditionalEnumHide(nameof(type), (int)Type.collect)]
        public int numToCollect = 1;
        [HideInInspector] int leftToCollect;

        [ConditionalEnumHide(nameof(type), (int)Type.generic)]
        public int completionTriggerID;
        [HideInInspector] public string triggerString;

        [Space(10)]
        public DisplayType display;
        [ConditionalEnumHide(nameof(display), (int)DisplayType.ProgressBar)]
        public int maxPoints = 10, points;
        public Reward reward;
        public bool completed;

        public void Init()
        {
            completed = false;
            FetchTriggerString();
            SetupPoints();
        }

        void SetupPoints()
        {
            if (display == DisplayType.checkBox) return;

            points = 0;
            if (type == Type.kill) {
                maxPoints = leftToKill = numToKill;
            }
            if (type == Type.kill) {
                maxPoints = leftToCollect = numToCollect;
            }
        }

        public void FetchTriggerString() {
            if (!Application.isPlaying) return;
            triggerString = QuestManager.instance.FetchTriggerString(completionTriggerID);
        }

        public float TriggerCheck(string _triggerString)
        {
            if (completed) return stepNum;
            completed = ShouldComplete(_triggerString);
            if (completed) {
                QuestManager.instance.RefeshQuestHUD();
                return stepNum;
            }
            return -1;
        }

        bool ShouldComplete(string _triggerString)
        {
            FetchTriggerString();
            if (type == Type.generic && _triggerString == triggerString) return true;
            if (type == Type.collect && _triggerString == item.ToString()) {
                numToCollect -= 1;
                points += 1;
                if (numToCollect <= 0) { return true;}
            }
            if (type == Type.kill && _triggerString == targetType.ToString()) {
                numToKill -= 1;
                points += 1;
                if (numToKill <= 0) { return true; }
            }
            if (type == Type.destination && destinationType == DestinationType.location && _triggerString == location.ToString()) return true;
            return false;
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
        objectiveStep = 0;
        for (int i = 0; i < objectives.Count; i++) {
            objectives[i].Init();
        }
    }

    public List<Objective> GetObjectivesToDisplay(bool recursiveCall = false)
    {
        int wholeStepNum = GetCurrentStepNum();
        var list = new List<Objective>();
        for (int i = 0; i < objectives.Count; i++) {
            if (objectives[i].stepNum >= wholeStepNum && objectives[i].stepNum < wholeStepNum + 1) {
                list.Add(objectives[i]);
            }
        }
        if (!recursiveCall && list.Count == 0 && !questCompleted) {

            return GetObjectivesToDisplay(true);
        }

        return list;
    }

    int GetCurrentStepNum()
    {
        int num = (int)Mathf.Floor(objectiveStep);
        return num;
    }

    public void ObjectiveTrigger(string trigger)
    {
        for (int i = 0; i < objectives.Count; i++) {
            var obj = objectives[i];

            if (Mathf.Floor(objectiveStep) + 1 <= obj.stepNum) continue;
            
            float objStep = obj.TriggerCheck(trigger);
            if (objectiveStep < objStep) objectiveStep = objStep;
        }
    }

    public string getListingName()
    {
        return questType.ToString() + ": " + name;
    }

    public string GetRwardString()
    {
        if (questType != QuestType.deliver) return null;
        return "Payment: " + payment + " Kleine";
    }

    public bool IsStepCompleted()
    {
        for (int i = 0; i < objectives.Count; i++) {
            var obj = objectives[i];

            if (Mathf.Floor(objectiveStep) + 1 <= obj.stepNum) continue;

            if (!obj.completed)return false;
        }
        return true;
    }

    public void NextStep()
    {
        objectiveStep = Mathf.Floor(objectiveStep) + 1;
        for (int i = 0; i < objectives.Count; i++) {
            if (objectives[i].stepNum >= objectiveStep) return;
        }
        questCompleted = true;
        QuestManager.instance.CompleteQuest();
    }

}
