using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


/*[System.Serializable]
public class ConversationSegment
{
    //public Fact startFact
    public List<LineData> lines = new List<LineData>();
}*/

[System.Serializable]
public class LineData
{
    public string line;
    public bool playerSpeak;

    public LineData(string line, bool playerSpeak)
    {
        this.line = line;
        this.playerSpeak = playerSpeak;
    }

    public LineData() { }
}

[CreateAssetMenu(fileName = "New Conversation")]
public class Conversation : ScriptableObject
{
    [SerializeField] TextAsset textFile;
    [SerializeField] bool readText;

    public List<LineData> lines;
    public int lineIndex = 0;
    bool lastLineSpokenByPlayer;

    private void OnValidate()
    {
       if (readText) {
            readText = false;
            ProcessTextFile();
       }
    }

    void ProcessTextFile()
    {
        if (textFile == null) return;

        lines.Clear();
        string[] _lines = Regex.Split(textFile.text, "\r|\n");
        foreach (var l in _lines) ProcessLine(l);
    }

    void ProcessLine(string line)
    {
        if (line.Length == 0) return;

        string[] parts = Regex.Split(line, "::");

        var newLine = new LineData("", lastLineSpokenByPlayer);
        if (parts.Length == 1) newLine.line = parts[0];
        else {
            newLine.line = parts[1];
            newLine.playerSpeak = parts[0] == "PLAYER";
        }
        lines.Add(newLine);
    }

    public void StartConvo()
    {
        lineIndex = 0;
    }

    public LineData GetNextLine()
    {
        var currentLine = lineIndex < lines.Count ? lines[lineIndex] : null;
        lineIndex += 1;
        return currentLine;
    }
}


