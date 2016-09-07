using UnityEngine;
using System;
using System.Collections;

public class PlayerAgent : MonoBehaviour {

    public Vector3 Position { get { return transform.position;  } }

    Vector3 targetPosition { get; set; }
    Vector3 lerpPosition { get; set; }
    Vector3 toPosition { get; set; }
    Vector3 velocity { get; set; }
    Vector3 heading { get { return toPosition.normalized; } }

    float speed { get { return velocity.magnitude; } }
    float maxSpeed { get { return 3.0f; } }

    float tStart { get; set; }
    float tElapsed { get; set; }
    float t { get; set; }
    float dt { get { return Time.deltaTime; } }

    void Start () {
        tStart = 1.0f;
    }

    void Update () {

        if (Input.GetMouseButton (0)) {
            targetPosition = MousePointer.Pos();
            toPosition = targetPosition - Position;

            float d = toPosition.magnitude;
            float s = d / dt;
        }
    }

    IEnumerator Go(Vector3 s, Vector3 e, float dur) {
        float i = 0.0f;
        float rate = 1.0f / dur; // check for 0

        while (i < 1.0f) {
            i += dt * rate;
            transform.position = Vector3.Lerp(s, e, Mathf.SmoothStep(0.0f,1.0f, i));
            yield return null;
        }
    }
}
