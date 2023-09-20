
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;


public enum Fact {None, WingsUnlocked, MikaDead, Tier3Reached, MikaAngry}   
public enum Location { traveling, bar, apartment, hotel, waterTreatmentPlant }  

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemNamePair
    {
        public string name;
        public ItemNameOLD itemName;
    }

    

    public static GameManager instance;
    public List<string> facts = new List<string>();         //CHANGE THIS: make it its own class (or SO? or enum?). this word list contains true things about the current state of the game.
                                                            //"wingsUnlocked", for example is in this list when Nadira has acsess to her wings.

    [SerializeField] List<ItemNamePair> itemDIsplayNames = new List<ItemNamePair>();
    public static Action OnQuestTrigger;
    public string currentQuestTrigger;
    
    [Header("Dependencies")]
    public PlayerStateCoordinator player;
    public ConversationDisplay conversationDisplay;
    public Wipe wipe;
    [SerializeField] TextMeshProUGUI newLocationText;
    [SerializeField] CameraController cameraController;
    
    

    [Space()]
    [SerializeField] GameObject FlightSpeedUI;
    

    [Header("Game State")]
    public Location currentLocation;
    public bool insideLocation;

    [HideInInspector] public bool talking;

    public void EnterNewLocation(string locationName)
    {
        newLocationText.text = locationName;
        newLocationText.gameObject.SetActive(true);
    }

    public void StartConversation(Conversation convo, Transform speaker)
    {
        talking = true;
        player.SetPaused(true);
        cameraController.StartConversation(speaker);
        conversationDisplay.StartConversation(convo);
    }

    public void EndConversation()
    {
        talking = false;
        player.SetPaused(false);
        cameraController.EndConversation();
    }

    public void CameraLookPlayer()
    {
        cameraController.LookAtPlayer();
    }

    public void CameraLookSpeaker()
    {
        cameraController.LookAtSpeaker();
    }

    public void QuestTrigger(int triggerID)
    {
        currentQuestTrigger = QuestManager.instance.FetchTriggerString(triggerID);
        QuestTrigger();
    }

    public void QuestTrigger(string questString)
    {
        currentQuestTrigger = questString;
        QuestTrigger();
    }

    void QuestTrigger()
    {
        print("trigger: " + currentQuestTrigger);
        if (OnQuestTrigger != null) OnQuestTrigger.Invoke();
    }

    private void Awake() {
        instance = this;
        OnQuestTrigger = null;
    }
    
    private void Update() {
        if (Input.GetKeyDown(resetKey)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (FlightSpeedUI.activeInHierarchy) cameraController.enabled = false;
        else cameraController.enabled = true;
    }

    public void BeginTravelling()
    {
        EnterLocation(Location.traveling);
    }

    void UnloadScene(int sceneIndex) {
        SceneManager.UnloadSceneAsync(sceneIndex);
    }

    public void LoadScene(int sceneIndex) {
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    public void EnterLocation(Location newLocation) {
        QuestTrigger(newLocation.ToString());
        currentLocation = newLocation;
    }

    public void setChildTags(GameObject parent, string tag)
    {
        for (int i = 0; i < parent.transform.childCount; i++) {
            setChildTags(parent.transform.GetChild(i).gameObject, tag);
        }
        parent.tag = tag;
    }
}
