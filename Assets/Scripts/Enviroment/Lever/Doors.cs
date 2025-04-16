using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour, IObjects
{
    [SerializeField] private Transform moveObject;
    [SerializeField] private Transform cablePosition;

    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Renderer doorRenderer;

    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f;

    private Vector3 startingPos, finalPos;
    private bool isMoving;
    
    public IInteractable lever { get; set; }
    public Material lockedMaterial { get; set; }
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
    }


    public void Interact()
    {
        isMoving = !isMoving;
    }

    public void ShowInteract(bool interact, bool locked)
    {
        if (interact)
        {
            if(locked)
                doorRenderer.material = lockedMaterial;
            else
                doorRenderer.material = visualMaterial;
        }
            
        else
            doorRenderer.material = defaultMaterial;
    }
    
    public void SetLocked(bool active)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetCablePosition()
    {
        return cablePosition.position;
    }

    
}
