using System;
using UnityEngine;

public class AnimationEventCoordinator : MonoBehaviour
{
    public static Action NadiraFootstep;

    public void PlayNadiraFootstep(){
        //if (NadiraFootstep != null) NadiraFootstep.Invoke();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
