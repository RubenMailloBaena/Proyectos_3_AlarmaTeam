using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoorTrigger : MonoBehaviour
{
    [SerializeField] private LevelDoor door;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            door.PlayerOnTrigger();
    }
}
