using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class goonEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;

    private void Start() {
        if (!agent) {
            agent = GetComponent<NavMeshAgent>();
        }
        target = GameManager.instance.player.gameObject;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            Ray worldPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(worldPoint, out var clickHit)) {
                agent.SetDestination(clickHit.point);
            }
        }
        if (NavMesh.SamplePosition(target.transform.position, out var hit, 3, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        }
    }
}
