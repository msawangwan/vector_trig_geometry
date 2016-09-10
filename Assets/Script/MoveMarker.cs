using UnityEngine;

public class MoveMarker : MonoBehaviour {
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
