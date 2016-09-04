using UnityEngine;

public class VectorPhysics : MonoBehaviour {

	public class EulerState {
        public Vector3 Position;
        public Vector3 Velocity;
        public float t;
        public float dt;
    }

	/* f = ma  */
	/* a = f/m */

	/* dv/dt = a = f/m */
	/* dx/dt = v */

	/* call while t <= some value ie, 10 */
	VectorPhysics.EulerState IntegrateEuler (VectorPhysics.EulerState state, Vector3 force, float mass) {

		VectorPhysics.EulerState newState = new VectorPhysics.EulerState();

        newState.Position += state.Velocity * state.dt;
        newState.Velocity += (force / mass) * state.dt;
        newState.t += state.dt;

        return newState;
    }

	/* it al boils down to 'something = something + change_in_something * change_in_time'  */

    const float k = 10f; // spring constant
    const float b = 1f;

    const float stepZero = 0.0f;
    const float stepHalf = 0.5f;
    const float stepEnd = 1.0f;

    public struct RK4State {
        public float x; // position
        public float v; // velocity
    }

	public struct RK4Derivative {
        public float dx; // dx/dt = change in velocity
        public float dv; // dv/dt = change in acceleration
    }

	/* the acceleration function drives the RK4 algorithm */
	/* calculates a spring and dampner effect and returns is as the acceleration assuming unit mass */
	float RK4Acceleration (VectorPhysics.RK4State state, float t) {
		return ( (-1 * k) * state.x ) - ( b * state.v );
    }

	/* advance the physics state from t to t+dt using one set of derivatives */
	VectorPhysics.RK4Derivative EvaluateRK4 (VectorPhysics.RK4State state, VectorPhysics.RK4Derivative der, float t, float dt) {

		VectorPhysics.RK4State nextState;
        nextState.x = state.x + der.dx * dt;
        nextState.v = state.v + der.dv * dt;

        VectorPhysics.RK4Derivative output;
        output.dx = nextState.v;
        output.dv = RK4Acceleration(nextState, t + dt);

        return output;
    }

	VectorPhysics.RK4State IntegrateRK4 (VectorPhysics.RK4State state, float t, float dt) {

        RK4State nextState;
        RK4Derivative a, b, c, d;

        d.dx = 0.0f;
        d.dv = 0.0f;

        a = EvaluateRK4(state, d, t, stepZero);
        b = EvaluateRK4(state, a, t, dt * stepHalf);
        c = EvaluateRK4(state, b, t, dt * stepHalf);
        d = EvaluateRK4(state, c, t, stepEnd);

        float dxdt = 1.0f / 6.0f * (a.dx + 2.0f * (b.dx + c.dx) + d.dx);
        float dvdt = 1.0f / 6.0f * (a.dv + 2.0f * (b.dv + c.dv) + d.dv);

        nextState.x = state.x + dxdt * dt;
        nextState.v = state.v + dvdt * dt;

        return nextState;
    }
}
