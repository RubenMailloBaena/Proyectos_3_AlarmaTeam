using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, IStatue, IVisible
{
    [Tooltip("Punto en el que se produce el tp")]
    [SerializeField] private Transform teleportPoint;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectMaterial;

    void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
    }
    public void ShowUI(bool show)
    {
        if (show)
            renderer.material = selectMaterial;
        else
            renderer.material = defaultMaterial;
    }

    public Vector3 GetTPPoint()
    {
        return teleportPoint.position;
    }

    public Quaternion GetTPRotation()
    {
        return Quaternion.LookRotation(teleportPoint.forward);
    }

    public void SetVisiblity(bool active)
    {
        if (active)
            renderer.material = visualMaterial;
        else
            renderer.material = defaultMaterial;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
}
