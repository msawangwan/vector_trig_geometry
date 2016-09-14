using UnityEngine;

    /*
    ==============================================
    ==========================================
        spring force is usually calculated using Hookes Law:
        F = -kx - bv
        force = -spring_tension_constant * displacement_vector - DampingConstant_constant * rel_velocity_between_two_points

        i haven't been able to apply it successfully yet and instead have come up with my own formulaz
    ==============================================
    ==========================================
    */

public class ChaseCamElastic : MonoBehaviour {

	// TODO: look into the concept of a 'camera rig'

    [TooltipAttribute("~ set in editor -- this is the target the camera will chase")]
    public Transform ChaseTarget;

    [TooltipAttribute("~ higher values result in a 'stiffer' feel")]
    [RangeAttribute(1,10)]
    public float SpringConstant  = 1.0f;
    [TooltipAttribute("~ higher values result in a 'stiffer' feel")]
    [RangeAttribute(0.1f,1.0f)]
    public float DampingConstant = 0.1f;

    float dtSqr { get { return Time.deltaTime * Time.deltaTime; } }

    float ticker                 = 0.0f;
    Vector3 displacement        = Vector3.zero;
    Vector3 prevDisplacement    = Vector3.zero;
    Vector3 velocity            = Vector3.zero;

    void Update () {
        if ( ChaseTarget ) {
            displacement = ChaseTargetCameraPosition(ChaseTarget.position) - transform.position;
            if ( displacement == prevDisplacement ) {
                displacement = Vector3.zero;
            } else {
                ticker += Time.deltaTime;
                if ( ticker >= 10.0f ) {
                    ticker = 0.0f;
                }
                velocity = displacement.Truncate(SpringConstant) * DampingConstant;
                velocity -= ( displacement / ( ticker + 1.0f ) ).Truncate ( SpringConstant ) * DampingConstant * dtSqr;
            }
            prevDisplacement = displacement;
            transform.position += velocity;
        }
    }

    Vector3 ChaseTargetCameraPosition (Vector3 target, float zDepth = -10.0f) {
        return new Vector3(target.x, target.y, zDepth);
    }
}