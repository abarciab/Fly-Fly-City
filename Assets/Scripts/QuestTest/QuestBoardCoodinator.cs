using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBoardCoodinator : MonoBehaviour
{


    [SerializeField] GameObject listingPrefab;

    [Header("Listing display Dependencies")]
    [SerializeField] GameObject DisplayParent;
    [SerializeField] TextMeshProUGUI title, description, reqTime, dest, client, payment;
    [SerializeField] Image listingImg;
    Quest displayQuest;

    public void DisplayListing(Quest toDisplay)
    {
        title.text = toDisplay.getListingName();
        description.text = toDisplay.description;
        reqTime.text = "Time required: " + toDisplay.hoursReqired + " hours";
        dest.text = "Destination: " + toDisplay.destination;
        if (toDisplay.questType == Quest.QuestType.deliver) {
            client.text = "Client: " + toDisplay.client;
            client.gameObject.SetActive(true);
        }
        else client.gameObject.SetActive(false);
        payment.text = toDisplay.GetRwardString();
        listingImg.sprite = toDisplay.ListingImg;
        displayQuest = toDisplay;

        DisplayParent.SetActive(true);
    }

    public void AcceptQuest()
    {
        QuestManager.instance.StartNewQuest(displayQuest);
        DisplayParent.SetActive(false);
        //leave the quest board entirely
    }

    public void DeclineQuest()
    {
        DisplayParent.SetActive(false);
    }
}
