using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class ConversationEditor : MonoBehaviour
{
    public static ConversationEditor instance;
    [System.Serializable]
    public class TEMPPORTRAITCOLORS
    {
        [HideInInspector] public string name;
        public Speaker speaker;
        public Color speakerColor = Color.white;
    }
    [HideInInspector]public int dataUpdated;

    [Header("Setup")]
    [SerializeField] Conversation currentConvo;
    [SerializeField] List<TEMPPORTRAITCOLORS> identities = new List<TEMPPORTRAITCOLORS>();
    [SerializeField] public GameObject previousLinePrefab;

    [Header("Styling")]
    [SerializeField] float leftSpeakerXpos;
    [SerializeField] float rightSpeakerXpos;
    
    [Header("Depenencies")]
    [SerializeField] GameObject assignConvoInstructions, metadataPanel;
    [SerializeField] SpeakerGalleryElement leftSpeaker;
    [SerializeField] SpeakerGalleryElement rightSpeaker;
    [SerializeField] TMP_InputField mainInputField, index, leadsTo, priority, reqFact, effFact;
    RectTransform mainInputRect;
    [SerializeField] TextMeshProUGUI mainPlaceholderText;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] TextMeshProUGUI indexDisplay;
    [SerializeField] GameObject previousLinesParent;
    List<PreviousLineCoordinator> previousLineRefs = new List<PreviousLineCoordinator>();
    [SerializeField] Scrollbar previousElementScrollBar;


    [Header("hotkeys")]
    [SerializeField] KeyCode completeLineKey = KeyCode.Return;
    [SerializeField] KeyCode swapSpeakerKey = KeyCode.LeftControl;
    [SerializeField] KeyCode newLineKey = KeyCode.Tilde;
    

    //runtime variables
    [HideInInspector] public SpeakerGalleryElement activeMainSpeaker;   //which speaker slot we're selecting a replacement for rn
    [HideInInspector] public bool leftSpeakerSelected;
    int currentLineIndex = 0;
    int currentID;

    private void OnValidate() {
        if (instance == null) Awake();

        for (int i = 0; i < identities.Count; i++) {
            identities[i].name = identities[i].speaker.ToString();
        }

        dataUpdated += 1;
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        if (!Application.isPlaying) return;

        mainInputRect = mainInputField.GetComponent<RectTransform>();
        currentLineIndex = 0;
        if (currentConvo.lines.Count == 0) {
            AddLine();
            DrawMain();
            return;
        }
        currentID = currentConvo.lines[currentConvo.lines.Count-1].metadata.ID;
        currentLineIndex = currentConvo.lines[currentConvo.lines.Count - 1].metadata.index;
        DrawMain();
        DisplayExistingLines();
    }

    public void ActivateTextBox() {
        mainInputField.ActivateInputField();
    }

    private void Update() {
        if (!Application.isPlaying) return;
        if (DisplayInstructions()) return;
        if (Input.GetKeyDown(completeLineKey)) CompleteLine();
        if (Input.GetKeyDown(swapSpeakerKey)) SwapSpeakers();
        if (Input.GetKeyDown(newLineKey)) mainInputField.text += "/n";
    }

    void DisplayExistingLines() {
        for (int i = 0; i < currentConvo.lines.Count; i++) {
            if (!string.IsNullOrEmpty(currentConvo.lines[i].line)) 
                StoreLineData(currentConvo.lines[i]);
        }
    }

    bool DisplayInstructions() {
        if (currentConvo == null) {
            assignConvoInstructions.SetActive(true);
            return true;
        }
        else assignConvoInstructions.SetActive(false);
        return false;
    }

    public void DeleteCurrentLine() {
        if (LineIsALreadyStored(currentID)) {
            var storedLine = GetPreviousLineRefByID(currentID);
            Destroy(storedLine.gameObject);
            previousLineRefs.Remove(storedLine);
        }
        currentConvo.DeleteLine(currentID);
        mainInputField.text = "";
        UpdateCurrentLineData();
    }

    public TEMPPORTRAITCOLORS GetIdentityFromName(string name) {
        for (int i = 0; i < identities.Count; i++) {
            if (identities[i].speaker.ToString() == name) {
                return identities[i];
            }
        }
        return null;
    }

    public void CompleteLine() {
        if (string.IsNullOrEmpty(mainInputField.text)) return;
        UpdateCurrentLineData();
        StoreCurrentLine();
        AddLine();
        UpdateCurrentLineData();
        DrawMain();
        mainInputField.ActivateInputField();
    }

    void StoreCurrentLine() {
        var lineToStore = currentConvo.getLineByID(currentID);
        StoreLineData(lineToStore);
    }

    void StoreLineData (Conversation.Line lineToStore) {
        if (LineIsALreadyStored(lineToStore)) {
            UpdateStoredLine(lineToStore);
            return;
        }
        MakeNewLineObj(lineToStore);
    }

    void MakeNewLineObj(Conversation.Line lineToStore) {
        var newObj = Instantiate(previousLinePrefab, previousLinesParent.transform);
        newObj.transform.SetAsFirstSibling();
        var coordinater = newObj.GetComponent<PreviousLineCoordinator>();
        coordinater.SetLine(lineToStore);
        previousLineRefs.Add(coordinater);
        previousElementScrollBar.value = 1;
    }

    void UpdateStoredLine (Conversation.Line updatedLine) {
        currentConvo.UpdateOrAddLine(updatedLine);
        for (int i = 0; i < previousLineRefs.Count; i++) {
            if (previousLineRefs[i].ID == updatedLine.metadata.ID) {
                previousLineRefs[i].UpdateLine(updatedLine);
            }
        }
    }

    public void AddAltLine() {
        //var currentLine = currentConvo.getLineByID(currentID);
        CompleteLine();
        currentConvo.getLineByID(currentID).metadata.index -= 1;
        currentConvo.getLineByID(currentID).metadata.nextIndex -= 1;
        DrawMain();
    }

    void AddLine() {
        if (currentConvo.lines.Count > 0) currentLineIndex = currentConvo.lines.Count;
        if (currentLineIndex == currentConvo.lines.Count) {
            var newLine = new Conversation.Line();
            var newdata = newLine.metadata;
            newdata.ID = Random.Range(1, 10000);
            while (GetLineByID(newdata.ID) != null) newdata.ID = Random.Range(1, 10000);
            currentID = newdata.ID;
            newdata.index = currentLineIndex;
            newdata.nextIndex = newdata.index + 1;
            newLine.metadata = newdata;
            currentConvo.UpdateOrAddLine(newLine);
            mainInputField.text = "";
        }
        
    }

    public void SwapSpeakers() {
        leftSpeakerSelected = !leftSpeakerSelected;
        UpdateCurrentLineData();
        DrawMain();
    }

    void UpdateCurrentLineData() {

        var newLine = currentConvo.getLineByID(currentID);
        if (newLine == null) {
            return;
        }

        var newdata = newLine.metadata;
        newdata.speaker = leftSpeakerSelected ? leftSpeaker.characterName : rightSpeaker.characterName;

        //newdata.index = newdata.index == -1? currentLineIndex : newdata.index;
        //newdata.nextIndex = newdata.nextIndex == -1? newdata.index + 1 : newdata.nextIndex;
        newdata.speakerLeft = leftSpeakerSelected;
        if (metadataPanel.activeInHierarchy) {
            ApplyMetadata(newdata);
        }
        //newdata.requiredFacts
        //newdata.factEffects

        newLine.metadata = newdata;
        newLine.line = mainInputField.text;
        currentConvo.UpdateOrAddLine(newLine);
    }

    public void OpenMetadataPanel() {
        var data = currentConvo.getLineByID(currentID).metadata;
        index.text = data.index.ToString();
        leadsTo.text = data.nextIndex.ToString();
        priority.text = data.priority.ToString();
        metadataPanel.SetActive(true);
    }

    public void CompleteConversation() {
        currentConvo = null;
    }

    public void ApplyMetadataToCurrentLine() {
        ApplyMetadata(currentConvo.getLineByID(currentID).metadata);
        metadataPanel.SetActive(false);
        ActivateTextBox();
        if (LineIsALreadyStored(currentID)) {
            StoreCurrentLine();
        }
        DrawMain();
    }

     void ApplyMetadata(Conversation.Line.Metadata data) {
        print("applying metadata!");
        data.priority = int.Parse(priority.text);
        data.index = int.Parse(index.text);
        data.nextIndex = int.Parse(leadsTo.text);
        //required facts
        //effect facts
    }

    void DrawMain() {
        AlignMainTextBox();
        DisplayCurrentLineData();
    }

    void DisplayCurrentLineData() {
        var line = currentConvo.getLineByID(currentID);
        var data = line.metadata;
        mainInputField.text = line.line;
        indexDisplay.text = "#" + data.index;
    }


    void AlignMainTextBox() {
        var newPos = mainInputRect.localPosition;
        newPos.x = leftSpeakerSelected ? leftSpeakerXpos : rightSpeakerXpos;
        mainInputRect.transform.localPosition = newPos;
        mainText.alignment = leftSpeakerSelected ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.TopRight;
        mainPlaceholderText.alignment = mainText.alignment;
    }

    public void SelectLine (Conversation.Line line) { SelectLine(line.metadata.ID); }
    public void SelectLine(int ID) {
        if (string.IsNullOrEmpty(GetLineByID(currentID).line)) {
            DeleteCurrentLine();
        }
        else {
            StoreCurrentLine();
        }

        var line = currentConvo.getLineByID(ID);
        currentID = ID;
        currentLineIndex = line.metadata.index;
        leftSpeakerSelected = line.metadata.speakerLeft;
        DrawMain();
        mainInputField.ActivateInputField();
    }

    public void ResetConversation() {
        currentConvo.ResetConversation();
        currentID = -1;
        previousLineRefs = new List<PreviousLineCoordinator>();
        for (int i = 0; i < previousLinesParent.transform.childCount; i++) {
            Destroy(previousLinesParent.transform.GetChild(i).gameObject);
        }
        Start();
        mainInputField.ActivateInputField();
    }

    public Conversation.Line GetLineByID(int ID) {
        for (int i = 0; i < currentConvo.lines.Count; i++) {
            if (currentConvo.lines[i].metadata.ID == ID) return currentConvo.lines[i];
        }
        return null;
    }

    public PreviousLineCoordinator GetPreviousLineRefByID(int ID) {
        for (int i = 0; i < previousLineRefs.Count; i++) {
            if (ID == previousLineRefs[i].ID) return previousLineRefs[i];
        }
        return null;
    }

    bool LineIsALreadyStored (Conversation.Line line) { return line != null && LineIsALreadyStored(line.metadata.ID); }
    bool LineIsALreadyStored(int ID) {
        for (int i = 0; i < previousLineRefs.Count; i++) {
            if (ID == previousLineRefs[i].ID) return true;
        }
        return false;
    }
}
