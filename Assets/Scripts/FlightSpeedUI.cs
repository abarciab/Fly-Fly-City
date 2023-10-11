using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlightSpeedUI : MonoBehaviour
{
    FlightSpeedController controller;
    [SerializeField] int currentlySelected = 0;

    [Header("List")]
    [SerializeField] GameObject listItemPrefab;
    //[SerializeField] Transform listParent;
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

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Directory.cam.enabled = false;
    }

    private void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (Directory.cam) Directory.cam.enabled = true;
    }

    private void Start()
    {
        controller = Directory.fMan;
    }

    private void Update()
    {
        for (int i = 0; i < listButtons.Count; i++) {
            listButtons[i].gameObject.name = i.ToString();
            if (i >= controller.avaliableDeliveryCount) listButtons[i].gameObject.SetActive(false);
            else FormatListButton(controller.GetDelivery(i), listButtons[i]);
        }
        if (currentlySelected < controller.avaliableDeliveryCount) DisplayDeliveryData(controller.GetDelivery(currentlySelected));
    }

    void FormatListButton(Delivery delivery, Image button)
    {
        var coord = button.GetComponent<DeliveryBriefCoord>();
        coord.Init(Directory.current.GetDistrictName(delivery.deliveryLocation), delivery.reward, delivery.difficulty, difficultyCharacter);
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
        deliverDistrictName.text = Directory.current.GetDistrictName(current.deliveryLocation);

        pickupLocationName.text = current.pickupLocation;
        pickupDistrictName.text = Directory.current.GetDistrictName(current.pickupLocation);

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

    public void AcceptDeliver()
    {
        controller.StartDelivery(currentlySelected);
        gameObject.SetActive(false);
    }
}
