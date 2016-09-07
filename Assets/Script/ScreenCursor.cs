using UnityEngine;

public class ScreenCursor : MonoBehaviour {
    public Transform Origin;
    public bool DrawOnlyOnClick;

    void Update () {
        if ( Input.GetMouseButton (0) || Input.GetMouseButton (1) || DrawOnlyOnClick == false ) {
            Debug.DrawRay ( Origin.position, MousePointer.Pos () * 10f, Color.green, 0.05f );
        }
    }
}
