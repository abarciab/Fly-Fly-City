using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionsShowHide : MonoBehaviour
{
    public GameObject otherOne;
    TextMeshProUGUI thisOne;

    private void Start()
    {
        thisOne = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            otherOne.SetActive(!otherOne.activeInHierarchy);
            thisOne.enabled = !thisOne.enabled;
        }
    }
}
