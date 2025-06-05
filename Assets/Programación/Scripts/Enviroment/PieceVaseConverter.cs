using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceVaseConverter : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Collider collider;
    private float waitTime = 0.5f;
    private float elapsedtime = 0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        elapsedtime += Time.deltaTime;
        if(elapsedtime >= waitTime)
        {
            if (rigidbody != null)
                if (rigidbody.velocity == Vector3.zero)
                {
                    rigidbody.useGravity = false;
                    if (collider != null)
                        collider.enabled = false;
                }
        }
    }
}
