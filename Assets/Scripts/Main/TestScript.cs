using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TestScript : MonoBehaviour
{
    [SerializeField] Conversation convo;
    [SerializeField] bool startConvo;

    private void Update() {
        if (startConvo) {
            startConvo = false;
            GameManager.instance.convoCoordinator.StartConvo(convo);
        }
    }
}
