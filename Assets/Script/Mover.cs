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
            DEBUG_PrintCurrentMoveID ();
            if ( mouseClick_L == true ) {
                targetPosition = MousePointer.Pos();
                MoveQueue.Move move = QueuedMoves.GetNextMoveFromInput ( targetPosition );
                if ( move != null && shouldMove == false ) {
                    currentMove = move;
                    QueuedMoves.OnSignalAfterSecondsStartMove ();
                }
            }
            if ( shouldMove == true && currentMove != null ) {
                Debug.LogFormat("executing move id# {0}", currentMove.ID);
                if ( mc.MoveUntilArrived ( moverTransform, currentMove.Position, MoveSpeed ) == true ) {
                    Debug.LogFormat("finished move id# {0}", currentMove.ID);
                    MoveQueue.Move move = QueuedMoves.OnMoveEndCheckForNext ( currentMove );
                    if ( move != null ) {
                        currentMove = move;
                    } else {
                        currentMove = null;
                        shouldMove = false;
                    }
                }
                if ( currentMove != null ) {
                    Debug.LogFormat("rotating to move dest id# {0}", currentMove.ID);
                    mc.RotateUntilFacingTarget ( moverTransform, currentMove.Position );
                }
            }
        }
    }

    void HandleOnStartMoveNow (bool flag) {
        shouldMove = flag;
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