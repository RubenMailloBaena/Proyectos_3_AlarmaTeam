using System;
using System.Collections;
using UnityEngine;


public class BridgeController : MonoBehaviour, IInteractuable
{
    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f; 

    private Vector3 startingPos, finalPos;

    private bool isMoving;

    void Start()
    {
        startingPos = transform.position;
        finalPos = startingPos + transform.forward * distance;
    }


    void Update()
    {
        Interact();
        PerformInteraction();
    }

    private void PerformInteraction()
    {
        Vector3 targetPos = isMoving ? finalPos : startingPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
    }



    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isMoving = !isMoving;
            Debug.Log(isMoving);
        }
    }

    public void ShowInteract()
    {
        throw new System.NotImplementedException();
    }
}