using UnityEngine;

/*
==========
=========
chase a target if the target enters a buffer area defined as zones along the edges of the screen.
==========
=========
*/

public class ChaseCamBounded : MonoBehaviour {

	// TODO: look into the concept of a 'camera rig'

	[TooltipAttribute ( "~ set in editor -- this is the target the camera will chase" )]
	public Transform ChaseTarget = null;

	[TooltipAttribute ( "~ detection zone to start chasing on for left and right sides of screen" )]
	[RangeAttribute ( 10.0f, 1000.0f )]
	public float xAxisBuffer = 110.0f; // TODO: calculate as percentage!!
	[TooltipAttribute( "~ detection zone to start chasing for top and bottom areas of screen" )]
	[RangeAttribute ( 10.0f, 1000.0f )]
	public float yAxisBuffer = 120.0f; // TODO: calculate as percentage!!
	[TooltipAttribute( "~ max chase speed camera is allowed to achieve" )]
	[RangeAttribute ( 1.0f, 1000.0f )]
	public float CameraMaximumChaseSpeed = 10.0f;
	[TooltipAttribute ( "~ radius (origin is target center) of zone to stop moving and set velocity to zero" )]
	[RangeAttribute ( 0.1f, 1.0f )]
	public float KillChaseRadius = 0.2f;

	float bufferLeft   { get { return Camera.main.pixelWidth - xAxisBuffer; } }
	float bufferRight  { get { return 0 + xAxisBuffer; } }
	float bufferTop    { get { return Camera.main.pixelHeight - yAxisBuffer; } }
	float bufferBottom { get { return 0 + yAxisBuffer; } }
	float killChaseRadSqr { get { return KillChaseRadius * KillChaseRadius; } }

	Vector3 displacement = Vector3.zero;
	bool isChasing = false;

	void Update () {
		if ( ChaseTarget ) {
			Vector3 chaseTargetScreenPos = Camera.main.WorldToScreenPoint ( ChaseTarget.position );
			displacement = PosWithCamDepth ( ChaseTarget.position ) - transform.position;
			if ( isChasing ) {
				float dSqr = displacement.sqrMagnitude;
				if ( dSqr < killChaseRadSqr ) {
					isChasing = false;
					return;
				}
				float d = Mathf.Sqrt(dSqr);
				transform.position += ( ( ( ( displacement / d ) * d ) / d ) * CameraMaximumChaseSpeed ) * Time.deltaTime;
				return;
			}
			isChasing = IsChaseTargetOutOfBounds ( chaseTargetScreenPos );
		}
	}

	bool IsChaseTargetOutOfBounds ( Vector3 chaseTargetCurrPos ) {
		return
			( chaseTargetCurrPos.x > bufferLeft ) || ( chaseTargetCurrPos.x < bufferRight ) ||
			( chaseTargetCurrPos.y > bufferTop )  || ( chaseTargetCurrPos.y < bufferBottom );
	}

	Vector3 PosWithCamDepth ( Vector3 target, float zTarget = -10.0f ) {
		return new Vector3 ( target.x, target.y, zTarget );
	}
}