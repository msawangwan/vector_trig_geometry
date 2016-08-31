using UnityEngine;

public class BaseAgent : MonoBehaviour {
    public bool ClickToFollow = true;
    void Update ( ) {
        if (Input.GetMouseButton(0) || ClickToFollow == false) {
            Vector3 target = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, 10.0f ) );
            target.z = 0;
            Vector3 heading = target - transform.position;

            float dot = Vector3.Dot ( Vector3.right, heading.normalized );
            float theta = Mathf.Acos ( dot ) * Mathf.Rad2Deg;
            float angle = 0f;

            if ( heading.y < 0 ) {
                angle = 270f - theta;
            } else {
                angle = theta - 90f;
            }

            Debug.Log(theta + "\n"  + angle);
            Quaternion rot = Quaternion.Euler ( 0f, 0f, angle);

            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            transform.rotation = rot;
        }
    }

	/* magnitude = d = sqrt ( (x2-x1) + (y2-y1) ) */
	float Distancef ( Vector3 a, Vector3 b ) {
		return Mathf.Sqrt ( ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) ) );
	}

    Vector3 VectorFromTo ( Vector3 a, Vector3 b ) {
        return b - a;
    }

    Vector3 VectorFromToNorm ( Vector3 a, Vector3 b ) {
        return ( b - a ).normalized;
    }

	void PaintRay2DHeading () {
		Debug.DrawRay ( transform.position, transform.up * 3.0f, Color.yellow, 0.2f, false );
	}
	
	void PaintRay2DAxisX () {
		Debug.DrawRay ( transform.position, transform.right * 10.0f, Color.yellow, 3.0f, false );
        Debug.DrawRay ( transform.position, transform.right * -10.0f, Color.yellow, 3.0f, false );
	}
}
