using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] float waitTime;

    [SerializeField] TextMeshProUGUI moneyText, moneyAdditionText;
    int currentDisplayMoney;
    int moneyDisplayTarget;

    [Header("Delivery Timer")]
    [SerializeField] GameObject deliveryTimerParent;
    [SerializeField] Slider deliveryTimerSlider;
    [SerializeField] TextMeshProUGUI DeliveryTimerText;

    private void Start()
    {
        moneyDisplayTarget = 0;
    }

    private void Update()
    {
        int displayMoney = Directory.player.GetMoneyDisplay();
        if (displayMoney != currentDisplayMoney) DisplayMoney(displayMoney);

        if (Directory.fMan.GetDeliveryTimePercent() != -1) DisplayRemainingDeliveryTime();
        else deliveryTimerParent.SetActive(false);
    }

    void DisplayRemainingDeliveryTime()
    {
        float timeLeft = Directory.fMan.GetDeliveryTime();
        float timePercent = Directory.fMan.GetDeliveryTimePercent();
        deliveryTimerParent.SetActive(true);
        deliveryTimerSlider.value = timePercent;
        string timeString = System.TimeSpan.FromSeconds(Mathf.RoundToInt(timeLeft)).ToString();
        timeString = timeString.Substring(3);
        DeliveryTimerText.text = timeString;
    }

    public void DisplayMoney(bool active)
    {
        StopAllCoroutines();
        currentDisplayMoney = Directory.player.GetMoney();
        moneyText.gameObject.SetActive(active);
    }
    void DisplayMoney(int newValue)
    {
        if (Directory.player.GetMoney() != moneyDisplayTarget) {
            moneyAdditionText.text = "+" + (Directory.player.GetMoney() - moneyDisplayTarget);
            moneyDisplayTarget = Directory.player.GetMoney();
            moneyAdditionText.gameObject.SetActive(false);
            moneyAdditionText.gameObject.SetActive(true);
        }

        StopAllCoroutines();
        currentDisplayMoney = newValue;
        moneyText.gameObject.SetActive(true);
        moneyText.text = newValue.ToString();
        StartCoroutine(HideMoney());
    }

    IEnumerator HideMoney()
    {
        yield return new WaitForSeconds(waitTime);
        moneyText.gameObject.SetActive(false);
        moneyDisplayTarget = currentDisplayMoney;
    }
}
