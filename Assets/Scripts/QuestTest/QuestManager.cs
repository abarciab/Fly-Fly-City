using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class QuestManager : MonoBehaviour {
    [System.Serializable]
    public class LocationData
    {
        [HideInInspector] public string name;
        public Location location;
    }

    [System.Serializable]
    public class QuestTrigger
    {
        [HideInInspector] public string name;
        [HideInInspector]public int ID;
        public string trigger;
    }

    public static QuestManager instance;

    [SerializeField] List<LocationData> locations = new List<LocationData>();
    //[SerializeField] List<Quest> quests = new List<Quest>();
    public Quest currentQuest;
    [SerializeField] List<QuestTrigger> questTriggers = new List<QuestTrigger>();

    private void Awake()
    {
        instance = this;
    }

    private void OnValidate() {
        for (int i = 0; i < locations.Count; i++) {
            locations[i].name = locations[i].location.ToString();
        }
        for (int i = 0; i < questTriggers.Count; i++) {
            var trigger = questTriggers[i];
            trigger.ID = i;
            trigger.name = trigger.ID + ": " + trigger.trigger;
            questTriggers[i] = trigger;
        }
    }

    private void Start() {
        currentQuest.Init();
    }

    public string FetchTriggerString(int triggerID) {
        for (int i = 0; i < questTriggers.Count; i++) {
            if (questTriggers[i].ID == triggerID) {
                return questTriggers[i].trigger;
            }
        }
        return null;
    }
}
