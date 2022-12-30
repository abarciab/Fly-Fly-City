using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCoordinator : MonoBehaviour
{
    [SerializeField] Location location;

    public void EnterLocation() {
        GameManager.instance.EnterLocation(location);
    }
}
