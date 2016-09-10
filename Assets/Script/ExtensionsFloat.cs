using UnityEngine;

public static class ExtensionsFloat {

    public const float TwoPI = Mathf.PI;

    public static float RandFloat01 () {
        return Random.Range(0f, 1f);
    }

    public static float RandFloat1 () {
        return Random.Range(-1f, 1f);
    }

    public static float RandClamped () {
        return RandFloat01() - RandFloat01();
    }

    public static float RandTheta () {
        return RandFloat01() * TwoPI;
    }

    /* convert to an angle based on x = cos(theta) y = sin(theta) */
    public static Vector3 AsAngledVector2 (this float a, float z = 0.0f) {
        return new Vector3(Mathf.Cos(a), Mathf.Sin(a), z);
    }

    // TODO: verify how this works and compare with AsAngledVector2
    public static Vector3 GetRandHeading (this float heading, float maxHeadingChange) {
        float floor = Mathf.Clamp(heading - maxHeadingChange, 0.0f, 360.0f);
        float ceil = Mathf.Clamp(heading + maxHeadingChange, 0.0f, 360.0f);
        float newHeading = Random.Range(floor, ceil);
        return new Vector3(0, newHeading, 0);
    }

	/* convert to rgb value */
	public static float AsRGB(this float f) {
        return f / 255.0f;
    }
}
