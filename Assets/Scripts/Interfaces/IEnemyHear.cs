using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyHear
{
    public void HeardSound(Vector3 soundPoint, bool isObject);
    public Vector3 GetPosition();

}
