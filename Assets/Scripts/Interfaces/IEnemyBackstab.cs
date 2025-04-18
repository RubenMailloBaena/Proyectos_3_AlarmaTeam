using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBackstab
{
    public void SetWeakSpot(bool active);
    public void Backstab();
    public Transform GetTransform();
}
