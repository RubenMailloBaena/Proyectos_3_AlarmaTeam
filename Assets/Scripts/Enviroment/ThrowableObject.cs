using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour, IInteractable, IVisible
{
    private PlayerController pController;

    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectMaterial;

    [SerializeField] private Renderer renderer;

    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float soundRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Space(10)] [SerializeField] private float interactDistance;
    public float InteractDistance => interactDistance;
    public bool isLocked { get; set; }

    private bool thrown, done; 
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
    }

    public void SelectObject(bool select)
    {
        if (thrown) return;

        if (select)
            renderer.material = selectMaterial;
        else
            renderer.material = defaultMaterial;
    }

    public  void Interact()
    {
        if (thrown) return;
        
        thrown = true;
        renderer.material = defaultMaterial;
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!thrown || done) return;
        
        if (other.transform.CompareTag("Ground"))
        {
            done = true;
            CheckIfEnemiesCanHear();
        }
    }

    private void CheckIfEnemiesCanHear()
    {
        foreach (IEnemyInteractions enemy in pController.GetEnemies())
        {
            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if(distance <= soundRadius)
                enemy.HeardSound(transform.position, true);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
    
    public Vector3 GetPosition() => transform.position;


    #region Cheat
    
    public void ResetObject()
    {
        thrown = false;
        done = false;

        if (renderer != null)
            renderer.material = defaultMaterial;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void SetVisiblity(bool active)
    {
        if (active)
            renderer.material = visualMaterial;
        else
            renderer.material = defaultMaterial;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
    #endregion
}
