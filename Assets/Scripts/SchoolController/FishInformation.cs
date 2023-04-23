using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishInformation
{
    public Vector3 Direction
    {
        private set;
        get;
    }
    public float Speed
    {
        private set;
        get;
    }
    public float PreySearchingRange
    {
        private set;
        get;
    }
    public FishInformation(Vector3 direction, float speed, float preySearchingRange)
    {
        Direction = direction;
        Speed = speed;
        PreySearchingRange = preySearchingRange;
    }
}