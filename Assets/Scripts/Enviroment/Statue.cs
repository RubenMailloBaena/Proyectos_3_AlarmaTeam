using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Statue : MonoBehaviour, IStatue, IVisible
{
    [Tooltip("Punto en el que se produce el tp")]
    [SerializeField] private Transform teleportPoint;

    [SerializeField] private Renderer statueRenderer;
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
            statueRenderer.material = selectMaterial;
        else
            statueRenderer.material = defaultMaterial;
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
            statueRenderer.material = visualMaterial;
        else
            statueRenderer.material = defaultMaterial;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
    
    private void OnDestroy() => GameManager.GetInstance().GetPlayerController().RemoveVisible(this);
}
