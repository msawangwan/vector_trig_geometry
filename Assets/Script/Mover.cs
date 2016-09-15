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

    MoveQueue.Move currentMove = null;
    MoveController mc = new MoveController ();
    Vector3 targetPosition = Vector3.zero;
    bool shouldMove = false;

    Transform moverTransform { get { return gameObject.transform; } }
    bool mouseClick_L { get { return Input.GetMouseButton ( 0 ); } }

    void Start () {
        if ( QueuedMoves == null ) {
            Debug.LogError ( "mover has no move queue!", gameObject );
        } else {
            QueuedMoves.CreateNewMoveQueuePool (10, gameObject.name);
            Debug.LogFormat("test {0}", QueuedMoves.MoverName);
        }
    }

    void Update () {
        if ( QueuedMoves == null ) {
            Debug.LogError ( "mover has no move queue!", gameObject );
            return;
        }

        if ( mouseClick_L ) {
            targetPosition = MousePointer.Pos();
            //MoveQueue.Move move = QueuedMoves.DequeueNextMove(targetPosition);
            //QueuedMoves.EnqueueMove (null);
            shouldMove = true;
        }
        if ( shouldMove ) {
            if ( mc.MoveUntilArrived ( moverTransform, targetPosition, MoveSpeed ) ) {
                shouldMove = false;
            }
            mc.RotateUntilFacingTarget ( moverTransform, targetPosition );
        }
    }
}