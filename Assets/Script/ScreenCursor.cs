using UnityEngine;

/*
===============
==============
A debug script that draws rays from an origin to the mouse pointer.
==============
===============
*/

public class ScreenCursor : MonoBehaviour {
    public Transform Origin;
    public bool EnableDraw;
    public bool DrawOnlyOnClick;

    void Update () {
        if (EnableDraw) {
            Debug.Assert(Origin != null, "screen cursor has no target");
            if (Origin) {
                if ( Input.GetMouseButton (0) || Input.GetMouseButton (1) || DrawOnlyOnClick == false ) {
                    Debug.DrawRay ( Origin.position, MousePointer.Pos () * 10f, Color.green, 0.05f );
                }
            }
        }
    }
}