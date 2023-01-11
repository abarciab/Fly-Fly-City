using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//THIS SCRIPT IS JUST A WAY TO TEST THE QUEST SYSTEM
public class QuestTestCoordinator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentLocation;
    [SerializeField] GameObject interior, locations;
    
    void Update()
    {
        currentLocation.text = "location: " + GameManager.instance.currentLocation;
        if (GameManager.instance.currentLocation != Location.traveling) {
            interior.SetActive(true);
            locations.SetActive(false);
        }
    }

    public void StartTravelling()
    {
        interior.SetActive(false);
        locations.SetActive(true);
        GameManager.instance.BeginTravelling();
    }
}
