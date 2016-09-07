using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerAgent : MonoBehaviour {

    public Vector3 Position { get { return transform.position;  } }
    
    Vector3 targetPosition { get; set; }
    Vector3 lerpPosition { get; set; }
    Vector3 toPosition { get; set; }

    float tStart { get; set; }
    float tElapsed { get; set; }
    float t { get; set; }
    float dt { get { return Time.deltaTime; } }

    void Start () {
        tStart = 1.0f;
    }

    void Update () {

        if (Input.GetMouseButton (0)) {
            //targetPosition = MousePointer.Pos();
            //float d = (pos - Pos).magnitude;
            //float tLerp = Time.time - tStart * dt/ d;

            //n = Vector3.Lerp(Pos, pos, 5.0f);

            Action<Vector3> cb = (toPosition) => {
                lerpPosition = toPosition;
            };

            StartCoroutine(Go(Position, targetPosition, 1.0f, cb).GetEnumerator());
            Debug.Log(lerpPosition + " AKJDLKSAD");
            transform.position = lerpPosition;
        }
    }

    void poop(Vector3 aa) {
        Debug.Log("AAA: " + aa.Stringify2());
    }

    IEnumerator retVect (System.Action<Vector3> cb) {
        yield return new WaitForSeconds(3.0f);
        cb(Position);
    }

    IEnumerable<Vector3> Go(Vector3 s, Vector3 e, float dur, Action<Vector3> callback) {
        float i = 0.0f;
        float rate = 1.0f / dur; // check for 0

        while (i < 1.0f) {
            i += dt * rate;
            yield return Vector3.Lerp(s, e, i);
        }
    }
}
