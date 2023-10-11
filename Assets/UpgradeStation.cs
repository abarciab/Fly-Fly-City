using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStation : MonoBehaviour
{
    [SerializeField] float interactRange;
    [SerializeField] Vector3 interactPos;
    [SerializeField] GameObject prompt;

    private void Update()
    {
        if (Directory.uiMan.busy) {
            prompt.SetActive(false);
            return;
        }

        var dist = Vector3.Distance(transform.position, Directory.player.transform.position);
        prompt.SetActive(dist < interactRange);
        if (prompt.activeInHierarchy && Input.GetKeyDown(KeyCode.E)) OpenMenu();
    }

    void OpenMenu()
    {
        Directory.uiMan.OpenSkillTree();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(interactPos), interactRange);
    }
}
