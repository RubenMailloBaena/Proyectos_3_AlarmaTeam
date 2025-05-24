using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharmEnemy
{
    public void SetTargetVisual(bool active);
    public bool IsInChaseOrAttack();
    public bool IsCharmed();
    public List<IInteractable> ActivateIntarectables();
    public void ClearIntarectables();
    public void SetCharmedState(IInteractable lever);
    public Vector3 GetPosition();
}
