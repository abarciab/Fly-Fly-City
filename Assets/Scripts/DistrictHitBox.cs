using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictHitBox : MonoBehaviour
{
    [SerializeField] int districtNum;

    private void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckEnter();
    }

    void CheckEnter()
    {
        if (Directory.gMan.currentDistrict != districtNum) Directory.gMan.EnterNewDistrict(districtNum);
    }
}
