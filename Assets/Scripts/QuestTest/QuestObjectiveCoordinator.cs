using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestObjectiveCoordinator : MonoBehaviour
{
    
    public Sprite checkedBox, uncheckedBox;

    [Header("Dependencies")]
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] Image checkBox;
    [SerializeField] Slider slider;

    public void Setup(Quest.Objective objective) {
        Text.text = formatText(objective.hudDisplayText);

    }

    string formatText(string displayText) {
        return displayText;
    }
}
