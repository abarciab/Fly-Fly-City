using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] bool yOnly = true;
    Transform player;
    

    void Start()
    {
        player = Directory.cam.transform;
    }

    void Update()
    {
        Vector3 rot = transform.localEulerAngles;
        transform.LookAt(player);
        if (!yOnly) return;
        
        rot.y = transform.localEulerAngles.y;
        transform.localEulerAngles = rot; 
    }
}