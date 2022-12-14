using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Fact {
    public string fact;
}
public class GameManager : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode landKey = KeyCode.LeftShift;
    public KeyCode takeOffKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode resetKey = KeyCode.R;

    public static GameManager instance;
    public List<string> facts = new List<string>();         //CHANGE THIS: make it its own class (or SO? or enum?). this word list contains true things about the current state of the game.
                                                            //"wingsUnlocked", for example is in this list when Nadira has acsess to her wings.
    
    [Header("references")]
    public GameObject UI;       //not sure what this is a ref to lol
    public PlayerController player;

    private void Awake() {
        instance = this;
        UI.SetActive(true);
    }
    
    private void Update() {
        if (Input.GetKeyDown(resetKey)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void UnloadScene(int sceneIndex) {
        SceneManager.UnloadSceneAsync(sceneIndex);
    }

    public void LoadScene(int sceneIndex) {
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
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
