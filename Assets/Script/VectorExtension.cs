using UnityEngine;

public static class VectorExtension {

    /* prettify for output to console (x and y only) */
    public static string Stringify2 (this Vector3 a) {
        return a.x.ToString() + "," + a.y.ToString();
    }

    /* return the right hand normal of a 2D vector */
    public static Vector3 NormalRH (this Vector3 a) {
        return new Vector3(-a.y, a.x, a.z);
    }

    /* return the left hand normal of a 2D vector */
    public static Vector3 NormalLH (this Vector3 a) {
        return new Vector3(a.y, -a.x, a.z);
    }

    /* if vector a is greater than maxLength, scale a by max length else return a */
    public static Vector3 Truncate (this Vector3 a, float maxLength) {
        if ( a.Lengthf () > maxLength ) return a.normalized * maxLength;
        return a;
    }

    public static Vector3 SpringVel (this Vector3 a, float tension, float damping, float dt) {
        return a.Truncate(tension) * damping * dt;
    }

    /* the perpproduct of two vectors a and b is the dot product of a and the RH normal of b */
    public static float PerpProduct(this Vector3 a, Vector3 b) {
        return Vector3.Dot(a, b.NormalRH());
    }

    /* return an attenuated force calculated by the law of inverse-squares */
    /* the inverse-square law: F_v = F / d**2 OR F_v = F / 1 + d**2 (no divide-by-zero risk?) */
    public static float InverseSquare(this Vector3 a, Vector3 b, float force = 10f) {
        return force / ( 1f + (b - a).sqrMagnitude );
    }

    /* overloaded, call on a vector representing the diff between two vectors */
    public static float InverseSquare(this Vector3 a, float force = 10f) {
        return force / ( 1f + a.sqrMagnitude );
    }

    /* l = c = sqrt ( a**2 + c**2 ) */
    public static float Lengthf (this Vector3 a) {
        return Mathf.Sqrt ( ( a.x * a.x ) + ( a.y * a.y ) );
    }

    /* magnitude = d = sqrt ( (a2-a1)**2 + (y2-y1)**2 ) */
    public static float Distancef (this Vector3 a, Vector3 b) {
		return Mathf.Sqrt ( ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) ) );
	}

    /* l = c = a**2 + c**2 */
    public static float sqrLengthf (this Vector3 a) {
        return ( a.x * a.x ) + ( a.y * a.y );
    }

    /* magnitude**2 = d**2 = (x2-x1)**2 + (y2-y1)**2 */
    public static float sqrDistancef (this Vector3 a, Vector3 b) {
		return  ( ( b.x - a.x ) * ( b.x - a.x ) ) + ( ( b.y - a.y ) * ( b.y - a.y ) );
	}

    /* return true if vector a is larger than a value equal to almost zero */
    public static bool IsNonZero (this Vector3 a) {
        return sqrLengthf ( a ) > 0.00000001;
    }
}
