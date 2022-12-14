using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthExpFeatherDisplay : MonoBehaviour
{
    [Header("references")]
    public TextMeshProUGUI level;
    public RectTransform HPBarTransform;
    public RectTransform XPBarTransform;
    public Image featherImage1;
    public Image featherImage2;
    public Image featherImage3;
    private PlayerController playerScript;
    public Color flappedFeatherColor = new Color32(0xCA, 0xBB, 0x90, 0xFF);

    [Header("sprites")]
    public Sprite feather_5;
    public Sprite feather_4;
    public Sprite feather_3;
    public Sprite feather_2;
    public Sprite feather_1;
    public Sprite feather_0;

    private void Start() {
        playerScript = GameManager.instance.player.GetComponent<PlayerController>();
    }

    void Update()
    {
        HPBarTransform.localScale = new Vector3(playerScript.health / playerScript.maxHealth, 1, 1);
        XPBarTransform.localScale = new Vector3(playerScript.xp / playerScript.xpToNextLevel, 1, 1);

        float totalFeathers = playerScript.totalFeathers;
        featherImage1.sprite = feather_5;
        featherImage2.sprite = feather_5;
        featherImage3.sprite = feather_5;

        if (totalFeathers > 0) {
            Image partialFeatherImage = featherImage3;
            if (playerScript.totalFeathers <= 2) {
                featherImage3.sprite = feather_0;
                partialFeatherImage = featherImage2;
            }
            if (playerScript.totalFeathers <= 1) {
                featherImage2.sprite = feather_0;
                partialFeatherImage = featherImage1;
            }
            float partialFeather = totalFeathers % 1;

            if (partialFeather > 0.8f || partialFeather == 0) {
                partialFeatherImage.sprite = feather_5;
            }
            else if (partialFeather > 0.6f) {
                partialFeatherImage.sprite = feather_4;
            }
            else if (partialFeather > 0.4f) {
                partialFeatherImage.sprite = feather_3;
            }
            else if (partialFeather > 0.2f) {
                partialFeatherImage.sprite = feather_2;
            }
            else if (partialFeather > 0.1) {
                partialFeatherImage.sprite = feather_1;
            }
            else {
                print("partialFeather: " + partialFeather);
                partialFeatherImage.sprite = feather_0;
            }
        }
        else {
            featherImage1.sprite = feather_0;
            featherImage2.sprite = feather_0;
            featherImage3.sprite = feather_0;
        }

        int remainingFlaps = playerScript.remainingFlaps;

        featherImage1.color = Color.white;
        featherImage2.color = Color.white;
        featherImage3.color = Color.white;

        if (remainingFlaps != Mathf.Floor(totalFeathers)) {
            if (remainingFlaps <= 2) {
                featherImage3.color = flappedFeatherColor;
            }
            if (remainingFlaps <= 1) {
                featherImage2.color = flappedFeatherColor;
            }
            if (remainingFlaps == 0) {
                featherImage1.color = flappedFeatherColor;
            }
        }        
    }
}
