using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyOneInstance : MonoBehaviour
{
    private void Awake()
    {
        if(GameManager.GetInstance().LevelHasNoEventSystem())
            GameManager.GetInstance().SetEventSystem(this);
        else if (GameManager.GetInstance().LevelHasNoLight())
            GameManager.GetInstance().SetLight(this);
        else
            Destroy(gameObject);
    }
}
