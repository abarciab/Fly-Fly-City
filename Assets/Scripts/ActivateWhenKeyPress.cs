using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivateWhenKeyPress : MonoBehaviour
{
    [SerializeField] UnityEvent onPress;
    [SerializeField] KeyCode keyPress = KeyCode.Tab;

    private void Update()
    {
        if (Input.GetKeyDown(keyPress)) {
            onPress.Invoke();
        }
    }
}
