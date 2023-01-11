using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateCoordinator : MonoBehaviour
{
    public enum PossibleStates { walking, flying, climbing, falling}
    public PossibleStates currenState { get; private set; } = PossibleStates.walking;
    RigidbodyConstraints originalConstraints;

    //dependencies
    [SerializeField] CapsuleCollider uprightCollider;
    [SerializeField] CapsuleCollider FlightCollider;
    Animator animator;
    [HideInInspector] public Rigidbody rb;
    PlayerWalkingBehavior walkBehav;
    PlayerFlyingBehavior flyBehav;
    PlayerClimbingBehavior climbBehav;
    PlayerFallingBehavior fallingBehav;

    private void Awake() {
        walkBehav = GetComponent<PlayerWalkingBehavior>();
        flyBehav = GetComponent<PlayerFlyingBehavior>();
        climbBehav = GetComponent<PlayerClimbingBehavior>();
        fallingBehav = GetComponent<PlayerFallingBehavior>();
        rb = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();
        GameManager.instance.player = this;
    }

    private void Start() {
        originalConstraints = rb.constraints;
    }

    public void EnterWalkingState() {
        if (!walkBehav) { return; }
        DisableAllStates();
        currenState  = PossibleStates.walking;
        if (FlightCollider.enabled) {
            FlightToNonFlightTransition();
        }
        CallTrigger("land on ground");
        walkBehav.enabled = true;
    }

    public void EnterClimbingState(RaycastHit hit) {
        if (!climbBehav) { return; }
        DisableAllStates();
        currenState = PossibleStates.climbing;
        

        transform.localRotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
        transform.Rotate(0, -90, 0);
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);
        transform.position = hit.point + (transform.forward * -climbBehav.anchorOffset);
        rb.velocity = Vector3.zero;
        if (FlightCollider.enabled) {
            FlightToNonFlightTransition();
        }
        CallTrigger("start climbing");
        climbBehav.enabled = true;
    }

    public void EnterFlyingState() {
        if (!flyBehav) { return; }
        bool takeOffFromGround = false;
        if (walkBehav.enabled) takeOffFromGround = true;
        DisableAllStates();
        currenState = PossibleStates.flying;

        if (!takeOffFromGround) {
            CallTrigger("take off air");
            NonFlightToFlightTransition();
            flyBehav.enabled = true;
            return;
        }
        TakeOffFromGround();
    }

    void TakeOffFromGround() {
        transform.localEulerAngles += Vector3.up * animator.transform.localEulerAngles.y;
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);

        CallTrigger("take off ground");
        StartCoroutine(DelayedTakeOffFromGround());
    }

    IEnumerator DelayedTakeOffFromGround() {
        yield return new WaitForSeconds(flyBehav.takeOffTime);

        rb.AddForce(new Vector3(0, 1.8f, 1) * flyBehav.takeOffSpeed, ForceMode.VelocityChange);
        flyBehav.flySpeed = flyBehav.takeOffSpeed;
        NonFlightToFlightTransition();
        flyBehav.enabled = true;
    }

    public void EnterFallingState() { 
        if (!fallingBehav) { return; }
        DisableAllStates();
        currenState = PossibleStates.falling;

        CallTrigger("start falling");
        fallingBehav.enabled = true;
    }

    void DisableAllStates() {
        if (walkBehav) walkBehav.enabled = false;
        if (climbBehav) climbBehav.enabled = false;
        if (flyBehav) flyBehav.enabled = false;
        if (fallingBehav) fallingBehav.enabled = false;
    }

    void FlightToNonFlightTransition() {
        transform.localEulerAngles = Vector3.up * transform.localEulerAngles.y;
        rb.constraints = originalConstraints;
        uprightCollider.enabled = true;
        FlightCollider.enabled = false;
    }

    void NonFlightToFlightTransition() {
        rb.constraints = RigidbodyConstraints.None;
        uprightCollider.enabled = false;
        FlightCollider.enabled = true;
    }

    private void Update() {
        animator.SetFloat("abs hori vel", Mathf.Abs(rb.velocity.x + rb.velocity.z + (AnyInput()? 1 : 0) ));
        animator.SetFloat("vert vel", rb.velocity.y);
        if (walkBehav) {
            animator.SetBool("grounded", walkBehav.grounded);
            animator.SetBool("walking", AnyInput() && !Input.GetKey(GameManager.instance.runningKey));
            animator.SetBool("running", AnyInput() && Input.GetKey(GameManager.instance.runningKey));
        }
        if (climbBehav) animator.SetBool("climbing left", climbBehav.facingLeft);
    }

    public void CallTrigger(string trigger) {
        animator.SetTrigger(trigger);
    }

    bool AnyInput() {
        GameManager manager = GameManager.instance;
        bool f = Input.GetKey(manager.forwardKey);
        bool b = Input.GetKey(manager.backKey);
        bool l = Input.GetKey(manager.leftKey);
        bool r = Input.GetKey(manager.rightKey);

        return (f || b || l || r);
    }
}
