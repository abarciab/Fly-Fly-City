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
        Text.text = objective.hudDisplayText;
        if (objective.display == Quest.Objective.DisplayType.ProgressBar) {
            slider.gameObject.SetActive(true);
            slider.value = objective.points / objective.maxPoints;
            return;
        }
        else {
            checkBox.gameObject.SetActive(true);
            if (objective.completed) checkBox.sprite = checkedBox;
            else checkBox.sprite = uncheckedBox;
        }
        if (objective.completed) {
            DisplayAsCompleted();
        }
        else {
            DisplayAsUncompleted();
        }
    }


    void DisplayAsCompleted()
    {
        Text.color = new Color( 1, 1, 1, 0.6f);
        Text.fontStyle = FontStyles.Strikethrough | FontStyles.Italic;
        slider.gameObject.SetActive(false);
    }

    void DisplayAsUncompleted()
    {
        Text.color = Color.white;
        Text.fontStyle = FontStyles.Normal;
    }
}
