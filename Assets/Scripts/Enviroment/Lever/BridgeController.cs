using System;
using System.Collections;
using UnityEngine;
public class BridgeController : MonoBehaviour, IObjects
{
    [SerializeField] private GameObject meshBlock1;
    [SerializeField] private GameObject meshBlock2;
    [SerializeField] private Transform movingObject;
    [SerializeField] private Transform cablePosition;

    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Renderer bridgeRenderer;

    [SerializeField] private float distance = 5f; 
    [SerializeField] private float speed = 5f;

    private Vector3 startingPos, finalPos;
    private bool isMoving;
    private bool isExtending; 

    private float distanceToConsiderArrive = 0.1f; 
    
    public IInteractable lever { get; set; }
    public Material lockedMaterial { get; set; }


    void Start()
    {
        startingPos = movingObject.position;
        finalPos = startingPos + movingObject.forward * distance;
        
        meshBlock1.SetActive(true);
        meshBlock2.SetActive(true);
    }

    void Update()
    {
        PerformInteraction();
    }

    private void PerformInteraction()
    {
        Vector3 targetPos = isMoving ? finalPos : startingPos;
        movingObject.position = Vector3.MoveTowards(movingObject.position, targetPos, Time.deltaTime * speed);

        float distanceToTarget = Vector3.Distance(movingObject.position, targetPos);
        
        if (isMoving)
        {
            if (isExtending && distanceToTarget > distanceToConsiderArrive)
            {
                meshBlock1.SetActive(true);
                meshBlock2.SetActive(true);  
            }
            
            if (isExtending && distanceToTarget <= distanceToConsiderArrive)
            {
                meshBlock1.SetActive(false);
                meshBlock2.SetActive(false); 
            }
        }
        else
        {
            if (!isExtending && distanceToTarget > distanceToConsiderArrive)
            {
                meshBlock1.SetActive(true);  
                meshBlock2.SetActive(true); 
            }
        }
    }


    public void Interact()
    {
        isMoving = !isMoving;
        isExtending = isMoving; 
    }

    public void ShowInteract(bool interact, bool locked)
    {
        if (interact)
        {
            if (locked)
                bridgeRenderer.material = lockedMaterial;
            else
                bridgeRenderer.material = visualMaterial;
        }
        else
            bridgeRenderer.material = defaultMaterial;
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
