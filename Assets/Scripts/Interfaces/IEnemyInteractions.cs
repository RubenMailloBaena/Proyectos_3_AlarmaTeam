using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyInteractions
{
    public void HeardSound(Vector3 soundPoint, bool isObject);
    public void SetWeakSpot(bool active);
    public void Backstab();
    public Vector3 GetPosition();
    public Transform GetTransform();
}
