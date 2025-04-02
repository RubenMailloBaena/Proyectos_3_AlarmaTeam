using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThrowableObject : MonoBehaviour, IThrowableObject
{
    [SerializeField] private GameObject interactVisual;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float soundRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private bool thrown, done; 
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SelectObject(bool select)
    {
        if (thrown) return;
        
        interactVisual.SetActive(select);
    }

    public void ThrowObject()
    {
        if (thrown) return;
        
        thrown = true;
        interactVisual.SetActive(false);
        
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius, enemyLayer);

        if (hitColliders.Length == 0) return;
        
        foreach (Collider enemy in hitColliders)
        {
            if (enemy.TryGetComponent(out ICanHear enemyController))
                enemyController.HeardSound(transform.position, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}
