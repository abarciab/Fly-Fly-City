using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MovingPlatform : MonoBehaviour
{
    public enum MoveType { linear, lerp}

    public Vector3 Pos1;
    public float waitTimeAtPos1 = 1;
    public bool setPos1;
    public Vector3 Pos2;
    public float waitTimeAtPos2 = 1;
    public bool setPos2;

    Vector3 currentTarget;
    float currentWaitTime = 0;
    bool finished;
    
    public bool bounce = true;
    public MoveType moveType;
    public float linearSpeed;
    public float lerpSpeed = 0.05f;
    public float distanceCutoff = 0.1f;

    void Start()
    {
        currentTarget = Pos1;
        if (!bounce) {
            transform.position = Pos1;
            currentTarget = Pos2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying) {
            if (setPos1) {
                setPos1 = false;
                Pos1 = transform.position;
            }
            if (setPos2) {
                setPos2 = false;
                Pos2 = transform.position;
            }
            return;
        }
        currentWaitTime -= Time.deltaTime;
        if (finished || currentWaitTime > 0)
            return;

        if (Vector3.Distance(transform.position, currentTarget) < distanceCutoff) {
            if (!bounce) {
                finished = true;
                return;
            }

            currentTarget = currentTarget == Pos1 ? Pos2 : Pos1;
            currentWaitTime = currentTarget == Pos1 ? waitTimeAtPos2 : waitTimeAtPos1;
            if (currentWaitTime > 0)
                return;
        }

        switch (moveType) {
            case MoveType.linear:
                var dir = (currentTarget - transform.position).normalized;
                transform.position = transform.position + (dir * (linearSpeed * Time.deltaTime));
                break;
            case MoveType.lerp:
                transform.position = Vector3.Lerp(transform.position, currentTarget, lerpSpeed);
                break;
            default:
                break;
        }
    }
}
