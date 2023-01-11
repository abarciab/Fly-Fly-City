using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerData : MonoBehaviour
{
    [SerializeField] List<Conversation> possibleConvos;

    public void StartNextConvo()
    {
        GameManager.instance.convoCoordinator.StartConvo(possibleConvos[0]);
    }
}
