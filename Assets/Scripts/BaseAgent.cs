using UnityEngine;

public class BaseAgent : MonoBehaviour {

    public bool ClickToFollow = true;

    Vector3 acceleration = Vector3.zero; // acceleration = force / mass
    Vector3 velocity = Vector3.zero; // velocity += acceleration * TimeElapsed
    Vector3 pos = Vector3.zero; // pos += trunc(velocity) * TimeElapsed;

    void Update ( ) {
        if (Input.GetMouseButton(0) || ClickToFollow == false) {
            Vector3 targetPos = MousePointer.Pos ();
            Quaternion rot = ToTargetRot (transform.position, targetPos);
            //Vector3 pos 
            transform.rotation = rot;
        }
    }

    Vector3 ToTargetPos ( Vector3 pos, Vector3 target ) {
        return target - pos;
    }

    /* Returns a Quaternion representing the rotation to apply to rotate from current facing to facing target. */
    Quaternion ToTargetRot ( Vector3 facing, Vector3 target ) {
        Vector3 heading = (target - facing).normalized;
        float theta = Mathf.Acos ( Vector3.Dot ( Vector3.right, heading ) ) * Mathf.Rad2Deg;

        if ( heading.y < 0 ) return Quaternion.Euler ( 0f, 0f, 270f - theta ); 
        else return Quaternion.Euler ( 0f, 0f, theta - 90f );
    }

	/* magnitude = d = sqrt ( (x2-x1) + (y2-y1) ) */
	float Distancef ( Vector3 a, Vector3 b ) {
		return Mathf.Sqrt ( ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) ) );
	}

	float sqrDistancef ( Vector3 a, Vector3 b ) {
		return  ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) );
	}

	void PaintRay2DHeading () {
		Debug.DrawRay ( transform.position, transform.up * 3.0f, Color.yellow, 0.2f, false );
	}
}
