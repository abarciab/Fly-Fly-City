using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] float speakingRange = 2.3f, repeatResetTime = 0.5f;
    float repeatCooldown;
    [SerializeField] GameObject buttonPrompt;
    [SerializeField] Conversation conversation;

    Transform player;
    bool talking;
    [SerializeField] bool advanceDeliveryOnConvoComplete, repeat;

    [Header("Animation")]
    [SerializeField] Animator anim;
    [SerializeField] string talkingBool = "talking";

    [Header("Camera")]
    [SerializeField] Vector3 headPos;
    [SerializeField] Vector3 camPos, standPos;

    [HideInInspector] public bool currentlyTalking;

    private void Start()
    {
        player = Directory.gMan.player.transform;
    }

    public Vector3 getHeadPos()
    {
        return transform.TransformPoint(headPos);
    }

    public Vector3 GetCamPos()
    {
        return transform.TransformPoint(camPos);
    }

    public Vector3 GetStandPos()
    {
        return transform.TransformPoint(standPos);
    }

    private void Update()
    {
        if (anim) anim.SetBool(talkingBool, talking && currentlyTalking);

        if (talking && !Directory.gMan.talking) {
            repeatCooldown -= Time.deltaTime;
            if (repeatCooldown <= 0) {
                repeatCooldown = repeatResetTime;
                talking = false;
            }
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        bool displayPrompt = dist < speakingRange && !Directory.gMan.talking;

        buttonPrompt.SetActive(displayPrompt);
        if (displayPrompt && Input.GetKeyDown(KeyCode.E) && !talking) {
            StartConversation();
        }
    }

    void StartConversation()
    {
        conversation.StartConvo();
        talking = true;
        Directory.gMan.StartConversation(conversation, this);
        Directory.gMan.player.FaceSpeaker(transform);
    }

    public void EndConversation()
    {
        if (anim) anim.SetBool(talkingBool, false);
        if (advanceDeliveryOnConvoComplete) Directory.fMan.AdvanceDeliveryStage();
        if (!repeat) enabled = false;
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, speakingRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(headPos), 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(camPos), 0.5f);
        Gizmos.DrawWireSphere(transform.TransformPoint(standPos), 0.5f);
    }
}
