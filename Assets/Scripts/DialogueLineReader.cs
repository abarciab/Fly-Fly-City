using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueLineReader : MonoBehaviour
{
    public static DialogueLineReader instance;

    public TextAsset textAssetData;
    public int numLanguages = 1;

    string[] lines;

    private void Awake() {
        instance = this;
    }

    void ReadCSV() {
        string[] data = textAssetData.text.Split(new string[] {",", "\n"}, StringSplitOptions.None);
        lines = data;
    }

    public string getLine(int lineNum) {
        return lines[lineNum];
    }

}
