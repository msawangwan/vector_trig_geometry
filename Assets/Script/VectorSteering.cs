using UnityEngine;

public static class VectorSteering {
    /* forumla for calculating a seeking force */
    public static Vector3 Seek (this Vector3 a, Vector3 b, Vector3 velocity, float maxSpeed) {
        return ( ( b - a ).normalized * maxSpeed ) - velocity;
    }

    /* forumla for calculating a fleeing force */
    public static Vector3 Flee (this Vector3 a, Vector3 b, Vector3 velocity, float maxSpeed) {
        return ( ( a - b ).normalized * maxSpeed ) - velocity;
    }

    public enum DecelerationRate { none = 0, slow = 3, normal = 2, fast = 1 }

    /* formula for calculating a decelerating, arrival force */
    public static Vector3 Arrive ( this Vector3 a, Vector3 b, Vector3 velocity, float maxSpeed, DecelerationRate decelerationRate = DecelerationRate.normal ) {
        Vector3 toTarget = b - a;

        float d = toTarget.Lengthf();
        if ( d <= 0 ) return Vector3.zero;
        float speed = Mathf.Min ( ( d / ( (float) decelerationRate * 0.3f ) ), maxSpeed ); // multiply decelerationRate by a 'tweaker' value

        return (toTarget * speed / d) - velocity;
    }
}
