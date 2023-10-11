using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) Directory.player.AddMoney(100);
    }
}
