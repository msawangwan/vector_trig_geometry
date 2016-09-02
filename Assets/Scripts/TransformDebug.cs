using UnityEngine;

public static class TransformDebug {

    /* Draw x and y axes in transform local space. */
    public static void DrawLocalAxis (this Transform self, float scale = 3.0f) {
		Debug.DrawRay ( self.position, self.up * scale, Color.yellow, 0.2f, false );
        Debug.DrawRay ( self.position, self.right * scale, Color.yellow, 0.2f, false );
	}
}