using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Constants;

public class PlayerController : MonoBehaviour {
    [System.Serializable]
    public class Stats      //done
    {
        public float maxHealth;
        public float health;
        public float xp;
        public float xpToNextLevel;
        public int level;
    }

    [System.Serializable]
    public class Walking {      //done   
        public float walkSpeed = 0.5f;
        public float strafeSpeed = 50;
        [Range(0, 0.2f)]
        public float frictionFactor = 0.08f;
        public float turnSpeed = 0.2f;
        public CapsuleCollider normalRB;
    }

    [System.Serializable]
    public class Jumping {      //done
        public float jumpForce = 10;
        public float jumpingGravity = 9;
        public float fallingGravity = 20;
        public float jumpBufferMax = 0.5f;
        public bool grounded = true;
        public int groundLayer = 0;
        public Vector3 groundedCheckOffset = new Vector3();
        public float groundedCheckRadius = 0.01f;
    }

    

    [SerializeField] Stats stats;
    [HideInInspector]public float maxHealth;
    [HideInInspector] public float health;
    [HideInInspector] public float xp;
    [HideInInspector] public float xpToNextLevel;
    [HideInInspector] public int level;

    [SerializeField] Walking walking;
    [HideInInspector] public float walkSpeed = 0.5f;
    [HideInInspector] CapsuleCollider normalCC;
    [HideInInspector]public float strafeSpeed = 50;
    [Range(0, 0.2f)]
    [HideInInspector]public float frictionFactor = 0.08f;
    [HideInInspector]public float turnSpeed = 0.2f;
    //public float slopeMax = 45;     //TODO: implement this lmao - maybe not?

    [SerializeField] Jumping jumping;
    [HideInInspector] public float jumpForce = 10;
    [HideInInspector] public float jumpingGravity = 9;
    [HideInInspector] public float fallingGravity = 20;
    [HideInInspector] public float jumpBufferMax = 0.5f;
    [HideInInspector] public bool grounded = true;
    [HideInInspector] public int groundLayer = 0;
    [HideInInspector] public Vector3 groundedCheckOffset = new Vector3();
    [HideInInspector] public float groundedCheckRadius = 0.01f;
    float timeSinceJumpButton = 10;

    [Header("Flying")]
    public float mousePitchSpeed = 1;
    public float mouseRollSpeed = 1;
    public float levelSpeed = 0.5f;         //how quickly the wings level out when banking left or right
    public float bankSpeed = 1;             //CHANGE LATER: speed that she rotates left and right when banking. TODO: make the speed depend on the angle of the wings + speed
    public float rollSpeed = 1.5f;          //how fast she rolls left and right   
    public float levelCutoff = 0.05f;       //how close does the y altitude of the two wingtips have to be for baking to stop
    public float pitchChangeRate = 1.5f;    //CHANGE LATER: how fast does pitch change. TODO: make this controlled by mouse
    public float flyGravity = 9;            //gravity, applied constantly when flying
    public float downBase = 0.5f;           //multiplier when flying down
    public float upBase = 0.5f;             //multiplier when flying up     
    public float takeOffSpeed = 15;         //CHANGE LATER: initial speed when taking off. TODO: make taking off more dynamic and feel better
    public float totalFeathers = 3;         //number of flaps
    public float flapSpeed = 15;            //force of a single flap
    public float freefallMod = 2;           //I think this is supposed to make falling faster when you were flying? TODO: fix this       
    float flySpeed;                         //the rb.velicty is updated by this amount each frame that the player is flying
    public float currentPitch;                     //the current pitch, clamped from 0-180, because loops aren't allowed. 0 is striaght up, and 180 is stright down.
    int extraFlaps = 0;                     //CHANGE. I think this is a lame work around? not sure
    bool falling = false;
    RigidbodyConstraints originalConstraints;
    float takingOff;
    public CapsuleCollider flyingCC;

    [Header("Climbing")]
    public bool climbing;
    bool climbingLeft;
    Vector3 currentAnchor;
    public float currentAnchorAngle;
    public float anchorOffset;


    [Header("Landing")]
    public bool showCollisionSpheres;
    public Vector3 forwardRayOffset;
    public float forwardLength;
    public float downLength;
    public Vector2 validLandingAngleRange;
    public Vector2 validWallAngleRange;
    public float minSwoopDist = 3;
    public float landingSpeedMult = 20;
    float landingSpeed;

    [Header("Combat")]
    public float basicStrikeCooldown;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public Vector3 projectileOffset = Vector3.zero;
    private float basicCountdown = 0;

    [Header("Refrences")]
    public Animator animator;
    public GameObject wings;        //REMOVE 
    public GameObject rightWingtip;
    public GameObject leftWingtip;

    [Header("Animation")]
    public float walkingThreshold = 0.2f;
    public float takeOffTime = 2;


    [Header("testing")]
    public bool TESTSHOWRAY;
    public Vector3 directionRay;

    //[HideInInspector]
    [HideInInspector] public int remainingFlaps;
    [HideInInspector] public bool flying;
     public Vector3 cameraRot;
    [HideInInspector] public Vector3 groundedPos;
    [HideInInspector] public Rigidbody rb;
    GameManager manager;

    //inspector cleanup
    private void OnValidate()
    {
        //stats
        maxHealth = stats.maxHealth;
        health = stats.health;
        xp = stats.xp;
        xpToNextLevel = stats.xpToNextLevel;
        level = stats.level;

        //walking
        walkSpeed = walking.walkSpeed;
        strafeSpeed = walking.strafeSpeed;
        frictionFactor = walking.frictionFactor;
        turnSpeed = walking.turnSpeed;
        normalCC = walking.normalRB;

        //jumping
        jumpForce =jumping.jumpForce;
        jumpingGravity =jumping.jumpingGravity;
        fallingGravity =jumping.fallingGravity;
        jumpBufferMax =jumping.jumpBufferMax;
        grounded = jumping.grounded;
        groundLayer =jumping.groundLayer;
        groundedCheckOffset = jumping.groundedCheckOffset;
        groundedCheckRadius =jumping.groundedCheckRadius;
    }

    // Start & update
    void Start()
    {
        manager = Directory.gMan;
        rb = GetComponent<Rigidbody>();
        originalConstraints = rb.constraints;
        groundedPos = transform.position;
        //manager.player = this;
        OnValidate();
    }

    private void Update() {
        //GroundedCheck();
        //grounded = true;
        if (!flying && !climbing) {
            //ProcessJump();
            //animator.SetBool("walking", (grounded && (Mathf.Abs(rb.velocity.x) > walkingThreshold || Mathf.Abs(rb.velocity.y) > walkingThreshold)));
        }
        else if (climbing) {
            //ProcessClimbing();
        }

        if (grounded && !flying && Input.GetKeyDown(takeOffKey)) {
            //TakeOffFromGround();
        }
        //ProcessCombat();

        //TESTING
        
        /*if (TESTSHOWRAY) {
            Debug.DrawRay(transform.position, directionRay);
        }*/
    }
    private void FixedUpdate() {
        if (flying) {
            //Fly();
            //CheckFlyingCollisions();
            //FlapCheck();
            //animator.SetBool("walking", false);
        }
    }

    //combat
    void ProcessCombat() {
        
        basicCountdown -= Time.deltaTime;

        if (Input.GetButton("Fire1") && basicCountdown <= 0 && totalFeathers > 0.2f) {
            basicCountdown = basicStrikeCooldown;
            BasicStrike();
        }
    }
    void BasicStrike() {
        totalFeathers -= 0.2f;
        GameObject newProjectile = Instantiate(projectilePrefab, transform.position + projectileOffset, Quaternion.identity);
        newProjectile.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 3000);
        newProjectile.transform.rotation = Camera.main.transform.rotation;
    }
    
    //flying
    void Fly() {
        bool keyboard = false;

        if (keyboard) {
            if (Input.GetKey(forwardKey)) {
                currentPitch += pitchChangeRate;
                transform.Rotate(pitchChangeRate, 0, 0);
            }
            else if (Input.GetKey(backKey)) {
                currentPitch -= pitchChangeRate;
                transform.Rotate(-pitchChangeRate, 0, 0);
            }

            if (Input.GetKey(leftKey)) {
                transform.Rotate(0, 0, rollSpeed);
            }
            if (Input.GetKey(rightKey)) {
                transform.Rotate(0, 0, -rollSpeed);
            }
        }
        else {
            float horizontal = -Input.GetAxis("Mouse X") * mouseRollSpeed;
            float vertical = -Input.GetAxis("Mouse Y") * mousePitchSpeed;

            currentPitch += vertical;
            transform.Rotate(vertical, 0, 0);
            transform.Rotate(0, 0, horizontal);
        }

        float rPos = rightWingtip.transform.position.y;
        float lPos = leftWingtip.transform.position.y;
        if (Mathf.Abs(rPos - lPos) > levelCutoff) {
            if (rPos > lPos) {
                transform.Rotate(0, -levelSpeed, 0);
                transform.Rotate(0, 0, -bankSpeed);
            }
            else {
                transform.Rotate(0, levelSpeed, 0);
                transform.Rotate(0, 0, bankSpeed);
            }
        } 

        var downAngle = Vector3.Dot(transform.forward, Vector3.down);
        if (downAngle > 0) {
            flySpeed += (downAngle * downBase);
        }
        else {
            flySpeed += (downAngle * upBase);
            if  (flySpeed < 0) {
                flySpeed = 0;
            }
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * flySpeed, ForceMode.VelocityChange);
        rb.AddForce(Vector3.down * flyGravity * (rb.mass * rb.mass));
    }
    void CheckFlyingCollisions()
    {
        if (!flying) { return; }
        //for the down one
        //raycast down (Debug.DrawRay(transform.position + forwardRayOffset,((transform.up * -1).normalized * downLength)))
        //if hit is not null, calculate the slope of the normal in hit.normal 
        int layerMask = 1 << 7;
        layerMask = ~layerMask;
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * downLength, Color.magenta);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out var hit, downLength, layerMask)) {
            var angleXZ = Vector3.Angle(hit.normal, Vector3.up);
            var y1 = Vector3.Angle(hit.normal, new Vector3(1, 0, 0));
            var y2 = Vector3.Angle(hit.normal, new Vector3(-1, 0, 0));
            var y3 = Vector3.Angle(hit.normal, new Vector3(0, 0, 1));
            var y4 = Vector3.Angle(hit.normal, new Vector3(0, 0, -1));
            var yAngle = y3 == y4 ? y1 - 90 : y4;

            if (!(Input.GetKey(forwardKey) && hit.distance > minSwoopDist)) {
                bool validLanding = angleXZ > validLandingAngleRange.x && angleXZ < validLandingAngleRange.y;
                if (validLanding) {
                    print("land!");
                    Land();
                    return;
                }
                bool validWall = angleXZ > validWallAngleRange.x && angleXZ < validWallAngleRange.y;
                if (validWall) {
                    
                    LandOnWall(hit.point, hit);
                }
            }
        }

        //same idea but w forward
        if (Physics.Raycast(transform.position + forwardRayOffset, transform.TransformDirection(Vector3.forward), out hit, forwardLength,layerMask)) {
            var angleXZ = Vector3.Angle(hit.normal, Vector3.up);

            var y1 = Vector3.Angle(hit.normal, new Vector3(1, 0, 0));
            var y2 = Vector3.Angle(hit.normal, new Vector3(-1, 0, 0));
            var y3 = Vector3.Angle(hit.normal, new Vector3(0, 0, 1));
            var y4 = Vector3.Angle(hit.normal, new Vector3(0, 0, -1));
            var yAngle = y3==y4? y1-90 : y4;
            print("1: " + y1 + ", -1: " + y2 + ", 01: " + y3 + ", 0-1: " + y4);

            if (!(Input.GetKey(forwardKey) && hit.distance > minSwoopDist)) {
                bool validLanding = angleXZ > validLandingAngleRange.x && angleXZ < validLandingAngleRange.y;
                if (validLanding) {
                    //print("land!");
                    Land();
                    return;
                }
                bool validWall = angleXZ > validWallAngleRange.x && angleXZ < validWallAngleRange.y;
                if (validWall) {
                    LandOnWall(hit.point, hit);
                }
            }

        }

    }
    void FlapCheck()
    {
        if (!flying) { return; }

        if (Input.GetKeyDown(landKey)) {
            Land();
            return;
        }
        if (Input.GetKeyDown(jumpKey) && (remainingFlaps >= 1 || extraFlaps > 0)) {
            if (extraFlaps > 0) {
                extraFlaps -= 1;
            }
            else {
                remainingFlaps -= 1;
            }
            flySpeed = Mathf.Max(flapSpeed, flySpeed + flapSpeed);
            animator.SetTrigger("flap");
        }
    }
    void TakeOffFromGround() {
        transform.localEulerAngles += Vector3.up * animator.transform.localEulerAngles.y;
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);

        //print("taking off");
        animator.SetTrigger("takeOff");
        takingOff = takeOffTime*2;

        StartCoroutine("DelayedTakeOffFromGround");
    }
    IEnumerator DelayedTakeOffFromGround()
    {
        yield return new WaitForSeconds(takeOffTime);

        takingOff = 1;
        animator.SetBool("flying", true);
        flying = true;
        normalCC.enabled = false;
        flyingCC.enabled = true;
        wings.SetActive(true);

        currentPitch = transform.eulerAngles.x + 90;
        currentPitch %= 360;
        while (currentPitch < 0) {
            currentPitch += 360;
        }
        currentPitch = 90;
        //currentRoll = 0;

        //print("adding force!");
        rb.AddForce(new Vector3(0, 1.8f, 1) * takeOffSpeed, ForceMode.VelocityChange);
        flySpeed = takeOffSpeed;
        rb.constraints = RigidbodyConstraints.None;
    }
    void takeOffFromAir()
    {
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);

        animator.SetBool("flying", true);
        flying = true;
        normalCC.enabled = false;
        flyingCC.enabled = true;
        wings.SetActive(true);

        rb.AddForce(transform.forward * (takeOffSpeed/3), ForceMode.VelocityChange);
        flySpeed = takeOffSpeed;
        rb.constraints = RigidbodyConstraints.None;
    }
    void Land() {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        rb.constraints = originalConstraints;
        flying = false;
        normalCC.enabled = true;
        flyingCC.enabled = false;
        GroundedCheck();
        if (!grounded) {
            falling = true;
        }
        wings.SetActive(false);
        rb.AddForce(transform.forward * flySpeed * landingSpeedMult, ForceMode.Acceleration);
        landingSpeed = flySpeed * landingSpeedMult;
    }
    void RoundTotalFeathers()
    {
        if (Mathf.Approximately(totalFeathers, 1)) { totalFeathers = 1; }
        if (totalFeathers < 0.1) { totalFeathers = 0; }
    }

    //climbing
    void ProcessClimbing()
    {
        //print("anchor + offset: " + currentAnchor + ", " + anchorOffset);
        transform.position = currentAnchor;

        if (Input.GetKeyDown(leftKey)) {
            climbingLeft = true;
        }
        if (Input.GetKeyDown(rightKey)) {
            climbingLeft = false;
        }

        animator.SetBool("climbingLeft", climbingLeft);

        if (Input.GetKeyDown(takeOffKey)) {
            transform.localEulerAngles += Vector3.up * 180;
            transform.position += transform.forward * 2;
            climbing = false;
            animator.SetBool("climbing", false);
            takeOffFromAir();
        }
    }
    void LandOnWall(Vector3 _anchor, RaycastHit hit)
    {
        //print("landing on wall");
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
        transform.Rotate(0, -90, 0);
        //transform.localEulerAngles = (Vector3.up * currentAnchorAngle);
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);
        currentAnchor = _anchor;
        currentAnchor += (transform.forward * -anchorOffset);


        rb.velocity = Vector3.zero;
        rb.constraints = originalConstraints;
        flying = false;
        normalCC.enabled = true;
        flyingCC.enabled = false;

        climbing = true;
        animator.SetBool("climbing", true);

        wings.SetActive(false);
    }

    //walking & running
    void ProcessWalk() {
        rb.angularVelocity = Vector3.zero;
        Vector3 localChildEulers = animator.transform.localEulerAngles;
        //localChildEulers.y = GameManager.instance.Circularize(localChildEulers.y, false);
        animator.transform.localEulerAngles = localChildEulers;

        bool f = Input.GetKey(forwardKey);
        bool b = Input.GetKey(backKey);
        bool l = Input.GetKey(leftKey);
        bool r = Input.GetKey(rightKey);

        bool pickAltChildRot = false;
        Vector2 normalAndAltChildRotTargets = Vector2.one * localChildEulers.y;

        if (f || b || l || r) {

            float target = cameraRot.y;

            float current = transform.localEulerAngles.y;
            float threshold = (target > 180) ? target - 180 : target + 180;
            float altTarget = (target > 180) ? target - 360 : target + 360;
            bool pickAltPath = (current > threshold && threshold < 180) || (current < threshold && threshold > 180);
            Vector3 lerpTarget = new Vector3(transform.localEulerAngles.x, (!pickAltPath) ? (altTarget) : target, transform.localEulerAngles.z);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, lerpTarget, 0.025f);
        }

        if (l) {
            //animator.gameObject.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, localChildEulers.y > 90? 270: -90, 0), 0.025f);
            normalAndAltChildRotTargets = new Vector2(270, -90);
            pickAltChildRot = localChildEulers.y < 90;

            rb.velocity = Vector3.Lerp(rb.velocity, transform.right * -strafeSpeed, frictionFactor);
        }
        if (r) {
            normalAndAltChildRotTargets = new Vector2(90, 450);
            pickAltChildRot = localChildEulers.y >= 270;

            rb.velocity = Vector3.Lerp(rb.velocity, transform.right * strafeSpeed, frictionFactor);
        }
        if (f) {
            if (r) {
                normalAndAltChildRotTargets = new Vector2(45, 405);
                pickAltChildRot = localChildEulers.y > 225;
            }
            else if (l) {
                //animator.gameObject.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, (localChildEulers.y < 135) ? -45 : 315, 0), 0.025f);
                normalAndAltChildRotTargets = new Vector2(315, -45);
                pickAltChildRot = localChildEulers.y < 135;
            }
            else {
                //animator.gameObject.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, (localChildEulers.y > 180) ? 360 : 0, 0), 0.025f);
                normalAndAltChildRotTargets = new Vector2(0, 360);
                pickAltChildRot = localChildEulers.y > 180;
            }
            
            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * walkSpeed, frictionFactor);
        }
        else if (b) {
            if (r) {
                //animator.gameObject.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, (localChildEulers.y > 315) ? 495 : 135, 0), 0.025f);
                normalAndAltChildRotTargets = new Vector2(135, 495);
                pickAltChildRot = localChildEulers.y > 315;
            }
            else if (l) {
                //animator.gameObject.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, (localChildEulers.y < 45) ? -135 : 225, 0), 0.025f);
                normalAndAltChildRotTargets = new Vector2(225, -135);
                pickAltChildRot = localChildEulers.y < 45;
            }
            else {
                //animator.gameObject.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, 180, 0), 0.025f);
                normalAndAltChildRotTargets = new Vector2(180, 360+180);
                pickAltChildRot = false;
            }
            
            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * -walkSpeed, frictionFactor);
        }
        else {
            if (landingSpeed > 0.1f) {
                landingSpeed = Mathf.Lerp(landingSpeed, 0, 0.025f);
            }
            else {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, frictionFactor);
            }
        }
        if (normalAndAltChildRotTargets.x != normalAndAltChildRotTargets.y) {
            //print("current: " + localChildEulers.y + ", normal: " + normalAndAltChildRotTargets.x + ", pickAlt: " + pickAltChildRot + ", chosen: " + (pickAltChildRot?normalAndAltChildRotTargets.y:normalAndAltChildRotTargets.x));
            animator.transform.localEulerAngles = Vector3.Lerp(localChildEulers, new Vector3(0, pickAltChildRot ? normalAndAltChildRotTargets.y : normalAndAltChildRotTargets.x, 0), 0.025f);
        }
        //if (animator.transform.localEulerAngles.y < 0) { animator.transform.localEulerAngles = new Vector3(animator.transform.localEulerAngles.x, animator.transform.localEulerAngles.y + 360, animator.transform.localEulerAngles.z); }
        //if (animator.transform.localEulerAngles.y > 360) { animator.transform.localEulerAngles = new Vector3(animator.transform.localEulerAngles.x, animator.transform.localEulerAngles.y - 360, animator.transform.localEulerAngles.z); }
    }

    //jumping
    void GroundedCheck() {
        grounded = false;
        takingOff -= Time.deltaTime;
        if (takingOff > 0 || climbing) {
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position + groundedCheckOffset, groundedCheckRadius);
        foreach (Collider collider in colliders) {
            if (collider.gameObject != gameObject && collider.gameObject.layer == groundLayer) {
                grounded = true;
                climbing = false;
                animator.SetBool("climbing", false);
                animator.SetBool("flying", false);
                falling = false;
                RoundTotalFeathers();
                remainingFlaps = Mathf.FloorToInt(totalFeathers);
                /*if (flying) {
                    Land();
                }*/
            }
        }

        if (grounded) {
            groundedPos = transform.position;
        }
        else {
            groundedPos.x = transform.position.x;
            groundedPos.z = transform.position.z;
        }
        animator.SetBool("grounded", grounded);
        
        
    }
    void ProcessJump() {
        timeSinceJumpButton += Time.deltaTime;


        bool jumpFall = false; 

        if (Input.GetKeyDown(jumpKey)) {
            timeSinceJumpButton = 0;
            /*if (!grounded && manager.facts.Contains("wingsUnlocked")) {
                TakeOff();
                return;
            }*/
        }

        if (grounded && timeSinceJumpButton <= jumpBufferMax) {
            print("jump!");
            grounded = false;
            timeSinceJumpButton = jumpBufferMax + 1;
            animator.SetTrigger("jump");
            rb.AddForce(transform.up * (jumpForce*10), ForceMode.Impulse);
        }

        if (!grounded && !flying) {
            if (rb.velocity.y > 0) {
                rb.AddForce(Vector3.down * jumpingGravity * (rb.mass * rb.mass));
            }
            else if (!falling) {
                jumpFall = true;
                rb.AddForce(Vector3.down * fallingGravity * (rb.mass * rb.mass));
            }
            else {
                rb.AddForce(Vector3.down * (fallingGravity * freefallMod) * (rb.mass * rb.mass));
            }
            
        }
        animator.SetBool("jumpFall", jumpFall);
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(transform.position + groundedCheckOffset, groundedCheckRadius);

        //Debug.DrawRay(transform.position + forwardRayOffset,((transform.forward).normalized * forwardLength) , Color.blue);
        //Debug.DrawRay(transform.position + forwardRayOffset,((transform.up * -1).normalized * downLength), Color.magenta);
        

    }
}
