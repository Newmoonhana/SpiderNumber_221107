using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newmoonhana.HADEngine;

public class NumberNode : MonoBehaviour
{
    [SerializeField] internal NumberNode_obj data;

    [SerializeField] internal SpriteRenderer sr;
    [SerializeField] internal SpriteRenderer bg_sr;
    [SerializeField] internal RectTransform text_tns;

    public void ChangeSize(Vector2 _size)
    {
        sr.size = _size;
        bg_sr.size = _size;
        text_tns.sizeDelta = _size;
    }
}
