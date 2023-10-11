
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

[System.Serializable]
public class ItemNamePair
{
    public string name;
    public ItemNameOLD itemName;
}

public class GameManager : MonoBehaviour
{
    public List<string> facts = new List<string>();         //CHANGE THIS: make it its own class (or SO? or enum?). this word list contains true things about the current state of the game.
                                                            //"wingsUnlocked", for example is in this list when Nadira has acsess to her wings.

    [SerializeField] List<ItemNamePair> itemDIsplayNames = new List<ItemNamePair>();
    public string currentQuestTrigger;

    public int currentDistrict;

    [Header("Dependencies")]
    public PlayerStateCoordinator player;
    public ConversationDisplay conversationDisplay;
    public Wipe wipe;
    [SerializeField] TextMeshProUGUI newLocationText, newDistrictText;    


    
    [Header("Game State")]
    public bool insideLocation;
    string _currentLocation;
    public string currentLocation { get { return insideLocation ? _currentLocation : ""; } }

    [HideInInspector] public bool talking;
    Speaker currentSpeaker;

    [Header("Prefabs")]
    [SerializeField] List<GameObject> deliveryClientPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> deliveryRecipientPrefabs = new List<GameObject>();

    public GameObject GetDeliveryClient()
    {
        return deliveryClientPrefabs[Random.Range(0, deliveryClientPrefabs.Count)];
    }

    public GameObject GetDeliveryRecipient()
    {
        return deliveryRecipientPrefabs[Random.Range(0, deliveryRecipientPrefabs.Count)];
    }

    public void EnterNewDistrict(int districtID)
    {
        currentDistrict = districtID;
        ShowNewDistrictText();
    }

    void ShowNewDistrictText()
    {
        newDistrictText.text = Directory.current.GetDistrictName(currentDistrict) + " District";
        newDistrictText.gameObject.SetActive(true);
    }

    public void SetInside(bool _insideLocation)
    {
        insideLocation = _insideLocation;
    }

    public void EnterNewLocation(string locationName)
    {
        print("Entered new location: " + locationName);
        _currentLocation = locationName;
        Directory.fMan.GenerateDeliveries();

        newLocationText.text = locationName;
        newLocationText.gameObject.SetActive(true);
    }

    public void StartConversation(Conversation convo, Speaker speaker)
    {
        talking = true;
        player.SetPaused(true);
        Directory.cam.StartConversation(speaker);
        conversationDisplay.StartConversation(convo, speaker);
        currentSpeaker = speaker;
    }

    public void EndConversation()
    {
        talking = false;
        player.SetPaused(false);
        player.EndConversation();
        Directory.cam.EndConversation();
        currentSpeaker.EndConversation();
    }

    public void CameraLookPlayer()
    {
        Directory.cam.LookAtPlayer();
    }

    public void QuestTrigger(string questString)
    {
        currentQuestTrigger = questString;
        QuestTrigger();
    }

    void QuestTrigger()
    {
        print("trigger: " + currentQuestTrigger);
    }
    
    private void Update() {
        if (Input.GetKeyDown(resetKey)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Directory.uiMan.busy) return;
        bool rightClickOrAltDown = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftAlt);  
        if (rightClickOrAltDown) {
            if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftAlt)) Directory.mapMan.ExpandMap();
        }
        Directory.mapMan.SetMapActive(Input.GetKey(KeyCode.LeftAlt), false);
    }
}
