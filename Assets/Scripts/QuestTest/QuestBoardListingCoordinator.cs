using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestBoardListingCoordinator : MonoBehaviour
{
    [SerializeField] Quest quest;

    [SerializeField] TextMeshProUGUI listingTitle;
    [HideInInspector] public QuestBoardCoodinator questBoard;

    private void Start()
    {
        if (questBoard == null) {
            questBoard = FindObjectOfType<QuestBoardCoodinator>();
        }
        listingTitle.text = quest.getListingName();
    }

    public void OpenMenu()
    {
        questBoard.DisplayListing(quest);
    }

    public void SelectQuest()
    {
        QuestManager.instance.StartNewQuest(quest);
    }
}
