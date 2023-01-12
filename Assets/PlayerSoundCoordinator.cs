using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundCoordinator : MonoBehaviour
{
    [SerializeField] int footstepID;
    [SerializeField] bool testplay;
    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioSource flyWindSource;
    [SerializeField] float maxFlySpeed = 40;
    PlayerFlyingBehavior flyScript;

    [Range(0, 2), SerializeField]
    float baseWindVolume;

    private void Start()
    {
        AnimationEventCoordinator.NadiraFootstep += PlayFootstepSound;
        flyScript = GetComponent<PlayerFlyingBehavior>();
    }

    private void Update()
    {
        if (flyScript) PlayWindSound();
        else if (flyWindSource.isPlaying) flyWindSource.Pause();
    }

    void PlayWindSound() {
        if (!flyScript.enabled) {
            flyWindSource.volume = Mathf.Lerp(flyWindSource.volume, 0, 0.04f);
            if (flyWindSource.volume <= 0.001f) flyWindSource.Pause();
            return;
        }
        if (!flyWindSource.isPlaying)
            {
            flyWindSource.Play();
            flyWindSource.volume = 0;
        }
        float targetVol = flyScript.flySpeed / maxFlySpeed * baseWindVolume;
        flyWindSource.volume = Mathf.Lerp(flyWindSource.volume, targetVol, 0.05f);
        //print("flyspeed: " + flyScript.flySpeed);
    }

    void PlayFootstepSound() {
        AudioManager.instance.PlaySound(footstepID, footstepSource);
    }
}
