using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingBehavior : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] float walkSpeed = 0.5f;
    [SerializeField] float runSpeed = 1f;
    [SerializeField] float turnSpeed = 0.2f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10;
    [SerializeField] float jumpUpGravity = 9;
    [SerializeField] float jumpDownGravity = 9;
    [SerializeField] float jumpBufferMax = 0.5f;

    [Header("Ground Check")]
    [SerializeField] int groundLayer;
    [SerializeField] Vector3 groundedCheckOffset = new Vector3();
    [SerializeField] float groundedCheckRadius = 0.01f;
    [SerializeField] bool showGroundCheck;
    [HideInInspector] public bool grounded { get; private set; } = true;
    
    float timeSinceJumpButton = 1000;
    
    //dependencies
    PlayerStateCoordinator stateController;
    GameManager manager;
    Transform model;
    Rigidbody rb;

    void Start() {
        stateController = GetComponent<PlayerStateCoordinator>();
        if (!stateController) {
            enabled = false;
            return;
        }
        manager = GameManager.instance;
        rb = stateController.rb;
        model = transform.GetChild(0);
    }

    void Update()
    {
        if (!stateController) {
            Start();
            return;
        }
        grounded = IsGrounded();
        ProcessMovement();
        ApplyGravity();
        if (ShouldTakeOff()) TakeOff();
    }
    


    bool ShouldTakeOff() {
        return grounded && Input.GetKeyDown(manager.takeOffKey);
    }

    void TakeOff() {
        stateController.EnterFlyingState();
    }

    public bool IsGrounded() {
        Collider[] colliders = Physics.OverlapSphere(transform.position + groundedCheckOffset, groundedCheckRadius);
        foreach (Collider collider in colliders) {
            if (collider.gameObject != gameObject && collider.gameObject.layer == groundLayer) {
                return true;
            }
        }
        return false;
    }

    void ProcessMovement() {
        Walk();
        ProcessJump();
    }

    void Walk() {
        int inputAngle = GetInputAngle();
        if (inputAngle != -1) {
            AlignParentToCamera();
            LerpObjToAngle(model, inputAngle, turnSpeed);
        }
        SetRbVel(inputAngle);
    }

    void SetRbVel(float angle) {
        if (angle == -1) {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, turnSpeed);
            return;
        }
        Vector3 forceDir = GetWorldVectorFromAngle(angle);
        bool running = Input.GetKey(manager.runningKey);
        rb.velocity = Vector3.Lerp(rb.velocity, forceDir * (running ? runSpeed : walkSpeed), turnSpeed);
    }

    Vector3 GetWorldVectorFromAngle(float angle) {
        Quaternion quatRot = Quaternion.AngleAxis(angle, Vector3.left);
        Vector3 localDirection = quatRot * Vector3.forward;
        localDirection.x = localDirection.y;
        localDirection.y = 0;
        return transform.TransformDirection(localDirection);
    }

    void AlignParentToCamera() {
        Vector3 camRot = Camera.main.transform.parent.localEulerAngles;
        LerpObjToAngle(transform, camRot.y);
    }

    void LerpObjToAngle(Transform obj, float targetRot, float lerpSpeed = 0.025f) {
        Vector3 angles = obj.localEulerAngles;
        float altTarget = (targetRot > 180) ? targetRot - 360 : targetRot + 360;
        bool pickAltPath = Mathf.Abs(targetRot - angles.y) > 180;
        angles.y = pickAltPath ? altTarget : targetRot;

        obj.localEulerAngles = Vector3.Lerp(obj.localEulerAngles, angles, lerpSpeed);
    }

    int GetInputAngle() {
        int angle = -1;
        bool f = Input.GetKey(manager.forwardKey);
        bool b = Input.GetKey(manager.backKey);
        bool l = Input.GetKey(manager.leftKey);
        bool r = Input.GetKey(manager.rightKey);

        if (f && r) angle = 45;
        else if (f && l) angle = 315;
        else if (f) angle = 0;
        else if (b && r) angle = 135;
        else if (b && l) angle = 225;
        else if (b) angle = 180;
        else if (r) angle = 90;
        else if (l) angle = 270;
        
        return angle;
    }

    void ProcessJump() {
        if (ShouldJump()) {
            Jump();
        }
    }

    void ApplyGravity() {
        //if (grounded) return;

        if (rb.velocity.y > 0) {
            rb.AddForce(Vector3.down * jumpUpGravity * (rb.mass * rb.mass));
        }
        else if (rb.velocity.y <= 0) {
            rb.AddForce(Vector3.down * jumpDownGravity * (rb.mass * rb.mass));
        }
    }

    bool ShouldJump() {
        timeSinceJumpButton += Time.deltaTime;
        if (Input.GetKeyDown(manager.jumpKey)) {
            timeSinceJumpButton = 0;
        }
        if (grounded && timeSinceJumpButton <= jumpBufferMax) 
            return true;
        else
            return false;
    }

    void Jump() {
        stateController.CallTrigger("jump");
        grounded = false;
        timeSinceJumpButton = jumpBufferMax + 1;
        rb.AddForce(Vector3.up * (jumpForce * 10), ForceMode.Impulse);
    }

    private void OnDrawGizmos() {
        if (showGroundCheck) { 
            Gizmos.DrawWireSphere(transform.position + groundedCheckOffset, groundedCheckRadius);
        }
    }
}
