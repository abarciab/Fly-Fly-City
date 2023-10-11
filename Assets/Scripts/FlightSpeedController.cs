using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeliveryStage { pickingUp, transporting, complete}

public class FlightSpeedController : MonoBehaviour
{
    [Header("Delivery Generation")]
    [SerializeField] DeliveryFactory deliveryFactory;
    [SerializeField] List<Delivery> avaliableDeliveries = new List<Delivery>();
    [SerializeField] int numDeliveries = 4;
    [SerializeField] bool debug;

    Delivery currentDelivery;
    DeliveryStage currentStage;
    public bool onDelivery { get { return currentDelivery != null && !string.IsNullOrEmpty(currentDelivery.pickupLocation); } }
    
    public int avaliableDeliveryCount { get { return avaliableDeliveries.Count; } }

    LocationController pickupLocation, dropOffLocation;

    float timeLeft;

    private void Update()
    {
        if (currentStage == DeliveryStage.transporting && !Directory.gMan.insideLocation) timeLeft -= Time.deltaTime;
    }

    public void GenerateDeliveries()
    {
        avaliableDeliveries.Clear();
        for (int i = 0; i < numDeliveries; i++) {
            avaliableDeliveries.Add(deliveryFactory.GenerateDelivery(Directory.gMan.currentDistrict));
        }
    }

    public Delivery GetDelivery(int index)
    {
        return avaliableDeliveries[index];
    }

    public void StartDelivery(int index)
    {
        currentDelivery = avaliableDeliveries[index];
        currentStage = DeliveryStage.pickingUp;
        
        pickupLocation = Directory.current.GetLocationRefernce(currentDelivery.pickupLocation);
        dropOffLocation = Directory.current.GetLocationRefernce(currentDelivery.deliveryLocation);
        pickupLocation.ActivateMarkerAsPickup();
        pickupLocation.SpawnDeliveryClient();
    }

    public void AdvanceDeliveryStage()
    {
        if (currentDelivery == null) return;

        currentStage += 1;
        if (currentStage == DeliveryStage.transporting) {
            timeLeft = currentDelivery.timeLimit;
            pickupLocation.MarkerRegular();
            dropOffLocation.ActivateMarkerAsDropoff();
            dropOffLocation.SpawnDeliveryRecipient();
        }
        if (currentStage == DeliveryStage.complete) {
            int reward = Mathf.RoundToInt(GetDeliveryTimePercent() * currentDelivery.reward);
            dropOffLocation.MarkerRegular();
            Directory.player.AddMoney(Mathf.Max(deliveryFactory.minReward, reward));

            currentDelivery = null;
        }
    }

    public float GetDeliveryTimePercent()
    {
        if (currentDelivery == null || currentStage == DeliveryStage.pickingUp) return -1;
        return Mathf.Max(0, timeLeft) / currentDelivery.timeLimit;
    }

    public float GetDeliveryTime()
    {
        if (currentDelivery == null || currentStage == DeliveryStage.pickingUp) return -1;
        return timeLeft;
    }

    private void OnDrawGizmos()
    {
        if (debug) Gizmos.DrawWireSphere(transform.position, deliveryFactory.maxDist);
    }

}
