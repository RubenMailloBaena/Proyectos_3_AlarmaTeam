using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    void Start()
    {
  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Pressing B");
            var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Jugador/jugador_habilidad_control_mental");
            instance.start();
        }
    }
}
