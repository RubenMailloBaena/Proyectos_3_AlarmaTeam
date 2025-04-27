using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [SerializeField] private bool isFinalDoor;
    
    [Header("REFERENCES")]
    [SerializeField] private LevelDoorPart leftDoor;
    [SerializeField] private LevelDoorPart rightDoor;
    
    [Header("MATERIALS")]
    [SerializeField] private Material selectedMat;

    [SerializeField] private float angleToInteract = 80f;
    [Space(10)] [SerializeField] private float interactDistance = 5f;

    private bool doorIsLocked;
    
    private void Awake()
    {
        if(isFinalDoor)
            GameManager.GetInstance().SetFinalDoor(this);
        
        leftDoor.SetDoorPart(this, interactDistance, selectedMat);
        rightDoor.SetDoorPart(this, interactDistance, selectedMat);
    }
    
    public void SelectObjects(bool select)
    {
        if (doorIsLocked) return;
        
        if (select && CanInteract())
        {
            leftDoor.ChangeSelected(true);
            rightDoor.ChangeSelected(true);
        }
        else
        {
            leftDoor.ChangeSelected(false);
            rightDoor.ChangeSelected(false);
        }
    }

    public bool CanInteract()
    {
        Vector3 playerPos = GameManager.GetInstance().GetPlayerController().GetPlayerPosition();
        Vector3 dir = (playerPos - transform.position).normalized;
        return Vector3.Angle(transform.forward, dir) <= angleToInteract;
    }
    
    public void PlayerOnTrigger()
    {
        LockDoor();
        CloseDoor();
    }

    public void OpenDoor()
    {
        
    }

    public void CloseDoor()
    {
        
    }

    public void UnlockDoor() => doorIsLocked = false;
    public void LockDoor() => doorIsLocked = true;

}
