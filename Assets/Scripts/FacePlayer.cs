using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] bool yOnly = true, faceCam = true;
    Transform player;
    

    void Start()
    {
        player = faceCam ? Directory.cam.transform : Directory.player.transform;
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
