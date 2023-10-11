using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlyingBehavior : MonoBehaviour
{
    PlayerStateCoordinator stateController;

    [Header("Flying")]
    [SerializeField] float mousePitchSpeed = 1;
    [SerializeField] float mouseRollSpeed = 1;
    [SerializeField] float levelSpeed = 0.5f;         //how quickly the wings level out when banking left or right
    [SerializeField] float levelCutoff = 0.05f;       //how close does the y altitude of the two wingtips have to be for baking to stop
    [SerializeField] float flyGravity = 9;            //gravity, applied constantly when flying
    [SerializeField] float downBase = 0.5f;           //multiplier when flying down
    [SerializeField] float upBase = 0.5f;             //multiplier when flying up     
    public float takeOffSpeed = 15;         //initial speed when taking off
    public float takeOffTime;
    [SerializeField] int maxFlaps = 3;
    int flapsRemaining;
    [SerializeField] float flapSpeed = 15;             
    [HideInInspector] public float flySpeed;

    [Header("Slowing down")]
    [SerializeField] float slowSpeedMin = 15;
    [SerializeField, Range(0, 1)] float slowSpeedpercent = 0.1f;
    [SerializeField] ParticleSystem slowParticles;
    [SerializeField] int slowParticleEmitCount;

    [Header("Landing")]
    [SerializeField] bool showCollisionSpheres;
    [SerializeField] Vector3 forwardRayOffset;
    [SerializeField] float forwardLength;
    [SerializeField] float downLength;
    [SerializeField] Vector2 validLandingAngleRange;
    [SerializeField] Vector2 validWallAngleRange;

    [Header("windtipWind")]
    [SerializeField] TrailRenderer wingTipwind1;
    [SerializeField] TrailRenderer wingTipwind2;
    [SerializeField] float wingTipTime = 0.1f;

    //dependencies
    [SerializeField] GameObject rightWingtip;
    [SerializeField] GameObject leftWingtip;
    Rigidbody rb;


    [Header("Misc")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] Sound slowSound;

    public void AddFlap(int num)
    {
        maxFlaps += num;
    }

    void Start()
    {
        slowSound = Instantiate(slowSound);

        stateController = GetComponent<PlayerStateCoordinator>();
        if (!stateController) {
            enabled = false;
            return;
        }
        rb = stateController.rb;
    }

    private void OnEnable() {
        flapsRemaining = maxFlaps;
        wingTipwind1.time = wingTipwind2.time = wingTipTime;
    }

    private void OnDisable()
    {
        wingTipwind1.time = wingTipwind2.time = 0;
    }

    private void Update() {
        if (ShouldFlap()) Flap();
        SlowDownCheck();
        Directory.cam.SetFlySpeed(Mathf.Min(flySpeed, 100) / 100);
    }

    void FixedUpdate()
    {
        if (!stateController) {
            Start();
            return;
        }
        ProcessFlight(); 
        CheckCollisions();
        AddForce();
    }

    void ProcessFlight() {
        ProcessInput();
        LevelAndTurn();
        AdjustFlightSpeed();
    }

    bool ShouldFlap() {
        return Input.GetKeyDown(Constants.jumpKey) && (flapsRemaining > 0);
    }

    void Flap() {
        flapsRemaining -= 1;
        flySpeed = Mathf.Max(flapSpeed, flySpeed + flapSpeed);
        stateController.CallTrigger("flap");
    }

    void AddForce() {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * flySpeed, ForceMode.VelocityChange);
        rb.AddForce(Vector3.down * flyGravity * (rb.mass * rb.mass));
    }

    void AdjustFlightSpeed() {
        var downAngle = Vector3.Dot(transform.forward, Vector3.down);
        if (downAngle > 0) {
            flySpeed += (downAngle * downBase);
        }
        else {
            flySpeed += (downAngle * upBase);
            if (flySpeed < 0) {
                flySpeed = 0;
            }
        }
    }

    void LevelAndTurn() {
        float wingAngle = GetWingAngle();
        if (Mathf.Abs(wingAngle) <= levelCutoff) return;

        if (wingAngle > 0) {
            transform.Rotate(0, -levelSpeed, 0);
            transform.Rotate(0, 0, -levelSpeed/2);
        }
        else {
            transform.Rotate(0, levelSpeed, 0);
            transform.Rotate(0, 0, levelSpeed/2);
        }
    }

    float GetWingAngle() {
        float rPos = rightWingtip.transform.position.y;
        float lPos = leftWingtip.transform.position.y;

        return (rPos - lPos);
    }

    void ProcessInput() {
        float horizontal = -Input.GetAxis("Mouse X") * mouseRollSpeed;
        float vertical = -Input.GetAxis("Mouse Y") * mousePitchSpeed;

        transform.Rotate(vertical, 0, 0);
        transform.Rotate(0, 0, horizontal);        
    }

    void SlowDownCheck()
    {
        if (Input.GetKeyDown(KeyCode.S) && flySpeed > Mathf.Abs(slowSpeedMin * 1.5f)) SlowDown();
    }

    void SlowDown()
    {
        float slowSpeed = Mathf.Max(flySpeed * slowSpeedpercent, slowSpeedMin);
        flySpeed -= slowSpeed;
        slowSound.Play();
        slowParticles.Emit(slowParticleEmitCount);
    }

    void CheckCollisions() {
        if (CheckCollision(Vector3.down, downLength, Vector3.zero)) return;
        CheckCollision(Vector3.forward, forwardLength, forwardRayOffset);
    }

    bool CheckCollision(Vector3 rayDir, float rayLength, Vector3 offset) {

        if (Physics.Raycast(transform.position + offset, transform.TransformDirection(rayDir), out var hit, rayLength, collisionMask)) {
            var angleXZ = Vector3.Angle(hit.normal, Vector3.up);

            bool validLanding = angleXZ > validLandingAngleRange.x && angleXZ < validLandingAngleRange.y;
            if (validLanding) {
                stateController.EnterWalkingState();
                return true;
            }
            bool validWall = angleXZ > validWallAngleRange.x && angleXZ < validWallAngleRange.y;
            if (validWall) {
                //GOTO CLIMBING STATE
                stateController.EnterClimbingState(hit);
                return true;
            }
        }
        return false;
    }




    //OLD
    
}
