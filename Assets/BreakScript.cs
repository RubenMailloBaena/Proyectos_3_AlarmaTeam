using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fractured;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Instantiate(fractured, transform.position, Quaternion.identity);
            fractured.SetActive(true);
            Destroy(gameObject);
        }
    }
}

