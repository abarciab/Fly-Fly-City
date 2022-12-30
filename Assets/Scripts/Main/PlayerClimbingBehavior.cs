using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingBehavior : MonoBehaviour
{
    PlayerStateCoordinator stateController;

    public float anchorOffset;
    [SerializeField] float dismountForce = 10;

    [HideInInspector] public bool facingLeft { get; private set; }
    Vector3 currentAnchor;
    float currentAnchorAngle;

    //dependencies
    Rigidbody rb;


    void Start()
    {
        stateController = GetComponent<PlayerStateCoordinator>();
        if (!stateController) {
            enabled = false;
            return;
        }
        rb = stateController.rb;
    }

    void Update()
    {
        if (!stateController) {
            Start();
        }
        ProcessClimbing();
    }

    void ProcessClimbing() {
        string inputString = ProcessInput();
        FaceDirection(inputString);

        if (ShouldClimb(inputString)) {
            //Climb()
        }
        else if (ShouldJump(inputString)) {
            //Jump()
        }
        else if (ShouldDismount(inputString)) {
            Dismount();
        }
    }

    void Dismount() {
        transform.localEulerAngles += Vector3.up * 180;
        rb.AddForce(transform.forward * dismountForce, ForceMode.Impulse);
        stateController.EnterFallingState();
    }

    string ProcessInput() {
        string s = ""; 
        GameManager manager = GameManager.instance;
        if (Input.GetKey(manager.forwardKey)) s += "f";
        if (Input.GetKey(manager.backKey)) s += "b";
        if (Input.GetKey(manager.rightKey)) s += "r";
        if (Input.GetKey(manager.leftKey)) s += "l";
        if (Input.GetKey(manager.takeOffKey)) s += "c";

        return s;
    }

    void FaceDirection(string inputString) {
        if (inputString.Contains("l")) facingLeft = true;
        if (inputString.Contains("r")) facingLeft = false;
    }

    bool ShouldClimb(string inputString) {
        return false;
    }

    bool ShouldJump(string inputString) {
        return false;
    }

    bool ShouldDismount(string inputString) {
        return inputString.Contains("c");
    }
}
