using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("the smoothness of the transition between the walking cam and the flying cam. higher number are smoother, but only to a point")]
    public int flyCamTransitionTime = 100;
    int transitionCount;
    Vector3 realOriginalCamPos;
    Vector3 originalCamPos = Vector3.one * -1;        //the localPos of the cam last time we were in normal mode
    Vector3 originalTargetPos;        //the localPos of the camLookTarget last time we were in normal mode
    bool transitioningBack;

    [Header("clipping avoidance")]
    public float clippingZOffset;
    public float clipZoomSpeed;
    public float minDistFromCamTarget = 1;
    public float camSize = 1;           //the space that needs to be between the camera and the nearest object before it'll zoom out again


    [Header("References")]
    public GameObject cameraLookTarget;     //object that is also a child of camera parent, across the middle from the camera (so the camera is always loking past the character)
    [SerializeField] PlayerStateCoordinator stateController;
    GameObject cameraParent;

    void Start()
    {
        realOriginalCamPos = transform.localPosition;
        cameraParent = transform.parent.gameObject;
        if (Application.isPlaying && !mouseEnabled) {
            transitionCount = flyCamTransitionTime;
            stateController = GameManager.instance.player;
            LockMouse();
        }
    }

    void LateUpdate()
    {
        if (Application.isPlaying) {
            if (Input.GetButtonDown("Fire1") && !mouseEnabled) {
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

        if (followWhenFlying && stateController.currenState == PlayerStateCoordinator.PossibleStates.flying) {
            return;
        }
        inputs.y += horizontal;
        inputs.x += vertical;
    }

    void MoveCam()
    {
        if (cameraParent == null) { cameraParent = transform.parent.gameObject; }
        
        if (followWhenFlying && stateController.currenState == PlayerStateCoordinator.PossibleStates.flying) {
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
                
                inputs.x = Mathf.Lerp(inputs.x, flyXAngle, transitionSmoothness);

                if (transitionCount <= 0) {
                    //print("done transitioning");
                    alreadyFollowing = true;
                    transitionCount = flyCamTransitionTime;
                }
            }
            else {
                cameraLookTarget.transform.localPosition = Vector3.zero;
                transform.localPosition = flyingOffset;
                inputs.x = flyXAngle;
            }
        }
        else {
            if (transform.localPosition != realOriginalCamPos && !transitioningBack) {
                StartCoroutine("TransitionBackToNormalCam");
            }
            float largestLimitDist = Mathf.Max(Mathf.Abs(parentRotXlimits.x - defaultX), Mathf.Abs(parentRotXlimits.y - defaultX));
            float percentFromMiddle = (Mathf.Abs(inputs.x - defaultX) / largestLimitDist) * (stateController.currenState == PlayerStateCoordinator.PossibleStates.flying ? 0 : 1);
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
            Physics.Raycast(transform.position, player.transform.position - transform.position, out var hit);
            if (count < 300 && hit.collider != null && hit.collider.tag != "Player" && Mathf.Abs(transform.localPosition.z - cameraLookTarget.transform.localPosition.z) > minDistFromCamTarget) {
                clippingZOffset += clipZoomSpeed;
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

        //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * -1, Color.red);

        //raycast behind camera, and if there's room, zoom out

        /*{
            if (currentDistance > distanceLimits.y && Physics.OverlapSphere(transform.position, 0.1f).Length == 0) {
                currentDistance -= 0.05f;
            }
        }
        currentOffset.z = currentDistance;*/
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

    /*if (!controllerScript.flying) {
            RaycastHit hit;
            Physics.Raycast(transform.position, target.transform.position - transform.position, out hit);
            if (hit.collider != null && hit.collider.name != "player" && hit.collider.name != "wings") {
                currentDistance += 0.05f;
            }
            else {
                if (currentDistance > distanceLimits.y && Physics.OverlapSphere(transform.position, 0.1f).Length == 0) {
                    currentDistance -= 0.05f;
                }
            }
            currentOffset.z = currentDistance;
        }
        

        if (Input.GetKeyDown(KeyCode.Escape)) {
            mouseEnabled = !mouseEnabled;
        }
        if (!mouseEnabled) {
            return;
        }
        else {
            Cursor.lockState = CursorLockMode.Confined;
        }

        Cursor.visible = false;
        parentDummy.transform.position = target.transform.position;
        transform.localPosition = currentOffset;

        float horizontal = Input.GetAxis("Mouse X") * mouseRotSpeed;
        float vertical = Input.GetAxis("Mouse Y") * mouseRotSpeed;
        vertical = -Mathf.Clamp(vertical, -90, 90);
        if (controllerScript.flying || true) {
            parentDummy.transform.Rotate(vertical, horizontal, 0);
        }
        else {
            parentDummy.transform.Rotate(0, horizontal, 0);
        }
        
        
        targetDummy.transform.localPosition = lookTargetOffset;
        transform.LookAt(targetDummy.transform);

        if (!controllerScript.flying) {
            controllerScript.cameraAlignedEuler = new Vector3(target.transform.eulerAngles.x, parentDummy.transform.eulerAngles.y, target.transform.eulerAngles.z);
        }*/
}
