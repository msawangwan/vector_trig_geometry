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
            if (currentMove != null) {
                Debug.LogFormat(" move {0}", currentMove.ID);
            }
            if ( mouseClick_L == true ) {
                targetPosition = MousePointer.Pos();
                MoveQueue.Move move = QueuedMoves.HandleMoveRequest ( targetPosition );
                if ( move != null && shouldMove == false ) {
                    currentMove = move;
                    QueuedMoves.OnAfterSecondsSignalStartMove ();
                }
            }
            if ( shouldMove == true && currentMove != null ) {
                if ( mc.MoveUntilArrived ( moverTransform, currentMove.Position, MoveSpeed ) == true ) {
                    MoveQueue.Move move = QueuedMoves.HandleMoveEnd ( currentMove );
                    if ( move != null ) {
                        currentMove = move;
                    } else {
                        Debug.Log("ending movement");
                        currentMove = null;
                        shouldMove = false;
                        return;
                    }
                }
                if ( currentMove != null ) {
                    mc.RotateUntilFacingTarget ( moverTransform, currentMove.Position );
                }
            }
        }
    }

    void HandleOnStartMoveNow (bool flag) {
        shouldMove = flag;
    }
}