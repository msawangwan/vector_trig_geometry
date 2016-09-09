using UnityEngine;

    /*
    ==============================================
    ==========================================
        spring force is usually calculated using Hookes Law:
        F = -kx - bv
        force = -spring_tension_constant * displacement_vector - DampingConstant_constant * rel_velocity_between_two_points
    ==============================================
    ==========================================
    */

public class ElasticChaseCam : MonoBehaviour {

    class State {

        public float t; // timeElapsed
        public float dt; // deltaTime

        public Vector3 d = Vector3.zero; // displacement
        public Vector3 f = Vector3.zero; // force
        public Vector3 v = Vector3.zero; // velocity

        public Vector3 RestPosition = Vector3.zero;
        public Vector3 CurrentPosition = Vector3.zero;

        public State () { }

        public State ( Vector3 restPosition ) {
            RestPosition = restPosition;
        }

        public State( float time, float deltatime ) { 
            t = time; dt = deltatime; 
        }

        public State( float time, float deltatime, Vector3 displacement, Vector3 velocity ) {
            t = time; dt = deltatime; d = displacement; v = velocity;
        }
    }

    public Transform ChaseTarget;

    [RangeAttribute(10,1000)]
    public float StepSizeAccumulator = 150f;
    [RangeAttribute(1,10)]
    public float SpringConstant = 6.1f; // higher values = tighter
    [RangeAttribute(0.1f,1.0f)]
    public float DampingConstant = 0.15f;

    const int xOffset = 0;
    const int yOffset = 0;
    const int zOffset = 0;

    const float zDepth = -10f;

    Vector3 camRestPos      = Vector3.zero; // original vertex
    Vector3 camTargetPos    = Vector3.zero; // chase target pos
    Vector3 camDisplacement = Vector3.zero;
    Vector3 camVelocity     = Vector3.zero; // vertex velocity

    float pointForce = 2.0f;

    State state = null;
    State initialState = null;

    void Start () {
        initialState = new State (0f, Time.deltaTime);
    }

    void Update () {

        if (ChaseTarget == null) {
            return;
        }

        camRestPos = transform.position;
        camTargetPos = ChaseTargetCameraPosition(ChaseTarget.position);

        if (state == null) {
            state = initialState;
        }

        Vector3 displacement = camTargetPos - camRestPos;

        if (state.d != displacement){
            state.d = displacement;
            state = SpringVelocity(state, SpringConstant, DampingConstant);
        } else {
            state = initialState;
            state.d = Vector3.zero;
        }

        transform.position += state.v;
        transform.DrawLineToTarget(ChaseTarget.position); // DEBUG
    }

    State SpringVelocity (State state, float tension, float DampingConstant) {
        State nextState = new State();

        state.t += Time.deltaTime;
        if (state.t >= StepSizeAccumulator) state.t = 0f;

        nextState.t = state.t;
        nextState.v = state.d.Truncate(tension) * DampingConstant; // TODO: refactor
        nextState.v -= (state.d / (nextState.t + 1f)).Truncate(tension) * DampingConstant * Time.deltaTime * Time.deltaTime;
        nextState.d = state.d;

        return nextState;
    }

    /* force = -spring_tension_constant * displacement_vector - DampingConstant_constant * rel_velocity_between_two_points */
    /*  */
    /* 
        F = -k(|x|-d)(x/|x|) - bv
        F = -spring_constant * (d - desired_d) * (dir_normalized) - DampingConstant * rel_vel
        |x| = distance between two points connected by spring 
        d = desired distance of seperation (offset??)
        x / |x| = direction normalized between a and b when applying the force to point a
    */
    Vector3 SpringForce (float springConstant, float dampConstant) {
        Vector3 displacement = camTargetPos - camRestPos;
        Vector3 dir = displacement.normalized;
        float dDesired = 1.0f;
        float d = displacement.magnitude;

        return ( ( -1 * springConstant ) * ( d - dDesired )  * dir ) - ( dampConstant * displacement );

        //Vector3 minus_kx = (-1 * displacement.normalized) * tensionConstant;
        //Vector3 minus_bv = ()
    }

    Vector3 ChaseTargetCameraPosition (Vector3 target) {
        return new Vector3(target.x + xOffset, target.y + yOffset, zDepth + zOffset);
    }
}