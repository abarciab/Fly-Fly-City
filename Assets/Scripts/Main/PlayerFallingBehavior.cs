using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingBehavior : MonoBehaviour
{
    [SerializeField] float gravity;

    //dependencies
    PlayerStateCoordinator stateController;
    PlayerWalkingBehavior walkBehav;
    Rigidbody rb;

    private void Start() {
        stateController = GetComponent<PlayerStateCoordinator>();
        walkBehav = GetComponent<PlayerWalkingBehavior>();
    }

    void Update()
    {
        bool grounded = walkBehav.IsGrounded();
        if (grounded) {
            stateController.EnterWalkingState();
            return;
        }
        if (Input.GetKeyDown(GameManager.instance.takeOffKey)) {
            stateController.EnterFlyingState();
            return;
        }
        if (!rb) rb = stateController.rb;
        rb.AddForce(Vector3.down * gravity * Time.deltaTime * (rb.mass * rb.mass));
    }
}
