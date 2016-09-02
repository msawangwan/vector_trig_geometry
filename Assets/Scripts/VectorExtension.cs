using UnityEngine;

public static class VectorExtension {

    /* if vector a is greater than maxLength, scale a by max length else return a */
    public static Vector3 Truncate (this Vector3 a, float maxLength) {
        if ( a.Lengthf () > maxLength ) return a.normalized * maxLength;
        return a;
    }

    /* return true if vector a is larger than a value equal to almost zero */
    public static bool IsNonZero (this Vector3 a) {
        return sqrLengthf ( a ) > 0.00000001;
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
}
