using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberLine : MonoBehaviour
{
    [SerializeField] internal SpriteRenderer sr;
    [SerializeField] internal SpriteRenderer bg_sr;

    public void ChangeSize(Vector2 _size)
    {
        sr.size = _size;
        bg_sr.size = _size;
    }
}
