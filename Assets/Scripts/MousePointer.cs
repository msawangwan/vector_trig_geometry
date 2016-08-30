using UnityEngine;

public class MousePointer : MonoBehaviour {
    public Transform target;

    void Update () {
        PaintLine2D ();
    }

    void PaintLine2D () {
        Vector3 to = Camera.main.ScreenToWorldPoint ( new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, 10.0f ) );
        Debug.DrawLine ( target.position, to, Color.green, 0.2f, false );
    }
}
