using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour
{
    [SerializeField] private bool isFinalDoor;
    
    [Header("REFERENCES")]
    [SerializeField] private LevelDoorPart leftDoor;
    [SerializeField] private LevelDoorPart rightDoor;
    [SerializeField] private Transform leftHinge;
    [SerializeField] private Transform rightHinge;
    
    [Header("MATERIALS")]
    [SerializeField] private Material selectedMat;

    [Header("ATTRIBUTES")]
    [SerializeField] private float openDoorAngle = 75f;
    [SerializeField] private float openDoorSpeed = 2f;
    [SerializeField] private float angleToInteract = 80f;
    [Space(10)] [SerializeField] private float interactDistance = 5f;

    private Quaternion leftClosedRotation, rightClosedRotation;
    private Quaternion leftOpenRotation, rightOpenRotation;
    
    private bool doorIsLocked;
    private bool isOpen = false;
    private bool isMoving = false;
    private bool unloadPreviousLvl;
    
    private void Awake()
    {
        if(isFinalDoor)
            GameManager.GetInstance().SetFinalDoor(this);
        
        leftDoor.SetDoorPart(this, interactDistance, selectedMat);
        rightDoor.SetDoorPart(this, interactDistance, selectedMat);
    }

    private void Start()
    {
        leftClosedRotation = leftHinge.localRotation;
        rightClosedRotation = rightHinge.localRotation;

        leftOpenRotation = leftClosedRotation * Quaternion.Euler(0, -openDoorAngle, 0);
        rightOpenRotation = rightClosedRotation * Quaternion.Euler(0, openDoorAngle, 0);
    }

    private void Update()
    {
        if (!isMoving) return;

        Quaternion targetLeft = isOpen ? leftOpenRotation : leftClosedRotation;
        Quaternion targetRight = isOpen ? rightOpenRotation : rightClosedRotation;

        leftHinge.localRotation = Quaternion.Lerp(leftHinge.localRotation, targetLeft, Time.deltaTime * openDoorSpeed);
        rightHinge.localRotation = Quaternion.Lerp(rightHinge.localRotation, targetRight, Time.deltaTime * openDoorSpeed);

        // Si ya estamos cerca del destino, paramos el movimiento
        if (Quaternion.Angle(leftHinge.localRotation, targetLeft) < 0.5f
            && Quaternion.Angle(rightHinge.localRotation, targetRight) < 0.5f)
        {
            isMoving = false;
            if (unloadPreviousLvl)
            {
                unloadPreviousLvl = false;
                LevelChangeManager.GetInstance().UnloadPreviousLevel();
            }
        }
    }

    public void SelectObjects(bool select)
    {
        if (isMoving || doorIsLocked)
        {
            ChangeSelected(false);
            return;
        }
        if (select && CanInteract())
            ChangeSelected(true);
        else
            ChangeSelected(false);
    }

    private void ChangeSelected(bool selected)
    {
        if (leftDoor == null || rightDoor == null) return;
        
        leftDoor.ChangeSelected(selected);
        rightDoor.ChangeSelected(selected);
    }

    public bool CanInteract()
    {
        Vector3 playerPos = GameManager.GetInstance().GetPlayerController().GetPlayerPosition();
        Vector3 dir = (playerPos - transform.position).normalized;
        return Vector3.Angle(transform.forward, dir) <= angleToInteract;
    }
    
    public void PlayerOnTrigger()
    {
        isOpen = false;
        isMoving = true;

        if (!isFinalDoor)
            unloadPreviousLvl = true;
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        isMoving = true;

        if (isOpen && isFinalDoor)
            LevelChangeManager.GetInstance().LoadNextLevel();
    }

    public void UnlockDoor()
    { 
        doorIsLocked = false;
    }

    public void LockDoor()
    {
        doorIsLocked = true;
    } 
    
    private void OnDestroy() => GameManager.GetInstance().RemoveFinalDoor();
}
