using UnityEngine;

public class PatrolAgent : MonoBehaviour {
    SteeringAgent agent;

    Vector3 velocity = Vector3.zero;

    void Start () {
        agent = GetComponent<SteeringAgent>();
		if (agent == null) {
            gameObject.AddComponent<SteeringAgent>();
        }
    }

	void Update () {

        Quaternion rot = GetRotationDegrees(transform.position, MousePointer.Pos());
        velocity = GetVelocityIncrement(velocity, Time.deltaTime);

        transform.position += velocity;
        transform.rotation = rot;
        transform.DrawHeading(velocity);
    }

    /* Returns a Vector representing amount of velocity to add to total velocity. This velocity should be applied to a target position to move the transform. */
    Vector3 GetVelocityIncrement ( Vector3 currentVel, float tElapsed ) {
        Vector3 sForce = agent.Wander(); // TODO: Steering.Calculate() ??
        Vector3 accel = sForce / agent.Mass; // acceleration = force / mass
        currentVel += accel * tElapsed; // velocity += acceleration * TimeElapsed

        return currentVel.Truncate(agent.MaximumSpeed); // pos += trunc(velocity) * TimeElapsed
    }

	Quaternion GetRotationDegrees ( Vector3 facing, Vector3 target ) {
        Vector3 heading = (target - facing).normalized;
        float theta = Mathf.Acos ( Vector3.Dot ( Vector3.right, heading ) ) * Mathf.Rad2Deg;

        if ( heading.y < 0 ) return Quaternion.Euler ( 0f, 0f, 270f - theta ); 
        else return Quaternion.Euler ( 0f, 0f, theta - 90f );
    }
}
