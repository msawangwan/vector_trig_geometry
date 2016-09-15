using UnityEngine;

    /*
    ===
    ==
        a mover is a gameobject that moves.
        the follow scripts/components are its dependencies:

            - MoveController.cs
            - MoveQueue.cs
    ==
    ===
    */

public class Mover : MonoBehaviour {

    public MoveQueue QueuedMoves = null;
    public float MoveSpeed = 10.0f;

    MoveController mc = new MoveController ();
    Vector3 targetPosition = Vector3.zero;
    bool shouldMove = false;

    Transform moverTransform { get { return gameObject.transform; } }
    bool mouseClick_L { get { return Input.GetMouseButton ( 0 ); } }

    void Update () {
        if ( QueuedMoves == null ) {
            Debug.LogError ( "mover has no move queue!", gameObject );
            return;
        }

        if ( mouseClick_L ) {
            targetPosition = MousePointer.Pos();
            QueuedMoves.RaiseMoveQueryEvent (targetPosition);
            shouldMove = true;
        }
        if ( shouldMove ) {
            if ( mc.MoveUntilArrived ( moverTransform, targetPosition, MoveSpeed, Time.deltaTime ) ) {
                shouldMove = false;
            }
            mc.RotateUntilFacingTarget ( moverTransform, targetPosition );
        }
    }
}