using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProxy //esta en los colliders de los objetos, los renders, para que podamos obtenerlo mediante el raycast
{
    IObjects GetObject();
}
