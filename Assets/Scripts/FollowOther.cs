using UnityEngine;

public class FollowOther : MonoBehaviour {

	public bool isFollowEnabled;
	public bool followMouse;

	/* d = sqrt ( (x2-x1) + (y2-y1) ) = magnitude */
	float Distancef ( Vector3 a, Vector3 b ) {
		return Mathf.Sqrt ( ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) ) );
	}

	void PaintRay2DHeading () {
		Debug.DrawRay ( transform.position, transform.up * 3.0f, Color.yellow, 0.2f, false );
	}
	
	void PaintRay2DAxisX () {
		Debug.DrawRay ( transform.position, transform.right * 10.0f, Color.yellow, 5.0f, false );
	}
}
