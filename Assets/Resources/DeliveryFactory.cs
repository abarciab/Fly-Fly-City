using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Hardware;
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
    public float timeLimit;
}

[CreateAssetMenu(fileName = "New DeliveryFactory")]
public class DeliveryFactory : ScriptableObject
{

    [Header("Difficulty Settings")]
    [SerializeField, Range(0, 3)] int attributesPerItem;
    [SerializeField] float distTimeMult = 100, maxReward;
    public float maxDist = 150;
    public int minReward = 100;

    [Space()]
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
        if (itemNamesTxt == null || clientNamesTxt == null) return;
        itemNames = clientNames = null;

        itemNames = Regex.Split(itemNamesTxt.text, "\n");
        clientNames = Regex.Split(clientNamesTxt.text, "\n");
    }


    public Delivery GenerateDelivery(int currentDistrict)
    {
        if (itemNamesTxt == null) Init();
        var delivery = new Delivery();

        float dist;
        do {
            delivery.pickupLocation = Directory.current.GetRandomDifferentLocation(new List<string> { Directory.gMan.currentLocation }, currentDistrict).name;
            delivery.deliveryLocation = Directory.current.GetRandomDifferentLocation(new List<string> { delivery.pickupLocation, Directory.gMan.currentLocation }).name;
            dist = Directory.current.GetDistance(delivery.pickupLocation, delivery.deliveryLocation);
        } while (dist > maxDist);

        float distFactor = dist / maxDist;
        delivery.difficulty = Mathf.RoundToInt(distFactor * 5);
        delivery.reward = Mathf.RoundToInt(distFactor * maxReward);
        delivery.timeLimit = distFactor * distTimeMult;

        delivery.item = GenerateDeliveryItem();
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


