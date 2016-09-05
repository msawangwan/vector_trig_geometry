using UnityEngine;

public class BaseAgent : MonoBehaviour {

    public bool ClickToFollow = true;

    Vector3 velocity = Vector3.zero; // velocity += acceleration * TimeElapsed <- TODO: use the steeringagent velocity
    Quaternion rotation = Quaternion.identity;

    SteeringAgent steeringController;
    public SteeringAgent a;
    public SteeringAgent b;

    void Start () {
        steeringController = GetComponent<SteeringAgent>();
        if (steeringController == null) {
            gameObject.AddComponent<SteeringAgent>();
        }
    }

    void Update () {
        if (Input.GetMouseButton(0) || ClickToFollow == false) {
            Vector3 targetPos = MousePointer.Pos ();

            rotation = GetRotationDegrees (transform.position, targetPos);
            velocity = GetVelocityIncrement (velocity, targetPos, Time.deltaTime);

            transform.rotation = rotation;
            transform.position += velocity;
            transform.DrawLocalAxis ();
            transform.DrawHeading(velocity);
        }

        if (Input.GetMouseButton(1) || ClickToFollow == false) {
            //Vector3 targetPos = a.Position;
            Vector3 targetPos = MousePointer.Pos();

            rotation = GetRotationDegrees (transform.position, targetPos);
            velocity = GetFleeVelocityIncrement (velocity, targetPos, Time.deltaTime);

            transform.rotation = rotation;
            transform.position += velocity;
            transform.DrawLocalAxis ();
            transform.DrawHeading(velocity);
        }
    }

    /* velocity is calculated by the numerical integration:  v_1 = v_0 + f / m * dt */

    /* Returns a Vector representing amount of velocity to add to total velocity. This velocity should be applied to a target position to move the transform. */
    Vector3 GetVelocityIncrement ( Vector3 currentVel, Vector3 targetPos, float tElapsed ) {
        Vector3 sForce = steeringController.Seek ( targetPos ); // TODO: Steering.Calculate() ??
        //Vector3 sForce = steeringController.AvoidObstacle(Obstacle.Obstacles); // TODO: Steering.Calculate() ??
        Vector3 accel = sForce / steeringController.Mass; // acceleration = force / mass
        currentVel += accel * tElapsed; // velocity += acceleration * TimeElapsed

        return currentVel.Truncate(steeringController.MaximumSpeed); // pos += trunc(velocity) * TimeElapsed
    }

    /* Returns a Vector representing amount of velocity to add to total velocity. This velocity should be applied to a target position to move the transform. */
    Vector3 GetFleeVelocityIncrement ( Vector3 currentVel, Vector3 targetPos, float tElapsed ) {
        //Vector3 sForce = steeringController.Interpose ( a, b ); // TODO: Steering.Calculate() ??
        //Vector3 sForce = steeringController.Seek ( targetPos ); // TODO: Steering.Calculate() ??
        Vector3 sForce = steeringController.Hide(a, Obstacle.Obstacles);
        Vector3 accel = sForce / steeringController.Mass; // acceleration = force / mass
        currentVel += accel * tElapsed; // velocity += acceleration * TimeElapsed

        return currentVel.Truncate(steeringController.MaximumSpeed); // pos += trunc(velocity) * TimeElapsed
    }

    /* Returns a Quaternion representing the rotation to apply to rotate from current facing to facing target. */
    Quaternion GetRotationDegrees ( Vector3 facing, Vector3 target ) {
        Vector3 heading = (target - facing).normalized;
        float theta = Mathf.Acos ( Vector3.Dot ( Vector3.right, heading ) ) * Mathf.Rad2Deg;

        if ( heading.y < 0 ) return Quaternion.Euler ( 0f, 0f, 270f - theta ); 
        else return Quaternion.Euler ( 0f, 0f, theta - 90f );
    }
}
