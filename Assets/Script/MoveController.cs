using UnityEngine;

    /*
    ===
    ==
        a 2D move controller that exposes two public functions:

            - MoveUntilArrived()
            - RotateUntilFacingTarget()
        
        both return TRUE to signal completion of the associated action.
    ==
    ===
    */

public class MoveController {

    public enum InputType { none = 0, mouse = 1 } // TODO: implement

    /* this is a re-implemented Vector.MoveTowards() */
    public bool MoveUntilArrived ( Transform myTransform, Vector3 target, float speed = 1.0f ) {
        Vector3 curr = myTransform.position;
        Vector3 remaining = target - curr;
        if ( remaining != Vector3.zero ) {
            float d = remaining.magnitude;
            float maxDistDelta = speed * Time.deltaTime;
            if ( d < maxDistDelta || d == 0.0f ) {
                myTransform.position = target; // how to maintain momentum?
            } else {
                myTransform.position = curr + (remaining  / d) * maxDistDelta; // (remaining / d) = normalized vector
            }
            return false;
        }
        return true;
    }

    /* can simply use Mathf.Atan2 instead of checking the sign of Mathf.Acos() -- which is better?  */
    public bool RotateUntilFacingTarget (Transform myTransform, Vector3 target) {
        Vector3 heading = (target - myTransform.position).normalized;
        float theta = Mathf.Acos ( Vector3.Dot ( Vector3.right, heading ) ) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.identity;
        if ( heading.y < 0 ){ 
            rot = Quaternion.Euler ( 0f, 0f, 270f - theta ); 
        }
        else {
            rot = Quaternion.Euler ( 0f, 0f, theta - 90f );
        }
        myTransform.rotation = rot;
        return true;
    }
}
