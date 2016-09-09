using UnityEngine;

    /* force = -spring_tension_constant * displacement_vector - DampingConstant_constant * rel_velocity_between_two_points */
    /* inverse square law:
        intensity is inversely proportional to the square of the distance from the source of that intensity 
        basically grows weaker the further out */

    /* the amount of extension is the amount of pull -- hooke*/

    /*potential energ (pe) = (1/2 * k * x^2) */

public class ElasticTetherCamera : MonoBehaviour {

    public Transform ChaseTarget;

    [RangeAttribute(1f, 10f)]
    public float ElasticityConstant = 6.1f; // higher values = tighter
    [RangeAttribute(0.1f, 100.0f)]
    public float DampingConstant = 0.15f;
    [RangeAttribute(0.1f, 100.0f)]
    public float ForceConstant = 5.0f;
    public float MassConstant = 1.0f;
    public float CameraMaximumPanSpeed = 10.0f;
    public float ThresholdMax = 10.0f;
    public float ThresholdMin = 3.0f;

    Vector3 camCurrentPos         = Vector3.zero;
    Vector3 camPrevPos            = Vector3.zero;
    Vector3 camCurrVelocity       = Vector3.zero;
    Vector3 camPrevVelocity       = Vector3.zero;
    Vector3 camAcceleration       = Vector3.zero;

    Vector3 chaseTargetCurrentPos = Vector3.zero;
    Vector3 chaseTargetPrevPos    = Vector3.zero;

    float speed { get { return camCurrVelocity.magnitude; } }
    float speedSqr { get { return camCurrVelocity.sqrMagnitude; } }

    float minThresholdSqr { get { return ThresholdMin * ThresholdMin; } }
    float maxThresholdSqr { get { return ThresholdMax * ThresholdMax; } }

    bool isLerping = false;

    void Update () {
        if (isLerping) {
            Debug.Log("lerp move" + Time.time);
            isLerping = ShouldCamLerp ( CurrentPositionConverted(transform.position), chaseTargetCurrentPos );
            return;
        }

        float dt = Time.deltaTime;

        camCurrentPos = CurrentPositionConverted (transform.position);
        chaseTargetCurrentPos = CurrentPositionConverted (ChaseTarget.position);

        camCurrVelocity = (camCurrentPos - camPrevPos) / dt;

        camAcceleration += (camCurrVelocity - camPrevVelocity) / dt;
        Vector3 displacement = chaseTargetCurrentPos - camCurrentPos;

        float dSqr = displacement.sqrMagnitude;

        if ( dSqr < minThresholdSqr || dSqr > maxThresholdSqr ) {
            transform.position += Vector3.zero;
            camCurrVelocity = Vector3.zero;
            camAcceleration = Vector3.zero;
            isLerping = true;
        } else {
            //transform.position += displacement.normalized + a.Truncate(CameraMaximumPanSpeed)  * dt;
            Vector3 velocityTowardsTarget = Vector3.Project(displacement.normalized, camAcceleration);
            Debug.Log("non lerp v " + velocityTowardsTarget);
            transform.position += velocityTowardsTarget * dt;
        }
    }

    bool ShouldCamLerp (Vector3 s, Vector3 e) {
        Vector3 d = e - s;
        if (d == Vector3.zero) {
            return false;
        }
        float ratio = Time.deltaTime / d.magnitude;
        transform.position = Vector3.Lerp(s, e, ratio);
        return true;
    }

    Vector3 CurrentPositionConverted (Vector3 target, float zTarget = -10.0f) {
        return new Vector3(target.x, target.y, zTarget);
    }
}
