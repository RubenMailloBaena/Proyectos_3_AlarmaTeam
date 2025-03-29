using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackStab : MonoBehaviour
{

    [Header("Backstab Attributes")] 
    [SerializeField] private float backstabRadius = 2f;
    
    private SphereCollider backstabCollider;

    void Awake()
    {
        backstabCollider = GetComponent<SphereCollider>();
    }

    void Start()
    {
        backstabCollider.radius = backstabRadius;
    }

    
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            print(other.name);
        }
    }
}
