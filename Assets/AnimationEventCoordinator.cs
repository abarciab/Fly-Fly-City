using System;
using UnityEngine;

public class AnimationEventCoordinator : MonoBehaviour
{
    public static Action NadiraFootstep;

    public void NadiraFootStep(){
        if (NadiraFootstep != null) NadiraFootstep.Invoke();
    }
}
