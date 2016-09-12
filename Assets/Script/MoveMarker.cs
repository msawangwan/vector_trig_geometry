using UnityEngine;

public class MoveMarker : MonoBehaviour {
    public Vector3 Position { get { return transform.position; } set { Position = value; } }
    Oscillator oscillator = null;

	void Start () {
        oscillator = new Oscillator ();
    }

    void Update () {
		if (gameObject.activeInHierarchy) {
            oscillator.Oscillate(transform, transform.position);
        }
	}
}
