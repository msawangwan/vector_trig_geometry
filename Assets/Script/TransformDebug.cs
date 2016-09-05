using UnityEngine;

public static class TransformDebug {

    /* draw x and y axes in transform local space */
    public static void DrawLocalAxis (this Transform self, float scale = 3.0f) {
		Debug.DrawRay ( self.position, self.up * scale, Color.yellow, 0.2f, false );
        Debug.DrawRay ( self.position, self.right * scale, Color.yellow, 0.2f, false );
	}

    /* draw a ray representing an objects heading or velocity */
    public static void DrawHeading (this Transform self, Vector3 heading) {
        Debug.DrawRay(self.position, heading * 5f, Color.red);
    }

    /* draw a line from transform origin to point target */
    public static void DrawLineToTarget (this Transform self, Vector3 target) {
        Debug.DrawLine(self.position, target);
    }
}