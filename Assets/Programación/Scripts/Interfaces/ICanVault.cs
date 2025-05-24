using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanVault
{
    public Vector3 GetVaultEndPoint(Vector3 playerPosition);
}
