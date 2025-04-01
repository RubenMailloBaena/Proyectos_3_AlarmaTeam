using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, IStatue
{
    [Tooltip("Punto en el que se produce el tp")]
    [SerializeField] private Transform teleportPoint;

    [Tooltip("Icono o Elemento que se muestra cuando el jugador puede hacer tp")]
    [SerializeField] GameObject uiIcon;

    public void ShowUI(bool show)
    {
        if(uiIcon != null)
            uiIcon.SetActive(show);
    }

    public Vector3 GetTPPoint()
    {
        return teleportPoint.position;
    }
}
