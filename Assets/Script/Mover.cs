using UnityEngine;

public class Mover : MonoBehaviour {
    public GameObject MoveMarkerPrefab  = null;

    public float       speedMultiplier   = 5.0f;

    Transform         moveMarker        = null;
    Vector3           targetPosition    = Vector3.zero;
    MoveController2D  mc                = new MoveController2D();
    bool              isMoving          = false;

    void Start () {
        moveMarker = MoveMarkerPrefab.InstantiateAtPositionToParent ( gameObject.transform, Vector3.zero, false ).transform;
    }

    void Update () {
        if (Input.GetMouseButton (0) && isMoving == false) {
            targetPosition = MousePointer.Pos();
            ToggleMarker ( targetPosition );
            isMoving = true;
        }

        if ( isMoving ) {
            transform.DrawLineToTarget ( targetPosition );
            if ( mc.MoveUntilArrived ( gameObject.transform, targetPosition, speedMultiplier, Time.deltaTime ) ) {
                ToggleMarker ( Vector3.zero );
                isMoving = false;
            }
            mc.RotateUntilFacingTarget ( gameObject.transform, targetPosition );
        }
    }

    void ToggleMarker ( Vector3 position ) {
        if (!moveMarker.gameObject.activeInHierarchy) {
            moveMarker.parent = null;
            moveMarker.position = position;
            moveMarker.gameObject.SetActive(true);
        } else {
            moveMarker.gameObject.SetActive(false);
            moveMarker.parent = gameObject.transform;
            moveMarker.position = position;
        }
    }
}
