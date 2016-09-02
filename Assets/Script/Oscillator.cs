using UnityEngine;

public class Oscillator : MonoBehaviour {

    public bool OscillationEnabled;

    float amp = 0.3f;
    float period = 2f;

    Vector3 startPos = Vector3.zero;

    void Start () {
        startPos = transform.position;
    }

    void Update () {
        if ( OscillationEnabled ) {
            Oscillate ();
        }
    }

    void Oscillate () {
        transform.position = startPos + Vector3.up * ( amp *  Mathf.Sin ( Time.timeSinceLevelLoad / period ) );
    }
}
