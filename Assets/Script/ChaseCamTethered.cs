using UnityEngine;

    /*
    ==========
    =========
    the amount of extension is the amount of pull -- hooke
    force = -spring_tension_constant * displacement_vector - DampingConstant_constant * rel_velocity_between_two_points

    inverse square law:
    intensity is inversely proportional to the square of the distance from the source of that intensity 
    basically: intensity diminishes the further it is from origin

    potential energ (pe) = (1/2 * k * x^2)
    ==========
    =========
    */

public class ChaseCamTethered : MonoBehaviour {

	// TODO: look into the concept of a 'camera rig'

    [TooltipAttribute("~ set in editor -- this is the target the camera will chase")]
    public Transform ChaseTarget;

    [TooltipAttribute("~ max velocity the camera is allowed to achieve when chasing")]
    [RangeAttribute(0.1f, 1000.0f)]
    public float CameraMaximumChaseSpeed = 10.0f;
    [TooltipAttribute("~ amount of displacement between chase target and cam until chase kicks in")]
    [RangeAttribute(0.1f, 500.0f)]
    public float DelayRadius             = 5.0f;
    [TooltipAttribute("~ kill cam velocity and set to zero when within this radius (centered at chase target origin)")]
    [RangeAttribute(0.1f, 1.0f)]
    public float KillChaseRadius         = 0.5f;

    Vector3 camCurrVelocity             = Vector3.zero;
    float killChaseRadiusSqr { get { return KillChaseRadius * KillChaseRadius; } }

    void Update () {
        if ( ChaseTarget ) {
            Vector3 displacement = PosWithCamDepth ( ChaseTarget.position ) - PosWithCamDepth ( transform.position );
            float dSqr = displacement.sqrMagnitude;
            if ( dSqr <= killChaseRadiusSqr ) {
                camCurrVelocity = Vector3.zero;
            } else {
                float d = Mathf.Sqrt ( dSqr );
                if ( d > DelayRadius ) {
                    camCurrVelocity += displacement.normalized * CameraMaximumChaseSpeed;
                } else {
                    if (d == 0) {
                        d = 0.01f;
                    }
                    camCurrVelocity += ( displacement * camCurrVelocity.magnitude ) / d;
                }
            }
            camCurrVelocity = camCurrVelocity.Truncate(CameraMaximumChaseSpeed);
            transform.position += ( camCurrVelocity * Time.deltaTime );
        }
    }

    Vector3 PosWithCamDepth ( Vector3 target, float zTarget = -10.0f ) {
        return new Vector3 ( target.x, target.y, zTarget );
    }
}
