using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundCoordinator : MonoBehaviour
{
    [SerializeField] int footstepID;
    [SerializeField] bool testplay;
    [SerializeField] Sound flySound;
    [SerializeField] float maxFlySpeed = 40;
    PlayerFlyingBehavior flyScript;


    private void Start()
    {
        AnimationEventCoordinator.NadiraFootstep += PlayFootstepSound;
        flyScript = GetComponent<PlayerFlyingBehavior>();

        flySound = Instantiate(flySound);
        flySound.PlaySilent(transform);
    }

    private void Update()
    {
        if (flySound && flyScript.enabled) PlayWindSound();
        else flySound.PercentVolume(0, 0.1f);
    }

    void PlayWindSound() {
        float targetVol = flyScript.flySpeed / maxFlySpeed;
        flySound.PercentVolume(targetVol, 0.05f);
    }

    void PlayFootstepSound() {
        //AudioManager.instance.PlaySound(footstepID, footstepSource);
    }
}
