using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStatesTextRotate : MonoBehaviour
{
    private Transform player;
    
    private void Start()
    {
        player = GameManager.GetInstance().GetPlayerController().transform;
    }

    void Update()
    {
        transform.LookAt(transform.position + player.forward);
    }
}
