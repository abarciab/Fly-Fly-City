using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Constants;

public class ConversationDisplay : MonoBehaviour
{
    Conversation currentConvo;
    [SerializeField] TextMeshProUGUI mainText;
    Speaker currentSpeaker;

    private void Start()
    {
        if (!Directory.gMan.talking) gameObject.SetActive(false);
    }

    public void StartConversation(Conversation conversation, Speaker speaker)
    {
        currentSpeaker = speaker;
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
        if (lineData.playerSpeak) Directory.cam.LookPlayer();
        else Directory.cam.LookSpeaker();
        currentSpeaker.currentlyTalking = !lineData.playerSpeak;
        mainText.text = line;
    }

    void EndConversation()
    {
        gameObject.SetActive(false);
        Directory.gMan.EndConversation();
    }
}
