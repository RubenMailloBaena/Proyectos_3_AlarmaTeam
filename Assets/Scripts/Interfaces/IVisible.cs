using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisible
{
    public void AddVisible();
    public void SetVisiblity(bool active);
    public Vector3 GetPosition();
}
