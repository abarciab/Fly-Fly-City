using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Speaker { None, Nadira, Mika, Harlan, Noah, Kenny, Lily }

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
    public class Effect
    {
        public enum Type { none, item, questTrigger};
        public Type type;

        [ConditionalEnumHide(nameof(type), (int)Type.item)]
        public Item items;

        [ConditionalEnumHide(nameof(type), (int)Type.questTrigger)]
        public int triggerID;
    }


    [System.Serializable]
    public class Condition
    {
        public List<Fact> requiredFacts = new List<Fact>();
    }

    public List<Line> lines = new List<Line>();
    public Condition preCondition = new Condition();
    public Effect completionEffect = new Effect();
    int index = -1;

    public void StartConversation()
    {
        index = -1;
    }
    public void EndConvo()
    {
        index = -1;
        if (completionEffect.type == Effect.Type.questTrigger) GameManager.instance.QuestTrigger(completionEffect.triggerID);
    }

    public Line GetNextLine()
    {
        index += 1;
        var alt = GetAltLine(index);
        if (alt != null) {
            return alt;
        }
        if (index == lines.Count) return null;
        return lines[index];
    }

    Line GetAltLine(int index)
    {
        //TODO: implement altlines
        return null;
    }


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


