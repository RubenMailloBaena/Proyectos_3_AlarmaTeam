using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour, IVisible
{
    [SerializeField] private GameObject heart;

    private void Start()
    {
        AddVisible();
    }

    public void AddVisible()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
    }

    public void SetVisiblity(bool active)
    {
        if(heart != null)
            heart.SetActive(active);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
