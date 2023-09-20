using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public bool colliding;
    List<Collider> colliders = new List<Collider>();

    private void LateUpdate()
    {
        //colliders.Clear();
    }

    void Update()
    {

        colliding = colliders.Count > 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!colliders.Contains(other)) colliders.Add(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other)) colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliders.Contains(other)) colliders.Remove(other);
    }



}
