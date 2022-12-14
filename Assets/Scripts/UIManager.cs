using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Dialogue references")]
    public Image leftPortrait;
    public Image rightPortrait;
    public GameObject textBox;
    Conversation activeConvo;

    private void Awake() {
        instance = this;
    }

    //dialogue display functions
    public void StartConversation(Conversation convo) {
        activeConvo = convo;
        DisplayLine(0);
    }

    void DisplayLine(int currentLine) {
        if (activeConvo.lines.Count <= currentLine) {
            activeConvo = null;
            return;
        }

        Conversation.Speaker currentSpeaker = null;
        foreach (Conversation.Speaker speaker in activeConvo.speakers) {
            if (speaker.name == activeConvo.lines[currentLine].speaker) {
                currentSpeaker = speaker;
                break;
            }
        }

        //activate the other speaker
        leftPortrait.sprite = currentSpeaker.portrait;
        rightPortrait.sprite = currentSpeaker.portrait;
        rightPortrait.gameObject.SetActive(!rightPortrait.gameObject.activeInHierarchy);
        leftPortrait.gameObject.SetActive(!leftPortrait.gameObject.activeInHierarchy);
        


    }

    private void Update() {
        
    }
}
