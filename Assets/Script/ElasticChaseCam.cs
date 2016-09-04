using UnityEngine;

    /* spring force is usually calculated using Hookes Law:
        Force_Vector = -damping_Constant * Displacement_Vector */

public class ElasticChaseCam : MonoBehaviour {

    class State {

        public float t; // timeElapsed
        public float dt; // deltaTime

        public Vector3 d = Vector3.zero; // displacement
        public Vector3 v = Vector3.zero; // velocity

        public State () { }
        public State(float time, float deltatime) { t = time; dt = deltatime; }
        public State(float time, float deltatime, Vector3 displacement, Vector3 velocity) {
            t = time; dt = deltatime; d = displacement; v = velocity;
        }
    }

    public Transform ChaseTarget;

    [RangeAttribute(10,1000)]
    public float StepSizeAccumulator = 150f;
    [RangeAttribute(1,10)]
    public float SpringTension = 6.1f; // higher values = tighter
    [RangeAttribute(0.1f,1.0f)]
    public float Damping = 0.15f;

    const int xOffset = 0;
    const int yOffset = 0;
    const int zOffset = 0;

    const float zDepth = -10f;

    Vector3 camRestPos = Vector3.zero; // original vertex
    Vector3 camTargetPos = Vector3.zero; // chase target pos

    State state = null;
    State initialState = null;

    void Start () {
        initialState = new State (0f, Time.deltaTime);
    }

    void Update () {

        camRestPos = transform.position;
        camTargetPos = ChaseTargetCameraPosition(ChaseTarget.position);

        if (state == null) {
            state = initialState;
        }

        Vector3 displacement = camTargetPos - camRestPos;

        if (state.d != displacement){
            state.d = displacement;
            state = SpringVelocity(state, SpringTension, Damping);
        } else {
            state = initialState;
            state.d = Vector3.zero;
        }

        transform.position += state.v;
        transform.DrawLineToTarget(ChaseTarget.position); // DEBUG
    }

    State SpringVelocity (State state, float tension, float damping) {

        State nextState = new State();

        state.t += Time.deltaTime;
        if (state.t >= StepSizeAccumulator) state.t = 0f;

        nextState.t = state.t;
        nextState.v = state.d.Truncate(tension) * damping; // TODO: refactor
        nextState.v -= (state.d / (nextState.t + 1f)).Truncate(tension) * damping * Time.deltaTime * Time.deltaTime;
        nextState.d = state.d;

        return nextState;
    }

    Vector3 ChaseTargetCameraPosition (Vector3 target) {
        return new Vector3(target.x + xOffset, target.y + yOffset, zDepth + zOffset);
    }
}