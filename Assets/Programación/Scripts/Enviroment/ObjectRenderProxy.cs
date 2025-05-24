using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRenderProxy : MonoBehaviour, IProxy
{
    [SerializeField] private MonoBehaviour logicObject;
    
    public IObjects GetObject()
    {
        return logicObject as IObjects;
    }
}
