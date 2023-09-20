using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryBriefCoord : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title, price, difficulty;

    public void Init(string titleText, int priceInt, int difficultyInt, string difficultyCharacter)
    {
        title.text = titleText;
        price.text = priceInt.ToString("C0");

        difficulty.text = "";
        for (int i = 0; i < difficultyInt; i++) {
            difficulty.text += difficultyCharacter;
        }

        gameObject.SetActive(true);
    }

}
