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
    //uncompleted quests
    //completed quests
    public Quest currentQuest;
    [SerializeField] List<QuestTrigger> questTriggers = new List<QuestTrigger>();

    [HideInInspector] public QuestHUDCoordinator questHUD;


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
        GameManager.OnQuestTrigger += RelayQuestTrigger;
    }

    public void RefeshQuestHUD()
    {
        if (currentQuest.IsStepCompleted()) {
            currentQuest.NextStep();
        }

        questHUD.DisplayQuest();
        //print("We just completed an objective and need to refresh the quest display, and maybe finish the quest");
    }

    public void CompleteQuest()
    {
        currentQuest = null;
        questHUD.DisplayQuest();
    }

    void RelayQuestTrigger()
    {
        currentQuest.ObjectiveTrigger(GameManager.instance.currentQuestTrigger);
    }

    public void StartNewQuest(Quest newQuest)
    {
        currentQuest = newQuest;
        currentQuest.Init();
        if (questHUD) {
            questHUD.DisplayQuest();
        }
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
