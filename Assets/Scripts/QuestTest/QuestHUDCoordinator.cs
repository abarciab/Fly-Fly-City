using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Drawing;

public class QuestHUDCoordinator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject ObjectiveCoordinatorPrefab, objectiveListParent;
    float objectiveStep = -1;
    Quest quest;

    private void Start()
    {
        QuestManager.instance.questHUD = this;
    }

    public void DisplayQuest()
    {
        if (quest != QuestManager.instance.currentQuest) SetupHUD();
        GetAndDisplayObjectives();
    }

    void GetAndDisplayObjectives()
    {
        ResetObjectiveHud();

        var objectives = quest.GetObjectivesToDisplay();
        if (objectives.Count == 0) return;

        for (int i = 0; i < objectives.Count; i++) {
            DisplayObjective(objectives[i]);
        }
    }

    void DisplayObjective (Quest.Objective toDisplay)
    {
        //print("displaying objective: " + toDisplay.name);
        var GO = Instantiate(ObjectiveCoordinatorPrefab, objectiveListParent.transform);
        var script = GO.GetComponent<QuestObjectiveCoordinator>();
        script.Setup(toDisplay);
    }

    void ResetObjectiveHud()
    {
        for (int i = 0; i < objectiveListParent.transform.childCount; i++) {
            Destroy(objectiveListParent.transform.GetChild(i).gameObject);
        }
    }

    void SetupHUD()
    {
        quest = QuestManager.instance.currentQuest;
        title.text = "<color=#808080>" + quest.questType + ":</color> <b>" + quest.name;
    }
}
