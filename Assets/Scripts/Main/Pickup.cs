using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType {scrap, credits, other}

    public PickupType type;

    public float value;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 0.1f);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == Directory.gMan.player) {
            switch (type) {
                case PickupType.scrap:
                    Directory.gMan.player.GetComponent<PlayerController>().totalFeathers = Mathf.Min(3, Directory.gMan.player.GetComponent<PlayerController>().totalFeathers + value);
                    break;
                case PickupType.credits:
                    break;
                case PickupType.other:
                    break;
            }
            Destroy(gameObject);

        }
    }
}
