using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour, IVisible
{
    [SerializeField] private GameObject heart;
    public void SetVisiblity(bool active)
    {
        if(heart != null)
            heart.SetActive(active);
    }
}
