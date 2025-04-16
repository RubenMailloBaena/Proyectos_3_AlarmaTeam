using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Transform enemyLocation;

    [SerializeField] private Transform playerObject;

    [SerializeField] private Transform indicatorImagePivot;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 direction = (enemyLocation.position - playerObject.position).normalized;
        float angle = (Vector3.SignedAngle(direction, playerObject.forward, Vector3.up));
        indicatorImagePivot.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
