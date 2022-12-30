using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//THIS SCRIPT IS JUST A WAY TO TEST THE QUEST SYSTEM
public class QuestTestCoordinator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentLocation;

    
    void Update()
    {
        currentLocation.text = "location: " + GameManager.instance.currentLocation;
    }
}
