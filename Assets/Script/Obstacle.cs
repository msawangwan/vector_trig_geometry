using UnityEngine;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{

    public static List<Obstacle> Obstacles = new List<Obstacle>();

    public bool IsTagged { get; set; }
    public float BRadius;

    void Start () {
        Obstacles.Add(this);
        IsTagged = false;
    }
}
