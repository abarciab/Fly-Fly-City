using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public enum PossiblePlayerStates { walking, flying, climbing, falling }

public class PlayerStateCoordinator : MonoBehaviour
{
    public PossiblePlayerStates currenState { get; private set; } = PossiblePlayerStates.walking;
    RigidbodyConstraints originalConstraints;
    [SerializeField] bool debug;
    
    [SerializeField] CapsuleCollider uprightCollider;
    [SerializeField] CapsuleCollider FlightCollider;
    [SerializeField] Animator anim;
    [SerializeField] Transform model;


    [HideInInspector] public Rigidbody rb;
    PlayerWalkingBehavior walkBehav;
    PlayerFlyingBehavior flyBehav;
    PlayerClimbingBehavior climbBehav;
    PlayerFallingBehavior fallingBehav;

    [HideInInspector] public bool grounded;

    [HideInInspector] public List<ContactPoint> allContactPoints = new List<ContactPoint>();
    [HideInInspector] public ContactPoint groundPoint;
    bool paused;

    [Header("Camera")]
    [SerializeField] Vector3 headPos;
    [SerializeField] Vector3 camPos;

    Transform speaker;
    public void SetPaused(bool _paused)
    {
        paused = _paused;
        walkBehav.SetPaused(_paused);
    }

    public void AddFlap(int num)
    {
        flyBehav.AddFlap(num);
    }
    public Vector3 getHeadPos()
    {
        return transform.TransformPoint(headPos);
    }

    public Vector3 GetCamPos()
    {
        return transform.TransformPoint(camPos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        allContactPoints.AddRange(collision.contacts);
    }
    private void OnCollisionStay(Collision collision)
    {
        allContactPoints.AddRange(collision.contacts);
    }

    void FaceSpeaker()
    {
        FaceSpeaker(speaker);
    }

    public void FaceSpeaker(Transform speaker)
    {
        if (speaker == null) return;

        var pos = speaker.GetComponent<Speaker>().GetStandPos();
        pos.y = transform.position.y;
        transform.position = pos;

        model.transform.localEulerAngles = Vector3.zero;
        this.speaker = speaker;
        var rot = transform.localEulerAngles;
        transform.LookAt(speaker);
        rot.y = transform.localEulerAngles.y;
        transform.localEulerAngles = rot;
    }

    public void EndConversation()
    {
        walkBehav.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!walkBehav.enabled) allContactPoints.Clear();
    }

    public bool groundedCheck(out ContactPoint groundPoint, List<ContactPoint> cpList)
    {
        groundPoint = default;
        bool found = false;
        foreach (var point in cpList) {
            if (point.normal.y > 0.0001f && (!found || point.normal.y > groundPoint.normal.y)) {
                groundPoint = point;
                found = true;
            }
        }
        return found;
    }

    private void Awake() {
        walkBehav = GetComponent<PlayerWalkingBehavior>();
        flyBehav = GetComponent<PlayerFlyingBehavior>();
        climbBehav = GetComponent<PlayerClimbingBehavior>();
        fallingBehav = GetComponent<PlayerFallingBehavior>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        originalConstraints = rb.constraints;
    }

    public void EnterWalkingState() {
        if (!walkBehav) { return; }
        DisableAllStates();
        currenState  = PossiblePlayerStates.walking;
        if (FlightCollider.enabled) {
            FlightToNonFlightTransition();
        }
        CallTrigger("land on ground");
        walkBehav.enabled = true;
    }

    public void EnterClimbingState(RaycastHit hit) {
        if (!climbBehav) { return; }
        DisableAllStates();
        currenState = PossiblePlayerStates.climbing;
        

        transform.localRotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
        transform.Rotate(0, -90, 0);
        model.localEulerAngles = new Vector3(0, 0, 0);
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
        currenState = PossiblePlayerStates.flying;

        if (!takeOffFromGround) {
            CallTrigger("take off air");
            NonFlightToFlightTransition();
            flyBehav.enabled = true;
            return;
        }
        TakeOffFromGround();
    }

    void TakeOffFromGround() {
        transform.localEulerAngles += Vector3.up * model.localEulerAngles.y;
        model.localEulerAngles = new Vector3(0, 0, 0);

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
        EnterFlyingState();
        return;


        if (!fallingBehav) { return; }
        DisableAllStates();
        currenState = PossiblePlayerStates.falling;

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
        if (Directory.gMan.talking) {
            walkBehav.enabled = false;
            FaceSpeaker();
            return;
        }

        if (grounded && walkBehav.enabled) climbBehav.ResetStamina();

        anim.SetFloat("abs hori vel", Mathf.Abs(rb.velocity.x + rb.velocity.z + (AnyInput()? 1 : 0) ));
        anim.SetFloat("vert vel", rb.velocity.y);
        if (walkBehav) {
            anim.SetBool("grounded", grounded);
            bool running = Input.GetKey(runningKey) && !Directory.gMan.insideLocation;
            anim.SetBool("walking", AnyInput() && !running);
            anim.SetBool("running", AnyInput() && running);
        }
        if (climbBehav) anim.SetBool("climbing left", climbBehav.facingLeft);
    }

    public void CallTrigger(string trigger) {
        if (debug) print("triggerCalled: " + trigger);
        anim.SetTrigger(trigger);
    }

    bool AnyInput() {
        //GameManager manager = Directory.gMan;
        bool f = Input.GetKey(forwardKey);
        bool b = Input.GetKey(backKey);
        bool l = Input.GetKey(leftKey);
        bool r = Input.GetKey(rightKey);

        return (!paused && (f || b || l || r) );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(headPos), 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(camPos), 0.5f);
    }
}
