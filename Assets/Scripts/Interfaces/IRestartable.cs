using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestartable 
{
    void RestartGame();
    void RestartFromCheckPoint();
    void SetCheckPoint(Transform checkpoint);
}
