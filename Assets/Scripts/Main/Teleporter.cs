using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Teleporter nextTeleporter;
    public Vector3 outputPoint;
    public bool showPoint;
    public float cooldown;
    float countdown;

    private void Start()
    {
        countdown = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (countdown > 0 || nextTeleporter == null) {
            print("countdown: " + countdown);
            return;
        }
        if (other.tag == "Player") {
            print("teleport");
            nextTeleporter.ResetCountdown();
            ResetCountdown();
            other.transform.position = nextTeleporter.outputPoint + nextTeleporter.transform.position;
        }
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
    }

    public void ResetCountdown()
    {
        countdown = cooldown;
    }

    private void OnDrawGizmosSelected()
    {
        if (showPoint) {
            Gizmos.DrawWireSphere(transform.position + outputPoint, 0.5f);
        }
    }

}
