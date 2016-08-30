using UnityEngine;

public class FollowOther : MonoBehaviour {

	public bool isFollowEnabled;
	public bool followMouse;

	float startTime = 0.0f;

	bool isInterpolating = false;

	Vector3 startpoint = Vector3.zero;
	Vector3 endpoint = Vector3.zero;

	void Update() {

		if ( isFollowEnabled && Input.GetMouseButtonDown ( 0 ) ) {
			Interpolate ();
		}

		if ( isInterpolating ) {
			if ( CalculateInterpolation ( gameObject.transform, startpoint, endpoint, startTime ) ) {
				isInterpolating = false;
			}
		}
	}

	void PaintRay2D () {
		Debug.DrawRay ( transform.position, transform.up * 3.0f, Color.yellow, 0.2f, false );
	}

	void FollowMouseSmoothDamp ( GameObject self, float transitionSpeed = 0.25f, float z = 10.0f ) {

		PaintRay2D ();

		Vector3 p1 = self.transform.position;
		Vector3 p2 = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, z ) );
		Vector3 vel = Vector3.zero;

		self.transform.position = Vector3.SmoothDamp ( p1, p2, ref vel, transitionSpeed );
	}

	void FollowMouseLerp ( GameObject self, float transitionSpeed = 0.5f, float startTime = 2.5f, float z = 10.0f ) {

		Vector3 p1 = self.transform.position;
		Vector3 p2 = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, z ) );
	
		float t = ( ( Time.time - startTime ) ) * transitionSpeed;
		float d = Distance ( p1, p2 );

		self.transform.position = Vector3.Lerp ( p1, p2, ( t / d ) );
	}

	void Interpolate () {

		isInterpolating = true;
		startTime = Time.time;

		startpoint = transform.position;
		endpoint = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, 10.0f ) );
	}

	bool CalculateInterpolation ( Transform self, Vector3 start, Vector3 end, float t ) {

		PaintRay2D ();

		float percent = ExpStep ( TimeStep ( t ) );
		self.position = Vector3.Lerp ( start, end, percent );

		if ( percent >= 1.0f ) return true;
		else return false;
	}

	/* t = currentLerpTime / lerpTime */
	float TimeStep ( float t ) {
		return ( Time.time - t ) / 1.0f;
	}

	/* t = t * t * ( 3 - ( 2 * t ) ) */
	float SmoothStep ( float t ) {
		return ( t * t ) * ( 3.0f - ( 2.0f * t ) );
	}

	float SinStep ( float t ) {
		return Mathf.Sin ( t * Mathf.PI * 0.5f );
	}

	float CosStep ( float t ) {
		return 1 - Mathf.Cos ( t * Mathf.PI * 0.5f );
	}

	float ExpStep ( float t ) {
		return t * t;
	}

	/* d = sqrt ( (x2-x1) + (y2-y1) ) */
	float Distance ( Vector3 a, Vector3 b ) {
		return Mathf.Sqrt( ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) ) );
	}
}
