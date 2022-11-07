using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NumberNode_obj", menuName = "ScriptableObject/Game/NumberNode", order = int.MaxValue)]
public class NumberNode_obj : ScriptableObject
{
    uint number = 0;
    internal uint Number
    {
        get
        {
            if (number == 0)
                number = (uint)Random.Range(1, 10);
            return number;
        }
        set { number = value; }
    }
}
