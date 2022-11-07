using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    uint width;
    int position;
    internal int Position => position;
    public int[] PositionArray
    {
        get
        {
            int[] temp = { X, Y };
            return temp;
        }
        set
        {
            position = value[1] * (int)width + value[0];
        }
    }
    public int X => position / (int)width;
    public int Y => position % (int)width;

    public Point() { }
    public Point(int[] xy) { PositionArray = xy; }

    //������ �����ε�
    public static Point operator +(Point thisP, Point otherP)
    {
        int[] temp = { thisP.X + otherP.X, thisP.Y + otherP.Y };
        thisP.PositionArray = temp;
        return thisP;
    }
    public static Point operator -(Point thisP, Point otherP)
    {
        int[] temp = { thisP.X - otherP.X, thisP.Y - otherP.Y };
        thisP.PositionArray = temp;
        return thisP;
    }
}
