using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FlightSpeedUI : MonoBehaviour
{
    [Header("Delivery Generation")]
    [SerializeField] DeliveryFactory deliveryFactory;
    [SerializeField] List<Delivery> avaliableDeliveries = new List<Delivery>();
    [SerializeField] bool generate;
    [SerializeField] int numDeliveries = 4;
    [SerializeField] int currentlySelected = 0;

    [Header("List")]
    [SerializeField] GameObject listItemPrefab;
    [SerializeField] Transform listParent;
    [SerializeField] List<Image> listButtons;
    [SerializeField] Color selectedListColor, inactiveListColor;

    [Header("Selected")]
    [SerializeField] TextMeshProUGUI deliveryLocationName;
    [SerializeField] TextMeshProUGUI deliverDistrictName, pickupLocationName, pickupDistrictName, deliveryItemName, clientName, RewardText, DifficultyText;
    [SerializeField] string difficultyCharacter;

    [Header("DeliveryItemAttributes")]
    [SerializeField] TextMeshProUGUI DeliverItemAttribute1;
    [SerializeField] TextMeshProUGUI DeliverItemAttribute2;
    [SerializeField] TextMeshProUGUI DeliverItemAttribute3;

    [Header("TEMP")]
    [SerializeField] List<string> districtNames = new List<string>();
    

    private void Update()
    {
        if (generate) {
            generate = false;

            avaliableDeliveries.Clear();

            for (int i = 0; i < numDeliveries; i++) {
                avaliableDeliveries.Add(deliveryFactory.GenerateDelivery());
            }
        }

        for (int i = 0; i < listButtons.Count; i++) {
            listButtons[i].gameObject.name = i.ToString();
            if (i >= avaliableDeliveries.Count) listButtons[i].gameObject.SetActive(false);
            else FormatListButton(avaliableDeliveries[i], listButtons[i]);
        }
        if (currentlySelected < avaliableDeliveries.Count) DisplayDeliveryData(avaliableDeliveries[currentlySelected]);
    }

    void FormatListButton(Delivery delivery, Image button)
    {
        var coord = button.GetComponent<DeliveryBriefCoord>();
        coord.Init(GetDistrict(delivery.deliveryLocation), delivery.reward, delivery.difficulty, difficultyCharacter);
    }

    public void SelectDelivery(Image buttonImage)
    {
        foreach (var b in listButtons) b.color = inactiveListColor;
        buttonImage.color = selectedListColor;

        var num = int.Parse(buttonImage.gameObject.name);
        currentlySelected = num;
    }

    void DisplayDeliveryData(Delivery current)
    {
        if (current == null) return;

        deliveryLocationName.text = current.deliveryLocation;
        deliverDistrictName.text = GetDistrict(current.deliveryLocation);

        pickupLocationName.text = current.pickupLocation;
        pickupDistrictName.text = GetDistrict(current.pickupLocation);

        DeliverItemAttribute1.gameObject.SetActive(false);
        DeliverItemAttribute2.gameObject.SetActive(false);
        DeliverItemAttribute3.gameObject.SetActive(false);

        var item = current.item;
        deliveryItemName.text = item.name;
        if (item.attributes.Count > 0) {
            DeliverItemAttribute1.text = difficultyCharacter + item.attributes[0].ToString();
            DeliverItemAttribute1.gameObject.SetActive(true);
        }
        if (item.attributes.Count > 1) {
            DeliverItemAttribute2.text = difficultyCharacter + item.attributes[1].ToString();
            DeliverItemAttribute2.gameObject.SetActive(true);
        }
        if (item.attributes.Count > 2) {
            DeliverItemAttribute3.text = difficultyCharacter + item.attributes[2].ToString();
            DeliverItemAttribute3.gameObject.SetActive(true);
        }

        clientName.text = "Client: \n" + current.client;
        RewardText.text = "Reward: \n" + current.reward.ToString("C0");

        DifficultyText.text = "Difficulty:\n"; 
        for (int i = 0; i < current.difficulty; i++) {
            DifficultyText.text += difficultyCharacter;
        }
    }

    string GetDistrict(string locationName)
    {
        int num = 0;
        foreach (char letter in locationName) num += (int) letter % 32;
        return districtNames[num % districtNames.Count] + " District";
    }

}
