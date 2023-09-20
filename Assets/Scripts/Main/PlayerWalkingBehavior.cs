using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static Constants;

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
    [HideInInspector] public bool grounded = true;

    [Header("Stepping")]
    [SerializeField] float stepUpForce;
    [SerializeField] CollisionChecker stepChecker, airChecker;

    bool paused;
    float timeSinceJumpButton = 1000;
    

    PlayerStateCoordinator stateController;
    GameManager manager;
    Transform model;
    Rigidbody rb;

    public void SetPaused(bool _paused)
    {
        paused = _paused;
    }

    private void FixedUpdate()
    {
        stateController.allContactPoints.Clear();
    }


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
        if (paused) {
            rb.velocity = Vector3.zero;
            return;
        }

        if (!stateController) {
            Start();
            return;
        }

        ProcessMovement();
        ApplyGravity();
        if (ShouldTakeOff()) TakeOff();
        grounded = IsGrounded();
        stateController.grounded = grounded;
    }
    


    bool ShouldTakeOff() {
        return grounded && Input.GetKeyDown(takeOffKey) && !GameManager.instance.insideLocation;
    }

    void TakeOff() {
        stateController.EnterFlyingState();
    }

    bool IsGrounded() {        
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
        bool running = Input.GetKey(runningKey) && !GameManager.instance.insideLocation;
        rb.velocity = Vector3.Lerp(rb.velocity, forceDir * (running ? runSpeed : walkSpeed), turnSpeed);

        var forwardSpeed = model.forward * Vector3.Dot(model.forward, rb.velocity);
        if (stepChecker.colliding && !airChecker.colliding && forwardSpeed.magnitude > 0) {
            rb.AddForce(Vector3.up * stepUpForce);
        }
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
        bool f = Input.GetKey(forwardKey);
        bool b = Input.GetKey(backKey);
        bool l = Input.GetKey(leftKey);
        bool r = Input.GetKey(rightKey);

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
        if (Input.GetKeyDown(jumpKey)) {
            timeSinceJumpButton = 0;
        }
        if (grounded && timeSinceJumpButton <= jumpBufferMax) 
            return true;
        else
            return false;
    }

    void Jump() {
        stateController.CallTrigger("jump");
        //stateControllergrounded = false;
        timeSinceJumpButton = jumpBufferMax + 1;
        rb.AddForce(Vector3.up * (jumpForce * 10), ForceMode.Impulse);
    }

    private void OnDrawGizmos() {
        if (showGroundCheck) Gizmos.DrawWireSphere(transform.position + groundedCheckOffset, groundedCheckRadius);
    }
}
