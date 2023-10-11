using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class Doorway : MonoBehaviour
{
    LocationController location;

    [Header("Opening")]
    [SerializeField] Transform model;

    [SerializeField] bool setOpen, setClose, openButton, closeButton, animateInEditMode;
    [SerializeField] float openTime = 1;
    [SerializeField] AnimationCurve openCloseCurve;

    [SerializeField] Vector3 openPos, closePos;
    [SerializeField] Quaternion openRot, closeRot;
    [SerializeField] Vector3 insidePlacement, outsidePlacement;

    [Header("Prompts")]
    [SerializeField] float promptRange;
    [SerializeField] GameObject openPrompt, openInterior, enterPrompt, enterInterior;
    Transform player;
    bool open, moving;

    private void Start()
    {
        if (Application.isPlaying) player = Directory.gMan.player.transform;
        location = GetComponentInParent<LocationController>();
    }

    private void Update()
    {
        if (openButton) {
            openButton = false;
            Open();

        }
        if (closeButton) {
            closeButton = false;
            Close();
        }

        if (Application.isPlaying) {
            RunTimeBehavior();
            return;
        }

        if (setOpen) {
            setOpen = false;
            openPos = model.transform.localPosition;
            openRot = model.transform.localRotation;
        }
        if (setClose) {
            setClose = false;
            closePos = model.transform.localPosition;
            closeRot = model.transform.localRotation;
        }
    }

    void RunTimeBehavior()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < promptRange) {
            if (moving) HidePrompts();
            if (!open && !moving) ShowOpenPrompt();
            if (open && !moving) ShowEnterPrompt();
        }
        else HidePrompts();

        if (OpenPromptActive() && Input.GetKey(Constants.interactKey)) Open();
        if (EnterPromptActive() && Input.GetKey(Constants.interactKey)) StartCoroutine(TraverseDoor());

    }
    bool EnterPromptActive()
    {
        return enterPrompt.activeInHierarchy || enterInterior.activeInHierarchy;
    }

    bool OpenPromptActive()
    {
        return openPrompt.activeInHierarchy || openInterior.activeInHierarchy;
    }

    void ShowOpenPrompt()
    {
        var prompt = Directory.gMan.insideLocation ? openInterior : openPrompt;
        prompt.SetActive(true);
    }

    void ShowEnterPrompt()
    {
        var prompt = Directory.gMan.insideLocation ? enterInterior : enterPrompt;
        prompt.SetActive(true);
    }

    IEnumerator TraverseDoor()
    {
        open = false;
        moving = true;

        Directory.gMan.player.SetPaused(true);
        Directory.gMan.wipe.DoWipe(0.5f, 0.1f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        var pos = Directory.gMan.insideLocation ? outsidePlacement : insidePlacement;
        pos = transform.TransformPoint(pos);
        pos.y = player.position.y;
        player.position = pos;

        model.transform.localPosition = closePos;
        model.transform.localRotation = closeRot;

        yield return new WaitForSeconds(0.3f);
        Directory.gMan.player.SetPaused(false);

        Directory.gMan.SetInside(!Directory.gMan.insideLocation);
        if (Directory.gMan.insideLocation) {
            location.Enter();
            Directory.gMan.EnterNewLocation(location.data.name);
        }
        moving = false;
    }

    void HidePrompts()
    {
        openPrompt.SetActive(false);
        enterPrompt.SetActive(false);
        openInterior.SetActive(false);
        enterInterior.SetActive(false);
    }

    void Open()
    {
        StopAllCoroutines();
        if (Application.isPlaying || animateInEditMode) StartCoroutine(AnimateOpen(closePos, openPos, closeRot, openRot));
        else {
            model.transform.localPosition = openPos;
            model.transform.localRotation = openRot;   
        }
        open = true;
    }

    void Close()
    {
        StopAllCoroutines();
        if (Application.isPlaying || animateInEditMode) StartCoroutine(AnimateOpen(openPos, closePos, openRot, closeRot));
        else {
            model.transform.localPosition = closePos;
            model.transform.localRotation = closeRot;
        }
        open = false;
    }

    IEnumerator AnimateOpen(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        moving = true;
        float timePassed = 0;
        while (timePassed < openTime) {
            float progress = timePassed / openTime;
            progress = openCloseCurve.Evaluate(progress);

            model.transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
            model.transform.localRotation = Quaternion.Lerp(startRot, endRot, progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        model.transform.localPosition = endPos;
        model.transform.localRotation = endRot;

        moving = false;
        open = endPos == openPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, promptRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(insidePlacement), 0.5f);
        Gizmos.DrawWireSphere(transform.TransformPoint(outsidePlacement), 0.5f);
    }
}
