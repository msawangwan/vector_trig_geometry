using UnityEngine;

public static class SteeringBehaviour {

    /* arrival decel rate */
    public enum DecelerationRate { none = 0, slow = 3, normal = 2, fast = 1 }

	/* acos(0.95) = 18 degrees */
    public const float ACosErrMargin = 0.95f;

    /* forumla for calculating a seeking force */
    public static Vector3 Seek (this SteeringAgent seeker, SteeringAgent target) {
        return ( ( target.transform.position - seeker.Position).normalized * seeker.MaximumSpeed ) - seeker.Velocity;
    }

	/* forumla for calculating a seeking force (overload) */
    public static Vector3 Seek (this SteeringAgent seeker, Vector3 target) {
        return ( ( target - seeker.Position).normalized * seeker.MaximumSpeed ) - seeker.Velocity;
    }

    /* forumla for calculating a fleeing force */
    public static Vector3 Flee (this SteeringAgent fleeing, SteeringAgent target) {
        return ( ( fleeing.Position - target.transform.position ).normalized * fleeing.MaximumSpeed ) - fleeing.Velocity;
    }

    /* forumla for calculating a fleeing force (overload) */
    public static Vector3 Flee (this SteeringAgent fleeing, Vector3 target) {
        return ( ( fleeing.Position - target ).normalized * fleeing.MaximumSpeed ) - fleeing.Velocity;
    }

    /* formula for calculating a decelerating, arrival force */
    public static Vector3 Arrive ( this SteeringAgent arriving, Vector3 destination, DecelerationRate decelerationRate = DecelerationRate.normal ) {

        Vector3 toTarget = destination - arriving.Position;

        float d = toTarget.Lengthf();
        if ( d <= 0 ) return Vector3.zero;

        float speed = Mathf.Min ( ( d / ( (float) decelerationRate * 0.3f ) ), arriving.MaximumSpeed ); // multiply decelerationRate by a 'tweaker' value
        return (toTarget * speed / d) - arriving.Velocity;
    }

    public static Vector3 Pursue (this SteeringAgent pursuing, SteeringAgent evading) {

        Vector3 a = pursuing.Position;
        Vector3 b = evading.Position;
        Vector3 toEvader = b - a;

        float relHeading = Vector3.Dot ( a, b );
        float absHeading = Vector3.Dot ( a, toEvader );

        if ( absHeading > 0 && relHeading < -ACosErrMargin ) { // acos(0.95) = 18 degrees
            return pursuing.Seek ( b ); // we're head2head so just seek to target
        }

        float tLookAhead = toEvader.Lengthf () / pursuing.MaximumSpeed + evading.MaximumSpeed;
        return pursuing.Seek ( b + evading.Velocity * tLookAhead );
    }
}
