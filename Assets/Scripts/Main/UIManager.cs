using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Flightspeed")]
    [SerializeField] GameObject flightSpeedMenu;
    [SerializeField] GameObject flightSpeedPrompt;

    [Header("Map")]
    [SerializeField] GameObject map;

    [Header("Misc")]
    public GameObject skillTree;
    [SerializeField] StatsDisplay stats;

    public bool busy {  get { return skillTree.activeInHierarchy || flightSpeedMenu.activeInHierarchy || map.activeInHierarchy;  } }

    private void Update()
    {
        flightSpeedPrompt.SetActive(!busy && Directory.gMan.insideLocation && !Directory.fMan.onDelivery);
    }

    public void OpenSkillTree()
    {
        stats.DisplayMoney(true);
        skillTree.SetActive(true);
        Directory.gMan.player.SetPaused(true);
        Directory.cam.enabled = false;
        SetMouseState(true);
    }

    public void CloseSkillTree()
    {
        stats.DisplayMoney(false);
        skillTree.SetActive(false);
        Directory.gMan.player.SetPaused(false);
        Directory.cam.enabled = true;
        SetMouseState(false);
    }

    void SetMouseState(bool free)
    {
        Cursor.visible = free;
        Cursor.lockState = free ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
