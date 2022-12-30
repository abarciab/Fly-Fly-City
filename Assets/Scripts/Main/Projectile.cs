using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public List<int> victimLayers = new List<int>();
    public float lifeTime = 10;
    public float damage = 5;

    private void OnTriggerEnter(Collider other) {
        if (victimLayers.Contains(other.gameObject.layer)) {
            other.gameObject.GetComponent<EntityInfo>().Damage(damage);
        }

        if (other.gameObject.layer != 7) {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) {
            Destroy(gameObject);
        }
    }
}
