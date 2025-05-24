using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISeeArrow
{
    bool IsActive { get; set; }
    void SetTarget(Transform target);
    void SetActive(bool show);
    void UpdateArrow(float amount, float maxCapacity);
    void PlayerSeen();
    void StopAll();
}
