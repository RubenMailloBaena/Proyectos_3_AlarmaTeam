using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;


public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movement Attributes")] 
    [SerializeField] private float playerSpeed = 5f;

    private CharacterController charController;
    private Vector3 direction;
    
    private void Start()
    {
        charController = GetComponent<CharacterController>();
    }


    
}
