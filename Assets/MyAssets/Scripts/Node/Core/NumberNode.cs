using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newmoonhana.HADEngine;
using TMPro;

public class NumberNode : MonoBehaviour
{
    [SerializeField] internal NumberNode_obj data;

    [SerializeField] internal BoxCollider2D col;
    [SerializeField] internal SpriteRenderer sr;
    [SerializeField] internal SpriteRenderer outline_sr;
    [SerializeField] internal RectTransform text_tns;
    [SerializeField] internal TextMeshPro text_tmp;

    internal Vector2 position;

    public void ChangeSize(Vector2 _size)
    {
        sr.size = _size;
        outline_sr.size = _size;
        text_tns.sizeDelta = _size;
        col.size = _size;
    }

    internal void TouchedDropEffect()   //들었을 때
    {
        //z 위치 바뀌어보이는 효과
        sr.sortingOrder = 1;
        outline_sr.sortingOrder = 1;
        text_tmp.sortingOrder = 1;
    }
    internal void UnTouchedDropEffect()   //놓았을 때
    {
        //z 위치 바뀌어보이는 효과
        sr.sortingOrder = 0;
        outline_sr.sortingOrder = 0;
        text_tmp.sortingOrder = 0;
        //좌표 변경
        Debug.Log(position);
        transform.localPosition = position;
    }
}
