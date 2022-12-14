using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Door : MonoBehaviour
{
    public enum openType { swing, slide}

    public openType type;
    public bool automatic;
    bool open;
    bool active;
    public float openSpeedFactor = 0.05f;
    public bool setOpen;
    public bool setClose;

    [Header("Swing")]
    public Quaternion closeRot;
    public Quaternion openRot;

    [Header("Slide")]
    public Vector3 closePos;
    public Vector3 openPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (setOpen) {
            setOpen = false;
            openRot = transform.rotation;
            openPos = transform.position;
        }
        if (setClose) {
            setClose = false;
            closeRot = transform.rotation;
            closePos = transform.position;
        }

        if (active) {
            switch (type) {
                case openType.swing:
                    Quaternion targetRotation = openRot;
                    if (open) { targetRotation = closeRot; }
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, openSpeedFactor);
                    if (transform.rotation == targetRotation) {
                        open = !open;
                        active = false;
                    }
                    break;
                case openType.slide:
                    Vector3 targetPos = openPos;
                    if (open) { targetPos = closePos; }
                    transform.position = Vector3.Lerp(transform.position, targetPos, openSpeedFactor);
                    if (transform.position == targetPos) {
                        open = !open;
                        active = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void Open() {
        open = false;
        active = true;
        print("opening door");
    }

    void Close() {
        open = true;
        active = true;
        print("closing door");
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.GetComponent<PlayerController>()) {
            if (automatic) {
                Open();
            }
        }
    }
    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.GetComponent<PlayerController>()) {
            if (automatic) {
                Close();
            }
        }
    }
}
