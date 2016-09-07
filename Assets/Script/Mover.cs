using UnityEngine;
using System.Collections.Generic;

public class Mover : MonoBehaviour {

    public Vector3 Pos { get { return transform.position; } }

    void Update () {
        Go();
    }

    void Go () {
        Vector3 p = Pos;
        Vector3 c = Closest(p, AutoAgent.Entities).transform.position;
    }

    GameObject Closest (Vector3 pos, List<GameObject> entities) {
        GameObject closest = null;
        float dMin = Mathf.Infinity;
        int i = 0;

        while (i < entities.Count) {
            GameObject curr = entities[i];
            Debug.Log("checking for closest: ", curr);
            if (curr == this) continue;
            float dSqr = (curr.transform.position - pos).sqrMagnitude;
            if (dSqr < dMin) {
                dMin = dSqr;
                closest = curr;
                Debug.Log("updating closest: ", curr);
            }
            i++;
        }

        return closest;
    }
}
