using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    [SerializeField] private Transform moveObject;

    [SerializeField] private Color visualColor;
    [SerializeField] private Outline outlineScript;

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
        AudioManager.Instance.HandlePlaySound3D("event:/Ambiente/ambiente_cortina_abriendo", transform.position);
        isMoving = !isMoving;
    }

    public void ShowInteract(bool interact, bool locked, Color lockedColor)
    {
        if (moveObject.position != targetPos)
            interact = false;
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
