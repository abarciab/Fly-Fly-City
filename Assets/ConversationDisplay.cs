using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Constants;

public class ConversationDisplay : MonoBehaviour
{
    Conversation currentConvo;
    [SerializeField] TextMeshProUGUI mainText;

    private void Start()
    {
        if (!GameManager.instance.talking) gameObject.SetActive(false);
    }

    public void StartConversation(Conversation conversation)
    {
        currentConvo = conversation;
        gameObject.SetActive(true);
        NextLine();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey)) NextLine();
    }

    void NextLine()
    {
        var lineData = currentConvo.GetNextLine();
        if (lineData == null) {
            EndConversation();
            return;
        }

        string line = lineData.line;
        if (lineData.playerSpeak) GameManager.instance.CameraLookPlayer();
        else GameManager.instance.CameraLookSpeaker();
        mainText.text = line;
    }

    void EndConversation()
    {
        gameObject.SetActive(false);
        GameManager.instance.EndConversation();
    }
}
