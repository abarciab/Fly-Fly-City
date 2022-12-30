using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TestScript : MonoBehaviour
{
    [SerializeField] float angle;

    private void Update() {
        Quaternion quatRot = Quaternion.AngleAxis(angle, Vector3.left);
        // that's a local direction vector that points in forward direction but also 45 upwards.
        Vector3 localDirection = quatRot * Vector3.forward;
        localDirection.x = localDirection.y;
        localDirection.y = 0;

        // If you need the direction in world space you need to transform it.
        Vector3 wDirection = transform.TransformDirection(localDirection);
        print("localSpace: " + localDirection + ", worldSpace: " + wDirection);
        Debug.DrawRay(transform.position, wDirection * 10, Color.magenta);
    }
}
