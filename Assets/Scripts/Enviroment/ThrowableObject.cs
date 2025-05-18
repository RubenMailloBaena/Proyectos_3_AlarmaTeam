using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowableObject : MonoBehaviour, IInteractable, IVisible, IRestartable
{
    private PlayerController pController;

    [SerializeField] private Color visionColor;
    [SerializeField] private Color selectColor;

    [SerializeField] private Outline outlineScript;

    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float soundRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Space(10)] [SerializeField] private float interactDistance;

    public float InteractDistance => interactDistance;
    public bool isLocked { get; set; }
    public bool canInteract { get; set; }

    private bool thrown, done, selecting; 
    private Rigidbody rb;

    public GameObject notFractured;
    public GameObject fracturedPrefab;
    private GameObject fracturedInstance;

    //RESTART
    private Vector3 startingPos, checkpointPos;
    private Quaternion startingRotation, checkpointRotation;
    private bool wasUsed;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        canInteract = true;
    }

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
        GameManager.GetInstance().AddRestartable(this);

        startingPos = transform.position;
        checkpointPos = startingPos;
        startingRotation = transform.rotation;
        checkpointRotation = startingRotation;
    }

    public void SelectObject(bool select)
    {
        if (thrown) return;

        if (select)
        {
            outlineScript.enabled = true;
            outlineScript.OutlineMode = Outline.Mode.OutlineVisible;
            outlineScript.OutlineColor = selectColor;
            selecting = true;
        }
        else
        {
            outlineScript.enabled = false;
            selecting = false;
        }
    }

    public  void Interact()
    {
        if (thrown) return;
        
        thrown = true;
        outlineScript.enabled = false;
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!thrown || done) return;
        
        if (other.transform.CompareTag("Ground"))
        {
            ShowFractured(true);
            done = true;
            CheckIfEnemiesCanHear();
            //LO CAMBIAMOS DE LAYER PARA NO PODER VOLVER A INTERACTUAR
            gameObject.layer = LayerMask.NameToLayer("Logic");

        }
    }
    private void CheckIfEnemiesCanHear()
    {
        foreach (IEnemyHear enemy in pController.GetHearEnemies())
        {
            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if(distance <= soundRadius)
                enemy.HeardSound(transform.position, true);
        }
    }

    private void ShowFractured(bool active)
    {
        if (active)
        {
            notFractured.SetActive(false);
            fracturedInstance = Instantiate(fracturedPrefab, transform.position, transform.rotation);
            fracturedInstance.SetActive(true);
        }
        else
        {
            if (fracturedInstance != null)
                Destroy(fracturedInstance);
            notFractured.SetActive(true);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
    
    public Vector3 GetPosition() => transform.position;
    
    //VISION
    public void SetVisiblity(bool active)
    {
        if (selecting) return;
        
        if (active)
        {
            outlineScript.enabled = true;
            outlineScript.OutlineColor = visionColor;
            outlineScript.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }
        else
            outlineScript.enabled = false;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }

    //CHECKPOINTS
    public void RestartGame()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        ShowFractured(false);
        transform.position = startingPos;
        transform.rotation = startingRotation;
        checkpointPos = startingPos;
        checkpointRotation = startingRotation;
        thrown = false;
        done = false;
        wasUsed = false;
    }

    public void RestartFromCheckPoint()
    {
        transform.position = checkpointPos;
        transform.rotation = checkpointRotation;
        thrown = wasUsed;
        done = wasUsed;
        if (!wasUsed)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            ShowFractured(false);
        }
    }

    public void SetCheckPoint()
    {
        checkpointPos = transform.position;
        checkpointRotation = transform.rotation;
        wasUsed = thrown;
    }
    
    private void OnDestroy()
    { 
        GameManager.GetInstance().GetPlayerController().RemoveVisible(this);
        GameManager.GetInstance().RemoveRestartable(this);
    }
}