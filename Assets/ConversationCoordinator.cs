using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationCoordinator : MonoBehaviour
{
    [System.Serializable]
    public class SpeakerIdentity {
        public Speaker name;
        public Sprite portrait;
        public Color color;
    }

    [SerializeField] TextMeshProUGUI playerText, speakerText, speakerName;
    [SerializeField] Image Speaker1, Speaker2;
    Conversation.Line currentLine;
    bool animating;
    [SerializeField] float textSpeed = 0.5f;
    [SerializeField] List<SpeakerIdentity> allSpeakers = new List<SpeakerIdentity>();
    Conversation convo;

    private void Start()
    {
        GameManager.instance.convoCoordinator = this;
        HideVisuals();
    }

    public void StartConvo(Conversation _convo)
    {
        //print("hey");
        ShowVisuals();
        _convo.StartConversation();
        playerText.gameObject.SetActive(true);
        speakerText.gameObject.SetActive(true);
        convo = _convo;
        Speaker1.gameObject.SetActive(false);
        Speaker2.gameObject.SetActive(false);
        NextLine();
    }

    void HideVisuals()
    {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        GetComponent<Image>().enabled = false;
    }

    void ShowVisuals()
    {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        GetComponent<Image>().enabled = true;
    }

    void DisplayLine() 
    { 
        animating = true;
        playerText.text = "";
        speakerText.text = "";
        if (currentLine.metadata.speaker != Speaker.Nadira) {
            DisplaySpeaker();
        }
        StartCoroutine(AnimateLetters());
    }

    void DisplaySpeaker()
    {
        var identity = getIdentity();
        if (identity == null) return;
        if (Speaker1.gameObject.activeInHierarchy && Speaker1.sprite != identity.portrait) {
            Speaker2.sprite = Speaker1.sprite;
            Speaker2.gameObject.SetActive(true);
        }
        Speaker1.gameObject.SetActive(true);
        Speaker1.sprite = identity.portrait;
        speakerName.text = identity.name.ToString();
        speakerName.color = identity.color;
    }

    SpeakerIdentity getIdentity()
    {
        for (int i = 0; i < allSpeakers.Count; i++) {
            if (allSpeakers[i].name == currentLine.metadata.speaker) {
                return allSpeakers[i];
            }
        }
        return null;
    }

    public void EndConversation()
    {
        convo.EndConvo();
        HideVisuals();
    }

    IEnumerator AnimateLetters()
    {
        for (int i = 0; i < currentLine.line.Length; i++) {
            if (currentLine.metadata.speaker == Speaker.Nadira) {
                playerText.text += currentLine.line[i];
            }
            else {
                speakerText.text += currentLine.line[i];
            }
            yield return new WaitForSeconds(textSpeed * Time.deltaTime);
        }
        animating = false;
    }

    public void NextLine()
    {
        currentLine = convo.GetNextLine();
        if (currentLine == null || string.IsNullOrEmpty(currentLine.line)) EndConversation();
        else DisplayLine();
    }

    void SkipAnim()
    {
        StopAllCoroutines();
        animating = false;
        if (currentLine.metadata.speaker == Speaker.Nadira) playerText.text = currentLine.line;
        else speakerText.text = currentLine.line;
    }

    private void Update()
    {
        if (Input.GetKeyDown(GameManager.instance.jumpKey)) {
            if (animating){
                SkipAnim();
            }
            else {
                NextLine();
            }
        }
    }
}
