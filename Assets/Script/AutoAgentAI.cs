using UnityEngine;

public class AutoAgentAI : AutoAgent {
	public float ms = 5.0f;
	Transform target = null;
	MoveController mc = new MoveController ();
    bool hasTarget = false;

    void Update () {
		if (target && hasTarget) {
			if ( mc.MoveUntilArrived(gameObject.transform, target.position, ms, Time.deltaTime)) {
                hasTarget = false;
            }
            mc.RotateUntilFacingTarget(gameObject.transform, target.position);
        } else {
            target = gameObject.Closest( AutoAgent.Entities ).transform;
			if (target) {
                hasTarget = true;
            }
        }
	}
}
