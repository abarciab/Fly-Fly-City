using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] int money;
    [SerializeField] float MoneyLerpTime = 1.5f;
    float moneyDisplayValue;

    private void Start()
    {
        var startMoney = money;
        money = 0;
        AddMoney(startMoney);
    }

    public void AddMoney(int extra, bool animate = true)
    {
        StopAllCoroutines();
        moneyDisplayValue = money;
        money += extra;
        if (animate) StartCoroutine(ChangeDisplayValue());
        else moneyDisplayValue = money;
    }

    IEnumerator ChangeDisplayValue()
    {
        float timePassed = 0;
        float start = moneyDisplayValue;
        while (timePassed < MoneyLerpTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            moneyDisplayValue = Mathf.Lerp(start, money, timePassed / MoneyLerpTime);
        }
        moneyDisplayValue = money;
    }

    public int GetMoneyDisplay()
    {
        return Mathf.RoundToInt(moneyDisplayValue);
    }

    public int GetMoney()
    {
        return money;
    }
}
