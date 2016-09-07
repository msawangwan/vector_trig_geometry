using UnityEngine;
using System.Collections.Generic;

public class AutoAgent : MonoBehaviour {

    public bool IsPlayerController;

    public static List<AutoAgent> Agents = new List<AutoAgent>();
    public static List<GameObject> Entities = new List<GameObject>();

    public Vector3 Pos { get { return transform.position; } }
    public bool Tagged { get; set; }

    void Awake () {
        Agents.Add(this);
        Entities.Add(gameObject);
        Tagged = false;
    }
}