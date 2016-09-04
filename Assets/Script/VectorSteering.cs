﻿using UnityEngine;

public static class VectorSteering {

    public static readonly float ACosErrMargin = 0.95f; // acos(0.95) = 18 degrees

    /* forumla for calculating a seeking force */
    public static Vector3 Seek (this Vector3 a, Vector3 b, Vector3 velocity, float maxSpeed) {
        return ( ( b - a ).normalized * maxSpeed ) - velocity;
    }

    /* forumla for calculating a fleeing force */
    public static Vector3 Flee (this Vector3 a, Vector3 b, Vector3 velocity, float maxSpeed) {
        return ( ( a - b ).normalized * maxSpeed ) - velocity;
    }

    /* tweak the arrival decel rate */
    public enum DecelerationRate { none = 0, slow = 3, normal = 2, fast = 1 }

    /* formula for calculating a decelerating, arrival force */
    public static Vector3 Arrive ( this Vector3 a, Vector3 b, Vector3 velocity, float maxSpeed, DecelerationRate decelerationRate = DecelerationRate.normal ) {

        Vector3 toTarget = b - a;

        float d = toTarget.Lengthf();
        if ( d <= 0 ) return Vector3.zero;

        float speed = Mathf.Min ( ( d / ( (float) decelerationRate * 0.3f ) ), maxSpeed ); // multiply decelerationRate by a 'tweaker' value
        return (toTarget * speed / d) - velocity;
    }

    public static Vector3 Pursue (this Vector3 pursuer, Vector3 evader, Vector3 velocity, float maxSpeed) {

        Vector3 toEvader = evader - pursuer;
        float relHeading = Vector3.Dot(pursuer, evader);
        float absHeading = Vector3.Dot(pursuer, toEvader);

        if (absHeading > 0 && relHeading < -ACosErrMargin) { // acos(0.95) = 18 degrees
            return pursuer.Seek(evader, velocity, maxSpeed); // we're head2head so just seek to target
        }

        //float tLookAhead = toEvader.Lengthf() / maxSpeed + evaderspeed;
        //return pursuer.Seek(evaderPos + evaderVel * tLookAhead, velocity, maxSpeed);
        return Vector3.zero;
    }
}
