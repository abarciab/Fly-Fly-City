using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoadBuilder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down);
    }
}
