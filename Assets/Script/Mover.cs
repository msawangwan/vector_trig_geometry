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
        Debug.AssertFormat ( QueuedMoves != null, "{0} was not assigned a move queue!", gameObject.name );
        QueuedMoves.CreateNewMoveQueuePool ( 10, gameObject.name );
        QueuedMoves.StartMoveNow += HandleOnStartMoveNow;
    }

    void Update () {
        if ( QueuedMoves ) {
            DEBUG_PrintClassLocalBools ();
            DEBUG_PrintCurrentMoveID ();
            if ( shouldMove == false && currentMove == null ) {
                Debug.Log("checking if any moves toexecute mover accepting input");
                MoveQueue.Move move = QueuedMoves.CheckForQueuedMove();
                if (move != null) {
                    currentMove = move;
                    QueuedMoves.SignalMoveStart ( true );
                }
            }
            if ( mouseClick_L == true && shouldMove == false ) {
                targetPosition = MousePointer.Pos();
                MoveQueue.Move move = QueuedMoves.GetNextMoveFromInput ( targetPosition );
                if (move != null) {
                    currentMove = move;
                    QueuedMoves.SignalMoveStart ( true );
                }
            }
            if ( shouldMove == true && currentMove != null ) {
                if ( mc.MoveUntilArrived ( moverTransform, currentMove.Position, MoveSpeed ) == true ) {
                    Debug.LogFormat("finished move with {0}", currentMove.ID);
                    QueuedMoves.SignalMoveEnd ( currentMove, true );
                    currentMove = null;
                    shouldMove = false;
                }
                if ( currentMove != null ) {
                    Debug.LogFormat("rotating to move dest {0}", currentMove.ID);
                    mc.RotateUntilFacingTarget ( moverTransform, currentMove.Position );
                }
            }
        }
    }

    void HandleOnStartMoveNow (bool flag) {
        shouldMove = flag;
        Debug.LogFormat("move flag changed event fired: {0}", Time.time);
    }

    void DEBUG_PrintClassLocalBools() {
        Debug.LogFormat("should move is {0}", shouldMove);
        Debug.LogFormat("current move is null {0}", currentMove == null);
    }

    void DEBUG_PrintCurrentMoveID ( MoveQueue.Move other = null ) {
        if (other == null) {
            if (currentMove != null) {
                Debug.LogFormat("status of current move: {0}", currentMove.ID);
            }
        } else {
            Debug.LogFormat("status of current move: {0}", other.ID);
        }
    }
}