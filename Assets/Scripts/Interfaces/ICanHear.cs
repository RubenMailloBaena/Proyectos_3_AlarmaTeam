using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanHear
{
    public void HeardSound(Vector3 soundPoint, bool isObject);
}
