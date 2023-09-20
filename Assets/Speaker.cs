using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] float speakingRange, repeatResetTime = 0.5f;
    float repeatCooldown;
    [SerializeField] GameObject buttonPrompt;
    [SerializeField] Conversation conversation;

    Transform player;
    bool talking;

    private void Start()
    {
        player = GameManager.instance.player.transform;
    }

    private void Update()
    {
        if (talking && !GameManager.instance.talking) {
            repeatCooldown -= Time.deltaTime;
            if (repeatCooldown <= 0) {
                repeatCooldown = repeatResetTime;
                talking = false;
            }
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        bool displayPrompt = dist < speakingRange && !GameManager.instance.talking;

        buttonPrompt.SetActive(displayPrompt);
        if (displayPrompt && Input.GetKeyDown(KeyCode.E) && !talking) {
            conversation.StartConvo();
            talking = true;
            GameManager.instance.StartConversation(conversation, transform);
        }
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, speakingRange);
    }
}
