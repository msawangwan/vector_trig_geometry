#define DEBUG

using UnityEngine;

/// <summary>
/// Attach as a component to a GameObject and, from another script on the object 
/// run the Interpolate method in the update method.
/// <summary>
public class TransformInterpolator : MonoBehaviour {

    bool isInterpolating = true;
    bool isLerping = false;
    bool isSlerping = false;

    float tLerp = 0f;
    float tSlerp = 0f;

    Vector3 initialPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;

    Quaternion initialRot = Quaternion.identity;
    Quaternion targetRot = Quaternion.identity;

    /* Call in update.  */
    public void Interpolate() {

        if (isInterpolating) {

            if (Input.GetMouseButtonDown(0)) {
                tLerp = StartLerp(gameObject.transform, MousePointer.Pos());
                tSlerp = StartSlerp(gameObject.transform, MousePointer.Pos());
            }

            if (isLerping) {
                if (LerpToTargetPos(gameObject.transform, initialPos, targetPos, tLerp)) {
                    isLerping = false;
                }
            }

            if (isSlerping) {
                if (SlerpToTargetRot(gameObject.transform, initialRot, targetRot, tSlerp)) {
                    isSlerping = false;
                }
            }
        }
    }

	float StartLerp (Transform start, Vector3 end) {
		isLerping = true;

        initialPos = start.position;
        targetPos = end;

#if DEBUG
        PaintPosStartToEnd(initialPos, targetPos);
#endif  

        return Time.time;
    }

    float StartSlerp (Transform start, Vector3 end) {
        isSlerping = true;

        initialRot = start.rotation;
        targetRot = TargetRotTo(start.position, end);

#if DEBUG
        PaintRotFacingToTarget(transform.position, end);
#endif  

        return Time.time;
    }

    bool LerpToTargetPos(Transform self, Vector3 start, Vector3 end, float t) {

        float percent = ExpStepf(LinearStepf(t));
        self.position = Vector3.Lerp(start, end, percent);

        if ( percent >= 1.0f ) return true;
		else return false;
	}

    bool SlerpToTargetRot(Transform self, Quaternion start, Quaternion end, float t) {

        float percent = ExpStepf(LinearStepf(t));
        self.rotation = Quaternion.Slerp(start, end, percent);

        if ( percent >= 1.0f ) return true;
        else return false;
    }

    /* Returns a Quaternion representing the rotation to apply to rotate from current facing to target facing. */
    Quaternion TargetRotTo(Vector3 facing, Vector3 target) {

        Vector3 heading = (target - facing).normalized;
        float theta = Mathf.Acos(Vector3.Dot(Vector3.right, heading)) * Mathf.Rad2Deg;

        if ( heading.y < 0 ) return Quaternion.Euler ( 0f, 0f, 270f - theta ); 
        else return Quaternion.Euler ( 0f, 0f, theta - 90f );
    }

    /* t = currentLerpTime / lerpTime */
    float LinearStepf(float t) {
		return ( Time.time - t ) / 1.0f;
	}

    /* t = t * t * ( 3 - ( 2 * t ) ) */
    float SmoothStepf(float t) {
		return ( t * t ) * ( 3.0f - ( 2.0f * t ) );
	}

    float SinStepf(float t) {
		return Mathf.Sin ( t * Mathf.PI * 0.5f );
	}

    float CosStepf(float t) {
		return 1 - Mathf.Cos ( t * Mathf.PI * 0.5f );
	}

    float ExpStepf(float t) {
		return t * t;
	}

	void PaintPosStartToEnd (Vector3 posFrom, Vector3 to, float dur = 3.0f) {
        Debug.DrawLine(posFrom, to, Color.red, dur, false);
	}

    void PaintRotFacingToTarget(Vector3 facingFrom, Vector3 to, float dur = 3.0f) {
        Debug.DrawRay(facingFrom, transform.up * 10.0f, Color.yellow, dur, false);
        Debug.DrawRay(facingFrom, to, Color.yellow, dur, false);
    }
}