using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum VaultOptions{
    OneVault,
    FurtherOne,
    CloseOne
}

public class VaultObject : MonoBehaviour, ICanVault
{
    [SerializeField] private VaultOptions vaultOption;
    [SerializeField] private Transform vaultEndPoint1;
    [SerializeField] private Transform vaultEndPoint2;

    public VaultOptions VaultOption => vaultOption;

    public Vector3 GetVaultEndPoint(Vector3 playerPosition)
    {
        switch (vaultOption)
        {
            case VaultOptions.OneVault: return vaultEndPoint1.position;
            case VaultOptions.CloseOne: return GetTargetVault(playerPosition, false);
            default: return GetTargetVault(playerPosition, true);
        }
    }

    private Vector3 GetTargetVault(Vector3 playerPosition, bool getFurther)
    {
        float dist1 = Vector3.Distance(playerPosition, vaultEndPoint1.position);
        float dist2 = Vector3.Distance(playerPosition, vaultEndPoint2.position);

        if (getFurther)
        {
            if (dist1 >= dist2) return vaultEndPoint1.position;
            return vaultEndPoint2.position;
        }
        
        if (dist1 <= dist2) return vaultEndPoint1.position;
        return vaultEndPoint2.position;
    }
}
    
