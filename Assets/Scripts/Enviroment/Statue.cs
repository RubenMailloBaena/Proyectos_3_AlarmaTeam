using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, IStatue
{
    [Tooltip("Punto en el que se produce el tp")]
    [SerializeField] private Transform teleportPoint;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;

    public void ShowUI(bool show)
    {
        if (show)
            renderer.material = visualMaterial;
        else
            renderer.material = defaultMaterial;
    }

    public Vector3 GetTPPoint()
    {
        return teleportPoint.position;
    }
}
