using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Fact {None, WingsUnlocked, MikaDead, Tier3Reached, MikaAngry}   //every word capital
public enum Location { traveling, bar, apartment, hotel, waterTreatmentPlant }  




public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemNamePair
    {
        public string name;
        public ItemName itemName;
    }

   
    

    [Header("Controls")]
    public KeyCode landKey = KeyCode.LeftShift;
    public KeyCode takeOffKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode runningKey = KeyCode.LeftShift;
    public KeyCode resetKey = KeyCode.R;

    public static GameManager instance;
    public List<string> facts = new List<string>();         //CHANGE THIS: make it its own class (or SO? or enum?). this word list contains true things about the current state of the game.
                                                            //"wingsUnlocked", for example is in this list when Nadira has acsess to her wings.

    [SerializeField] List<ItemNamePair> itemDIsplayNames = new List<ItemNamePair>();
    public static Action OnQuestTrigger;
    public string currentQuestTrigger;
    
    [Header("Dependencies")]
    public GameObject UI;       //not sure what this is for rn lol
    public PlayerStateCoordinator player;
    public ConversationCoordinator convoCoordinator;

    [Header("Game State")]
    public Location currentLocation;

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
        if (UI)UI.SetActive(true);
        OnQuestTrigger = null;
    }
    
    private void Update() {
        if (Input.GetKeyDown(resetKey)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void BeginTravelling()
    {
        EnterLocation(Location.traveling);
    }

    public string GetItemDisplayName(ItemName enumName) {
        for (int i = 0; i < itemDIsplayNames.Count; i++) {
            if (enumName == itemDIsplayNames[i].itemName) return itemDIsplayNames[i].name; 
        }
        return "";
    }

    void UnloadScene(int sceneIndex) {
        SceneManager.UnloadSceneAsync(sceneIndex);
    }

    public void LoadScene(int sceneIndex) {
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    public void EnterLocation(Location newLocation) {
        //print("newlocation: " + newLocation);
        QuestTrigger(newLocation.ToString());
        currentLocation = newLocation;
    }

    public void DisplayInternalCommentary(int lineNum) {
        string lineToDisplay = DialogueLineReader.instance.getLine(lineNum);

    }

    public float Circularize (float angle, bool simple = false)
    {
        if (simple) {
            if (angle > 360) {
                return (angle - 360);
            }
            if (angle < 360) {
                return (angle + 360);
            }
            else {
                return angle;
            }
        }

        if (angle > 360) {
            return (angle % 360);
        }
        if (angle < 360) {
            angle = -(Mathf.Abs(angle) % 360);
            return (angle + 360);
        }
        else {
            return angle;
        }
    }

    public void setChildTags(GameObject parent, string tag)
    {
        for (int i = 0; i < parent.transform.childCount; i++) {
            setChildTags(parent.transform.GetChild(i).gameObject, tag);
        }
        parent.tag = tag;
    }
}
