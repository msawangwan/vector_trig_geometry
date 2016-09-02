using UnityEngine;

[RequireComponent(typeof(TransformInterpolator))]
public class ChaseCam : MonoBehaviour {
    const int xPerimeter = 5;
    const int yPerimeter = 5;
    float t = 0f;
    bool isChase = false;
    public Transform ChaseTarget;
    TransformInterpolator interpolator;
    Vector3 p = Vector3.zero;
    Vector3 vel = Vector3.zero;
    void Start () {

        interpolator = GetComponent<TransformInterpolator>();
		if (interpolator == null) {
            interpolator = gameObject.AddComponent<TransformInterpolator>();
        }

        interpolator.isSlerpEnabled = false;
        interpolator.isLerpEnabled = true;
        interpolator.isInterpolating = true;
    }
	void Update() {
        Debug.Log(ChaseTarget.localPosition.x);
        Debug.Log(ChaseTarget.position.x);
        if (ChaseTarget.localPosition.x > xPerimeter || ChaseTarget.localPosition.y > yPerimeter) {
            isChase = true;
        } else {
            isChase = false;
        }

		if (isChase) {
            p = new Vector3(ChaseTarget.position.x, ChaseTarget.position.y, -10);
            transform.position = Vector3.SmoothDamp(transform.position, p, ref vel, 0.25f);
        }
    }
}
