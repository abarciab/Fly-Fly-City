using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Wipe : MonoBehaviour
{
    [SerializeField] AnimationCurve moveCurve;
    [SerializeField] Vector3 startPos, holdPos, endPos;
    [SerializeField] bool setStart, setHold, setEnd;

    private void Update()
    {
        if (Application.isPlaying && Input.GetKeyDown(KeyCode.L)) DoWipe();

        if (setStart) {
            setStart = false;
            startPos = transform.localPosition;
        }
        if (setEnd) { 
            setEnd = false;
            endPos = transform.localPosition;
        }
        if (setHold) {
            setHold = false;
            holdPos = transform.localPosition;
        }
    }

    public void DoWipe(float startTime = 0.5f, float holdTime = 0.5f, float endTime = 0.5f)
    {
        StartCoroutine(WipeAnimate(startTime, holdTime, endTime));
    }

    IEnumerator WipeAnimate(float startTime, float holdTime, float endTime)
    {
        float timePassed = 0;
        while (timePassed < startTime) {
            float progress = timePassed / startTime;
            progress = moveCurve.Evaluate(progress);
            transform.localPosition = Vector3.Lerp(startPos, holdPos, progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = holdPos;

        timePassed = 0;
        while (timePassed < holdTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timePassed = 0;
        while (timePassed < endTime) {
            float progress = timePassed / endTime;
            progress = moveCurve.Evaluate(progress);
            transform.localPosition = Vector3.Lerp(holdPos, endPos, progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = endPos;
    }
}
