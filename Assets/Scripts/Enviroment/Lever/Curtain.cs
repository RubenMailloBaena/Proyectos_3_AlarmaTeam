using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    [SerializeField] private Transform moveObject;

    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Renderer curtainRenderer;

    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f;

    private Vector3 startingPos, finalPos, targetPos, restartPosition;
    private bool isMoving, wasMoving;
    
    void Start()
    {
        startingPos = moveObject.position;
        restartPosition = startingPos;
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

    public void ShowInteract(bool interact, bool locked, Material lockedMat)
    {
        if (moveObject.position != targetPos)
            interact = false;
        if (interact)
        {
            if(locked)
                curtainRenderer.material = lockedMat;
            else 
                curtainRenderer.material = visualMaterial;
        }
        else
            curtainRenderer.material = defaultMaterial;
    }
    
    public void RestartGame()
    {
        moveObject.position = startingPos;
        restartPosition = startingPos;
        isMoving = false;
        wasMoving = false;
    }

    public void RestartFromCheckPoint()
    {
        moveObject.position = restartPosition;
        isMoving = wasMoving;
    }

    public void SetCheckPoint()
    {
        restartPosition = moveObject.position;
        wasMoving = isMoving;
    }
}
