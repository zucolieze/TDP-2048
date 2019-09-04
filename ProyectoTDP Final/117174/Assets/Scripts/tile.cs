using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour
{
    public int tileValue;
    public bool merged;

    public Vector2 startPos;
    public Vector2 moveToPos;

    public bool destroyMe = false;
    public bool mergedWhithCollidingTile;
    public Transform collidingTile;

    public Animation animacion;

}
