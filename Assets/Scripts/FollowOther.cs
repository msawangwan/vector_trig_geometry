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

	void Interpolate () {

		isInterpolating = true;
		startTime = Time.time;

		startpoint = transform.position;
		endpoint = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, 10.0f ) );
		PaintRay2DAxisX();
	}

	bool CalculateInterpolation ( Transform self, Vector3 start, Vector3 end, float t ) {

		PaintRay2DHeading ();

		float percent = ExpStepf ( TimeStepf ( t ) );
		self.position = Vector3.Lerp ( start, end, percent );

		if ( percent >= 1.0f ) return true;
		else return false;
	}

	/* t = currentLerpTime / lerpTime */
	float TimeStepf ( float t ) {
		return ( Time.time - t ) / 1.0f;
	}

	/* t = t * t * ( 3 - ( 2 * t ) ) */
	float SmoothStepf ( float t ) {
		return ( t * t ) * ( 3.0f - ( 2.0f * t ) );
	}

	float SinStepf ( float t ) {
		return Mathf.Sin ( t * Mathf.PI * 0.5f );
	}

	float CosStepf ( float t ) {
		return 1 - Mathf.Cos ( t * Mathf.PI * 0.5f );
	}

	float ExpStepf ( float t ) {
		return t * t;
	}

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
