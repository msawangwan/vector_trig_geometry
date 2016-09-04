using UnityEngine;

public static class FloatExtension {

    /* convert to an angle based on x = cos(theta) y = sin(theta) */
    public static Vector3 AsAngledVector2 (this float a, float z = 0.0f) {
        return new Vector3(Mathf.Cos(a), Mathf.Sin(a), z);
    }

	/* convert to rgb value */
	public static float AsRGB(this float f) {
        return f / 255.0f;
    }
}
