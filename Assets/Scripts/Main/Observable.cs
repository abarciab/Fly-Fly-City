using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable : MonoBehaviour
{
    [Space]
    public bool dialogueTrigger;

    [Space]
    public bool loadScene;

    [Space]
    public bool internalCommentary;
    public int[] lineNumber;
    public bool oneTime = true;

    public void TriggerIC() {
        if (oneTime) {
            internalCommentary = false;
        }
    }

}
