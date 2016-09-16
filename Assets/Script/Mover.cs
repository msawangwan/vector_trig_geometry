using UnityEngine;
using System.Collections;

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
            QueuedMoves.StartMoveNow += HandleOnStartMoveNow;
        }
    }

    void Update () {
        if ( QueuedMoves ) {
            if (currentMove != null) {
                Debug.LogFormat("status of current move: {0}", currentMove.ID);
            }
            if ( mouseClick_L && shouldMove == false ) {
                targetPosition = MousePointer.Pos();
                currentMove = QueuedMoves.GetNextMove(targetPosition);
            }
            if ( shouldMove && currentMove != null ) {
                if ( mc.MoveUntilArrived ( moverTransform, currentMove.Position, MoveSpeed ) ) {
                    currentMove = null;
                    shouldMove = false;
                }
                mc.RotateUntilFacingTarget ( moverTransform, currentMove.Position );
            }
            return;
        }
    }

    void HandleOnStartMoveNow (bool flag) {
        shouldMove = flag;
        Debug.LogFormat("move flag changed event fired: {0}", Time.time);
    }
}