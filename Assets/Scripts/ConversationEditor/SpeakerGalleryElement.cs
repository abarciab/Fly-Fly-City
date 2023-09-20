using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeakerGalleryElement: MonoBehaviour
{
    /*
    public SpeakerType characterName;
    ConversationEditor.TEMPPORTRAITCOLORS identity;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image portrait;
    public int lastUpdate;

    public void OnValidate() {
        if (ConversationEditor.instance == null) {
            return;   
        }
        
        identity = ConversationEditor.instance.GetIdentityFromName(characterName.ToString());
        if (identity == null) return;

        gameObject.name = characterName.ToString();
        if (nameText) nameText.text = identity.speaker.ToString();
        if (portrait) portrait.color = identity.speakerColor;
    }

    private void Update() {
        if (ConversationEditor.instance != null && lastUpdate != ConversationEditor.instance.dataUpdated) {
            lastUpdate = ConversationEditor.instance.dataUpdated;
            OnValidate();
        }
    }

    public void StartToReplace() {
        ConversationEditor.instance.activeMainSpeaker = this;
    }

    public void SelectAsReplacement() {
        ConversationEditor.instance.activeMainSpeaker.characterName = characterName;
        ConversationEditor.instance.activeMainSpeaker.OnValidate();
        ConversationEditor.instance.ActivateTextBox();
    }
    */
}
