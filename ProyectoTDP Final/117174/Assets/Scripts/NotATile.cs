using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotATile 
{
    private Vector2 location;
    private int value;

    public Vector2 getLocation()
    {
        return location;
    }
    public int getValue()
    {
        return value;
    }

    public void setLocation(Vector2 loc)
    {
        location = loc;
    }
    public void setValue(int v)
    {
        value = v;
    }
}
