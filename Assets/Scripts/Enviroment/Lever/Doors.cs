using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour, IObjects
{
    [SerializeField] private Transform moveObject;
    [SerializeField] private Transform cablePosition;
    [SerializeField] private GameObject visualSelect;
    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f;

    private Vector3 startingPos, finalPos;
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
        Vector3 targetPos = isMoving ? finalPos : startingPos;
        moveObject.position = Vector3.MoveTowards(moveObject.position, targetPos, Time.deltaTime * speed);
        if (moveObject.position != targetPos)
        {
            ShowInteract(false);
        }
    }
    
    public void Interact()
    {
        isMoving = !isMoving;
    }

    public void ShowInteract(bool interact)
    {
        visualSelect.SetActive(interact);
    }

    public Vector3 GetCablePosition()
    {
        return cablePosition.position;
    }
}
