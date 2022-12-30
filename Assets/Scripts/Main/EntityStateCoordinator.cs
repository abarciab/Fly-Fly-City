using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a generic script that every character has - it requires a 'brain' script (different entities have different brains)
public class EntityStateCoordinator : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum EntityType { pedestrian, batonAgent, gunAgent, sniperAgent}
