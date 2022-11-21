using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberLine : MonoBehaviour
{
    internal uint goalCal;
    uint cal;
    [SerializeField] internal BoxCollider2D col;
    [SerializeField] internal SpriteRenderer sr;
    [SerializeField] internal SpriteRenderer outline_sr;
    [SerializeField] internal RectTransform goalCal_tns;
    [SerializeField] internal TextMeshPro goalCal_txt;
    [SerializeField] internal RectTransform cal_tns;
    [SerializeField] internal TextMeshPro cal_txt;

    public void ChangeSize(Vector2 _size)
    {
        sr.size = _size;
        outline_sr.size = _size;
        Vector2 sizeDelta = Vector2.one;
        sizeDelta.x = _size.x;
        goalCal_tns.sizeDelta = sizeDelta * 10;
        cal_tns.sizeDelta = sizeDelta * 10;
        col.size = _size;
    }
}
