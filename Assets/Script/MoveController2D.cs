using UnityEngine;

public class MoveController2D {

    public enum InputType { none = 0, mouse = 1 } // TODO: implement

    public MoveController2D () {}

    public bool MoveUntilArrived (Transform myTransform, Vector3 target, float speed, float dt) {
        Vector3 curr = myTransform.position;
        Vector3 remaining = target - curr;
        if ( remaining != Vector3.zero ) {
            float d = remaining.magnitude;
            float maxDistDelta = speed * dt;
            if ( d < maxDistDelta || d == 0.0f ) {
                myTransform.position = target; // how to maintain momentum?
            } else {
                myTransform.position = curr + (remaining  / d) * maxDistDelta; // (remaining / d) = normalized vector
            }
            return false;
        }
        return true;
    }

    public bool RotateUntilFacingTarget (Transform myTransform, Vector3 target) {
        myTransform.rotation = GetRotation(myTransform.position, target);
        return true;
    }

    Quaternion GetRotation ( Vector3 facing, Vector3 target ) {
        Vector3 heading = (target - facing).normalized;
        float theta = Mathf.Acos ( Vector3.Dot ( Vector3.right, heading ) ) * Mathf.Rad2Deg;

        if ( heading.y < 0 ) return Quaternion.Euler ( 0f, 0f, 270f - theta ); 
        else return Quaternion.Euler ( 0f, 0f, theta - 90f );
    }
}
