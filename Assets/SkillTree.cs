using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum Skill { none, extraFlap }

[System.Serializable]
public class SkillTreeButtonData
{
    public string skillName;
    public Button button;
    public Image costBacking;
    public int cost;
    public Skill skill;
    public float amount;
    [Space()]
    public bool singleUse = true;
}
public class SkillTree : MonoBehaviour
{
    [SerializeField] List<SkillTreeButtonData> buttons = new List<SkillTreeButtonData>();
    [SerializeField] Color activeColor, inactiveColor, unaffordableColor;

    private void Update()
    {
        int money = Directory.player.GetMoney();
        foreach (var b in buttons) UpdateButton(b, money);
    }

    void UpdateButton(SkillTreeButtonData data, int money)
    {
        bool active = data.cost < money && data.cost >= 0;
        data.button.GetComponent<Image>().color = active ? activeColor : inactiveColor;
        data.costBacking.color = active ? activeColor : unaffordableColor;

        data.costBacking.gameObject.SetActive(data.cost != - 1);
        data.costBacking.GetComponentInChildren<TextMeshProUGUI>().text = data.cost.ToString();
        data.button.enabled = active;
    }

    public void UpgradeSkill(GameObject obj)
    {
        foreach (var b in buttons) if (b.button.gameObject == obj) PurchaseSkill(b);
    } 

    void PurchaseSkill(SkillTreeButtonData data)
    {
        Directory.player.AddMoney(-data.cost);
        if (data.skill == Skill.extraFlap) Directory.gMan.player.AddFlap(Mathf.RoundToInt(data.amount));
        if (data.singleUse) data.cost = -1;
    }
}
