using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Convo")]
public class Conversation : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        [System.Serializable]
        public class Metadata
        {
            public Speaker speaker = Speaker.None;
            public int index = -1;
            public int nextIndex = -1;
            public int priority = -1;
            public List<Fact> requiredFacts = new List<Fact>();
            public List<Fact> factEffects = new List<Fact>();
            public bool complete;
            public int ID = -1;
            public bool speakerLeft;

            public Metadata() {
                index = -1;
            }
        }

        public string line = "";
        public Metadata metadata = new Metadata();
    }

    [System.Serializable]
    public class Condition
    {
        public List<Fact> requiredFacts = new List<Fact>();
    }

    public List<Line> lines = new List<Line>();
    public Condition preCondition = new Condition();

    public Line getLineByID(int ID) {
        for (int i = 0; i < lines.Count; i++) {
            if (lines[i].metadata.ID == ID) return lines[i];
        }
        return null;
    }

    public void UpdateOrAddLine(Line updatedLine) {
        var ID = updatedLine.metadata.ID;
        for (int i = 0; i < lines.Count; i++) {
            if (lines[i].metadata.ID == ID) {
                lines[i] = updatedLine;
                return;
            }
        }
        lines.Add(updatedLine);
    }

    public void DeleteLine(int ID) {
        for (int i = 0; i < lines.Count; i++) {
            if (lines[i].metadata.ID == ID) lines.RemoveAt(i);
        }
    }

    public void ResetConversation() {
        lines = new List<Line>();
        preCondition = new Condition();
    }
}

public enum Speaker {None, Nadira, Mika, Harlan, Noah, Kenny}
