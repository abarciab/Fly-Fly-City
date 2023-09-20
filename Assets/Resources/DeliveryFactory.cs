using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public enum DeliveryItemAttribute { KeepCold, KeepWarm, Illegal, AvoidMetalDetectors}

[System.Serializable]
public class DeliveryItem
{
    public string name;
    public List<DeliveryItemAttribute> attributes = new List<DeliveryItemAttribute>();
}

[System.Serializable]
public class Delivery
{
    public string deliveryLocation;
    public string pickupLocation;
    public DeliveryItem item;
    public int difficulty;
    public int reward;
    public string client;
}

[CreateAssetMenu(fileName = "New DeliveryFactory")]
public class DeliveryFactory : ScriptableObject
{

    [Header("Difficulty Settings")]
    [SerializeField, Range(0, 3)] int attributesPerItem;

    [Space()]
    [SerializeField] TextAsset locationNamesTxt;
    [SerializeField] string[] locationNames = null;
    [SerializeField] TextAsset itemNamesTxt;
    [SerializeField] string[] itemNames = null;
    [SerializeField] TextAsset clientNamesTxt;
    [SerializeField] string[] clientNames = null;

    private void OnValidate()
    {
        Init();
    }

    void Init()
    {
        if (locationNamesTxt == null || itemNamesTxt == null || clientNamesTxt == null) return;
        itemNames = locationNames = clientNames = null;

        locationNames = Regex.Split(locationNamesTxt.text, "\n");
        itemNames = Regex.Split(itemNamesTxt.text, "\n");
        clientNames = Regex.Split(clientNamesTxt.text, "\n");
    }


    public Delivery GenerateDelivery()
    {
        if (locationNames == null) Init();

        var delivery = new Delivery();

        int pickupIndex = Random.Range(0, locationNames.Length);
        int dropOffIndex = pickupIndex;
        while (dropOffIndex == pickupIndex) dropOffIndex = Random.Range(0, locationNames.Length);

        delivery.pickupLocation = locationNames[pickupIndex];
        delivery.deliveryLocation = locationNames[dropOffIndex];

        delivery.item = GenerateDeliveryItem();

        delivery.difficulty = 1; //eventually calculate distance between places and factor in item attributes
        delivery.reward = Random.Range(10, 200) * 10; //difficulty rating + extra reward for certain districts

        delivery.client = clientNames[Random.Range(0, clientNames.Length)];

        return delivery;
    }

    DeliveryItem GenerateDeliveryItem()
    {
        var item = new DeliveryItem();

        float numAttributes = Random.Range(0, attributesPerItem + 1);
        for (int i = 0; i < numAttributes; i++) {
            var DIA = GetRandomDIA();
            while (item.attributes.Contains(DIA)) DIA = GetRandomDIA();
            item.attributes.Add(DIA);
        }
        item.name = itemNames[Random.Range(0, itemNames.Length)];

        return item;
    }

    DeliveryItemAttribute GetRandomDIA()
    {
        int numOptions = System.Enum.GetValues(typeof(DeliveryItemAttribute)).Length;
        return (DeliveryItemAttribute) Random.Range(0, numOptions);
    }


}


