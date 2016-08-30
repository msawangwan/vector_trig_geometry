using UnityEngine;

public class Lerper : MonoBehaviour {

	void FollowMouseSmoothDamp ( GameObject self, float transitionSpeed = 0.25f, float z = 10.0f ) {

		Vector3 p1 = self.transform.position;
		Vector3 p2 = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, z ) );
		Vector3 vel = Vector3.zero;

		self.transform.position = Vector3.SmoothDamp ( p1, p2, ref vel, transitionSpeed );
	}

	void FollowMouseLerp ( GameObject self, float transitionSpeed = 0.5f, float startTime = 2.5f, float z = 10.0f ) {

		Vector3 p1 = self.transform.position;
		Vector3 p2 = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, z ) );
	
		float t = ( ( Time.time - startTime ) ) * transitionSpeed;
		float d = Distancef ( p1, p2 );

		self.transform.position = Vector3.Lerp ( p1, p2, ( t / d ) );
	}

	/* d = sqrt ( (x2-x1) + (y2-y1) ) */
	float Distancef ( Vector3 a, Vector3 b ) {
		return Mathf.Sqrt( ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) ) );
	}
}
