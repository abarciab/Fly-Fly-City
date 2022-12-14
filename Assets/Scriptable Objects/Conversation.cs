using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Convo")]
public class Conversation : ScriptableObject
{
    public List<string> activeConditions = new List<string>();     //add strings to this list to trigger alt lines
    
    [System.Serializable]
    public class Line
    {
        public string speaker;
        public string line;
        public List<AltLine> altLLines = new List<AltLine>();
    }

    [System.Serializable]
    public class AltLine 
    {
        public string condition;
        public string line;
        public bool endHere = false;
    }

    [System.Serializable]
    public class Speaker
    {
        public string name;
        public Sprite portrait;
        public Color speakerColor;
    }

    public List<Speaker> speakers;
    public List<Line> lines;
}
