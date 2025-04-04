using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] private Transform finalpoint;

    private Vector3 initialpoint;

    void Start()
    {
        transform.position = initialpoint;
    }



    void Update()
    {
        
    }
}
