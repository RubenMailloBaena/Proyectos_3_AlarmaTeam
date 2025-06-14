using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour, IObjects, IRestartable
{
    [SerializeField] private Transform moveObject;
    [SerializeField] private Transform cablePosition;

    [SerializeField] private Color visualColor;
    [SerializeField] private Outline outlineScript;

    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f;

    private Vector3 startingPos, finalPos, restartPosition;
    private bool isMoving, wasMoving;
    
    public IInteractable lever { get; set; }
    public Color lockedColor { get; set; }
    void Start()
    {
        GameManager.GetInstance().AddRestartable(this);
        
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
        Vector3 targetPos = isMoving ? finalPos : startingPos;
        moveObject.position = Vector3.MoveTowards(moveObject.position, targetPos, Time.deltaTime * speed);
    }


    public void Interact()
    {
        isMoving = !isMoving;
    }

    public void ShowInteract(bool interact, bool locked)
    {
        if (interact)
        {
            outlineScript.enabled = true;
            if (locked)
                outlineScript.OutlineColor = lockedColor;
            else
                outlineScript.OutlineColor = visualColor;
        }
        else
            outlineScript.enabled = false;
    }
    
    public Vector3 GetCablePosition()
    {
        return cablePosition.position;
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

    private void OnDestroy()
    {
        GameManager.GetInstance().RemoveRestartable(this);
    }
}
