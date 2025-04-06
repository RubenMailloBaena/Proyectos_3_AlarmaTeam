using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    [SerializeField] private Transform moveObject;
    [SerializeField] private GameObject visualSelect;
    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f;

    private Vector3 startingPos, finalPos, targetPos;
    private bool isMoving;
    
    void Start()
    {
        startingPos = moveObject.position;
        finalPos = startingPos + moveObject.forward * distance;
    }
    
    void Update()
    {
        PerformInteraction();
    }
    
    private void PerformInteraction()
    {
        targetPos = isMoving ? finalPos : startingPos;
        moveObject.position = Vector3.MoveTowards(moveObject.position, targetPos, Time.deltaTime * speed);
    }
    
    public void Interact()
    {
        isMoving = !isMoving;
    }

    public void ShowInteract(bool interact)
    {
        if (moveObject.position != targetPos)
            interact = false;
        visualSelect.SetActive(interact);
    }
}
