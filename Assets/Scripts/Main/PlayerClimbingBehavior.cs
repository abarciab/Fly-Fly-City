using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class PlayerClimbingBehavior : MonoBehaviour
{
    PlayerStateCoordinator stateController;
    [SerializeField] Slider staminaSlider;
    [SerializeField] float maxStamina, stamina, minStamina = 0.7f;

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

    private void OnEnable()
    {
        stamina = Mathf.Max(minStamina, stamina);
        staminaSlider.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!stateController) {
            Start();
        }
        ProcessClimbing();

        stamina -= Time.deltaTime;
        staminaSlider.value = stamina / maxStamina;
        if (stamina <= 0) Dismount();
    }

    public void ResetStamina()
    {
        stamina = maxStamina;
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
        staminaSlider.gameObject.SetActive(false);
        transform.localEulerAngles += Vector3.up * 180;
        rb.AddForce(transform.forward * dismountForce, ForceMode.Impulse);
        stateController.EnterFallingState();
    }

    string ProcessInput() {
        string s = ""; 
        GameManager manager = Directory.gMan;
        if (Input.GetKey(forwardKey)) s += "f";
        if (Input.GetKey(backKey)) s += "b";
        if (Input.GetKey(rightKey)) s += "r";
        if (Input.GetKey(leftKey)) s += "l";
        if (Input.GetKey(takeOffKey)) s += "c";

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
