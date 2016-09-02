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
}
