using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreviousLineCoordinator : MonoBehaviour
{
    public int  ID { get; private set; }
    Conversation.Line line;
    public bool speakerLeft { get; private set; }

    [SerializeField] GameObject rightText, leftText;
    [SerializeField] Image leftSpeaker, rightSpeaker;
    [SerializeField] TextMeshProUGUI mainText, leftIndex, rightIndex, leftName, rightName;

    public void SetLine(Conversation.Line line) { SetLine(line.metadata.ID);  }
    public void SetLine(int _ID) {
        ID = _ID;
        line = ConversationEditor.instance.GetLineByID(ID);
        speakerLeft = line.metadata.speakerLeft;
        UpdateVisuals();
    }

    public void UpdateLine (Conversation.Line _line) {
        speakerLeft = _line.metadata.speakerLeft;
        line = _line;
        UpdateVisuals();
    }

    void UpdateVisuals() {
        rightText.SetActive(!speakerLeft);
        rightSpeaker.gameObject.SetActive(!speakerLeft);

        leftText.SetActive(speakerLeft);
        leftSpeaker.gameObject.SetActive(speakerLeft);

        mainText.alignment = speakerLeft ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.TopRight;
        mainText.text = line.line;
        leftIndex.text = rightIndex.text = line.metadata.index.ToString();
        leftName.text = rightName.text = line.metadata.speaker.ToString();
        leftSpeaker.color = rightSpeaker.color = ConversationEditor.instance.GetIdentityFromName(line.metadata.speaker.ToString()).speakerColor;
    }

    public void SelectLine() {
        ConversationEditor.instance.SelectLine(ID);
    }
}
