using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    public Vector2 inputs;
    Vector2 parentRot;
    public Vector2 parentRotXlimits;
    public float baseZ = -5;
    public float angleZoomDist;             //as the camera tilts up and down, how much closer to we get to the character
    public float defaultX = 5;
    public Vector3 parentOffset;
    
    public float mouseRotSpeedX = 5;
    public float mouseRotSpeedY = 5;
    public GameObject player;       //get this from gamemanager instead lol

    public bool mouseEnabled;

    [Header("fly cam")]
    bool alreadyFollowing;
    public bool followWhenFlying = true;
    public Vector3 flyingOffset;
    public float flyXAngle;
    [SerializeField] bool facePlayerFlyDirection;
    [Tooltip("the smoothness of the transition between the walking cam and the flying cam. higher number are smoother, but only to a point")]
    public int flyCamTransitionTime = 100;
    int transitionCount;
    Vector3 realOriginalCamPos;
    Vector3 originalCamPos = Vector3.one * -1;        //the localPos of the cam last time we were in normal mode
    Vector3 originalTargetPos;        //the localPos of the camLookTarget last time we were in normal mode
    bool transitioningBack;
    [Space()]
    [SerializeField] Vector2 FOVminMax;
    float flySpeedPercent;

    [Header("clipping avoidance")]
    public float clippingZOffset;
    public float clipZoomSpeed;
    public float minDistFromCamTarget = 1;
    public float camSize = 1;           //the space that needs to be between the camera and the nearest object before it'll zoom out again
    [SerializeField] LayerMask camRaycastMask;


    [Header("References")]
    public GameObject cameraLookTarget;     //object that is also a child of camera parent, across the middle from the camera (so the camera is always loking past the character)
    [SerializeField] PlayerStateCoordinator stateController;
    GameObject cameraParent;
    Camera cam;

    [Header("Conversation")]
    [SerializeField] Speaker speaker;

    public void SetFlySpeed(float speedPercent)
    {
        flySpeedPercent = speedPercent;
    }

    public void LookSpeaker()
    {
        if (!speaker) return;
        StopAllCoroutines();
        transform.position = speaker.GetCamPos();
        transform.LookAt(speaker.getHeadPos());
    }

    public void LookPlayer()
    {
        StopAllCoroutines();
        var p = player.GetComponent<PlayerStateCoordinator>();  
        transform.position = p.GetCamPos();
        transform.LookAt(p.getHeadPos());
    }

    public void StartConversation(Speaker _speaker)
    {
        speaker = _speaker;
    }

    public void EndConversation()
    {
        speaker = null;
    }

    public void LookAtPlayer()
    {
        transform.LookAt(player.transform);
    }

    void Start()
    {
        realOriginalCamPos = transform.localPosition;
        cameraParent = transform.parent.gameObject;
        if (Application.isPlaying && !mouseEnabled) {
            transitionCount = flyCamTransitionTime;
            stateController = Directory.gMan.player;
            LockMouse();
        }
        cam = Camera.main;
    }

    private void Update()
    {
        if (!Application.isPlaying || speaker != null) return;

        if (Directory.gMan.player.currenState != PossiblePlayerStates.flying) flySpeedPercent = 0;
        float targetFov = Mathf.Lerp(FOVminMax.x, FOVminMax.y, flySpeedPercent);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 0.02f);
    }

    void LateUpdate()
    {
        if (speaker != null) return;

        if (Application.isPlaying) {
            if (!Directory.mapMan.gameObject.activeInHierarchy && Input.GetButtonDown("Fire1") && !mouseEnabled) {
                LockMouse();
            }
            ProcessMouseInput();
            MoveCam();
            PreventCamClipping();
        }
        else {
            MoveCam();
        }        
    }

    void ProcessMouseInput()
    {
        
        float horizontal = Input.GetAxis("Mouse X") * mouseRotSpeedY;
        float vertical = -Input.GetAxis("Mouse Y") * mouseRotSpeedX;

        if (followWhenFlying && stateController.currenState == PossiblePlayerStates.flying) {
            return;
        }
        inputs.y += horizontal;
        inputs.x += vertical;
    }

    void MoveCam()
    {
        if (cameraParent == null) { cameraParent = transform.parent.gameObject; }
        
        if (followWhenFlying && stateController.currenState == PossiblePlayerStates.flying) {
            float playerY = player.transform.localEulerAngles.y;
            if (Mathf.Abs(inputs.y - playerY) > 180 && Mathf.Abs(inputs.y - playerY) < 360) {
                if (inputs.y > playerY) {
                    inputs.y -= 360;
                }
                else {
                    inputs.y += 360;
                }
            }
            inputs.y = Mathf.Lerp(inputs.y, player.transform.localEulerAngles.y, 0.025f);

            if (!alreadyFollowing) {
                if (transitionCount == flyCamTransitionTime) {
                    originalCamPos = transform.localPosition;
                    originalTargetPos = cameraLookTarget.transform.localPosition;
                }
                transitionCount -= 1;
                float transitionSmoothness = 0.025f;

                cameraLookTarget.transform.localPosition = Vector3.Lerp(cameraLookTarget.transform.localPosition, Vector3.zero, transitionSmoothness);
                transform.localPosition = Vector3.Lerp(transform.localPosition, flyingOffset, transitionSmoothness);

                float targetX = facePlayerFlyDirection ? player.transform.eulerAngles.x : flyXAngle;
                inputs.x = EulerLerp(inputs.x, targetX, transitionSmoothness);

                if (transitionCount <= 0) {
                    alreadyFollowing = true;
                    transitionCount = flyCamTransitionTime;
                }
            }
            else {
                cameraLookTarget.transform.localPosition = Vector3.zero;
                transform.localPosition = flyingOffset;
                
                float targetX = facePlayerFlyDirection ? player.transform.eulerAngles.x : flyXAngle;
                inputs.x = EulerLerp(inputs.x, targetX, 0.05f);
            }
        }
        else {
            if (transform.localPosition != realOriginalCamPos && !transitioningBack) {
                StartCoroutine("TransitionBackToNormalCam");
            }
            float largestLimitDist = Mathf.Max(Mathf.Abs(parentRotXlimits.x - defaultX), Mathf.Abs(parentRotXlimits.y - defaultX));
            float percentFromMiddle = (Mathf.Abs(inputs.x - defaultX) / largestLimitDist) * (stateController.currenState == PossiblePlayerStates.flying ? 0 : 1);
            Vector3 pos = transform.localPosition;
            pos.z = baseZ + (angleZoomDist * percentFromMiddle) + clippingZOffset;
            transform.localPosition = pos;
            cameraLookTarget.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, cameraLookTarget.transform.localPosition.z);
            inputs.x = Mathf.Clamp(inputs.x, parentRotXlimits.x, parentRotXlimits.y);

            alreadyFollowing = false;
        }

        parentRot = inputs;
        cameraParent.transform.position = player.transform.position + parentOffset;

        transform.LookAt(cameraLookTarget.transform);
        Vector3 targetParentRot = new Vector3(parentRot.x, parentRot.y, 0);
        cameraParent.transform.localEulerAngles = targetParentRot;
    }

    float EulerLerp(float angle1, float angle2, float smoothness)
    {
        angle1 = clampEuler(angle1);
        angle2 = clampEuler(angle2);

        if (angle1 > 180 && angle2 < 180) angle2 += 360;
        if (angle1 < 180 && angle2 > 180) angle2 -= 360;

        return Mathf.Lerp(angle1, angle2, smoothness);
    }

    float clampEuler(float angle)
    {
        if (angle > 360) angle %= 360;
        while (angle < 0) angle += 360;

        return angle;
    }

    IEnumerator TransitionBackToNormalCam()
    {
        if (transitioningBack) { yield break; }
        transitioningBack = true; 
        var targetPos = realOriginalCamPos;
        for (int i = 0; i < flyCamTransitionTime; i++) {
            yield return new WaitForEndOfFrame();
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(targetPos.x, targetPos.y, transform.localPosition.z), 0.025f);
            cameraLookTarget.transform.localPosition = Vector3.Lerp(cameraLookTarget.transform.localPosition, new Vector3(originalTargetPos.x, originalTargetPos.y, originalTargetPos.z), 0.025f);
        }
        transform.localPosition = new Vector3(targetPos.x, targetPos.y, transform.localPosition.z);
        cameraLookTarget.transform.localPosition = new Vector3(originalTargetPos.x, originalTargetPos.y, originalTargetPos.z);
        transitioningBack = false;
    } 

    void PreventCamClipping()
    {
        Vector3 modifiedPos = transform.position;
        int count = 0;
        while (true) {
            count++; 
            Physics.Raycast(transform.position, player.transform.position - transform.position, out var hit, 50, camRaycastMask);
            if (count < 300 && hit.collider != null && hit.collider.tag != "Player" && Mathf.Abs(transform.localPosition.z - cameraLookTarget.transform.localPosition.z) > minDistFromCamTarget) {
                clippingZOffset = Mathf.Min(clippingZOffset + clipZoomSpeed, 3);
                modifiedPos += Vector3.forward * clipZoomSpeed;
                transform.localPosition += Vector3.forward * clipZoomSpeed;
            }
            else {
                break;
            }
        }

        count = 0;
        while (clippingZOffset > 0 && LineOfSightToPlayer()) {
            count++;
            Physics.Raycast(transform.position, (player.transform.position - transform.position)*-1, out var hit);
            if (count < 300 && hit.collider == null || hit.distance > camSize) {
                clippingZOffset -= clipZoomSpeed;
                modifiedPos -= Vector3.forward * clipZoomSpeed;
                transform.localPosition -= Vector3.forward * clipZoomSpeed;
            }
            else {
                break;
            }
        }
    }

    bool LineOfSightToPlayer()
    {
        Physics.Raycast(transform.position, player.transform.position - transform.position, out var hit);
        if (hit.collider != null && hit.collider.tag != "Player" && Mathf.Abs(transform.localPosition.z - cameraLookTarget.transform.localPosition.z) > minDistFromCamTarget) {
            return false;
        }
        else {
            return true;
        }
    }

    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
}
