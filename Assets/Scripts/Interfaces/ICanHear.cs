using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanHear
{
    public void HeardObject(Vector3 soundPoint);
    public void HeardPlayer();
}
