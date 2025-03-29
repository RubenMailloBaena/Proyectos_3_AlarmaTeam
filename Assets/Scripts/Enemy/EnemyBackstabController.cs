using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBackstabController : MonoBehaviour, ICanBackstab
{
    [SerializeField] private GameObject weakSpotRenderer;
    
    public void SetWeakSpot(bool active)
    {
        weakSpotRenderer.SetActive(active);
    }

    public void Backstab()
    {
        Destroy(gameObject);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
