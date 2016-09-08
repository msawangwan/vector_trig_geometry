using UnityEngine;

public class ToTargetMoveController2D {

    public enum InputType { none = 0, mouse = 1 } // TODO: implement

    public ToTargetMoveController2D () {}

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
}
