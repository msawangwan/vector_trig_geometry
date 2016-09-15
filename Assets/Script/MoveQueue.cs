using UnityEngine;
using System.Collections;

    /*
    ===
    ==
        queues positions for a mover to iterate over and move-to
    ==
    ===
    */

public class MoveQueue : MonoBehaviour {

    public class Move {
        public static int totalActiveCount = 0;

        public Move       Next            { get; private set; }
        public Move       Previous        { get; private set; }

        public int        Priority        { get; set; }

        public GameObject MarkerObject    { get; private set;}
        public Transform  MarkerTransform { get { return MarkerObject.transform; } }
        public Vector3    Position        { get { return MarkerObject.transform.position; } }

        public Move ( Move next, Move previous, GameObject markerObject ) {
            Next = next;
            Previous = previous;
            MarkerObject = markerObject;
        }
    }

    /* references */
    [TooltipAttribute ( "~ the parent container for the marker pool and move queue-- warning: do NOT use a gameobject that changes position, use either the object this script is attached to or an empty gameobject" )]
    public Transform   MarkerPoolTransform  = null;
    [TooltipAttribute ( "~ this prefab is used to mark the current move-to target vector position" )]
    public GameObject  MoveMarkerPrefab     = null;
    [TooltipAttribute ( "~ this prefab is used to mark all moves in the move queue" )]
    public GameObject  QueueMarkerPrefab    = null;

    /* editor variables */
    [TooltipAttribute ( "~ maximum number of moves allowed to be queued up" )]
    [RangeAttribute ( 1, 50 )]
    public int         QueueBufferCapacity  = 10;
    [TooltipAttribute ( "~ adjusts the move-to time of the moving object" )]
    [RangeAttribute ( 0.1f, 1000.1f )]
    public float       MoveSpeedMultiplier  = 5.0f;
    [TooltipAttribute ( "~ adjusts the amount of time to wait before performing the next move" )]
    [RangeAttribute ( 0.1f, 10.1f )]
    public float       MoveDelay            = 0.35f;
    [TooltipAttribute ( "~ rate limits clicks registered as actual clicks" )]
    [RangeAttribute ( 0.1f, 1.1f )]
    public float       RateLimitInterval    = 0.25f;

    /* local variables */
    MoveController     mc                   = new MoveController ();
    GameObject         moveMarker           = null;
    GameObject         currentMove          = null;
    float              timeSinceLastClicked = 0.0f;
    bool               isExecutingMove      = false;

    /* local properties */
    Transform          moverTransform       { get { return gameObject.transform; } }
    float              setLastClickTime     { get { return Time.time + RateLimitInterval; } }

/* Priority Queque using the Min-Heap invariant:
 - Uses a fixed-size array (for speed)
 - Index starts at 1 not 0
 - Left Child node is at [2 * i], if available
 - Right Child node is at [2 * i + 1], if available
 - Parent node is at [i / 2], if available
Insert aka Enqueue -- O(logn)
Remove aka Dequeue -- O(logn)
Extract-Min aka Head- O(1)
 - Contains() ---------- O(1) */

    /* WIP min-heap implementation */
    const int maxBufferSize = 10;
    const int idFirst = 0;
    const int idInactive = maxBufferSize * 100; // can not be a valid ID, represents the id/priority of an inactive move
    
    public static int numMovesActive = 0;

    public MoveQueue.Move Head = null;
    public MoveQueue.Move Tail = null;

    public int ActiveCount { get { return numMovesActive; } }

    public void Enqueue ( MoveQueue.Move m ) {
        //int leftChild = numMovesActive;
        int leftChild = MarkerPoolTransform.ChildCountActive () - 1;
        while (leftChild > 0) {
            int parent = (leftChild - 1) / 2;
            Move priorityLeft = MarkerPoolTransform.GetChild (leftChild).gameObject.GetComponent <Move>();
            Move parentPriority = MarkerPoolTransform.GetChild (parent).gameObject.GetComponent <Move>();
            if (priorityLeft.Priority > parentPriority.Priority) {
                break;
            }


        }
    }



    public IEnumerable MoveEnumerable ( Vector3 targetPosition ) {
        if ( Time.time > timeSinceLastClicked ) { // rate-limiting
            timeSinceLastClicked = setLastClickTime;
            if ( isExecutingMove ) {
                GameObject queuedMove = GetNextInactive ();
                if ( queuedMove != null ) {
                    queuedMove.transform.position = targetPosition;
                    queuedMove.transform.SetAsLastSibling ();
                    queuedMove.SetActive ( true );
                }
            } else {
                currentMove = MoveMarkerToCurrentMove ( targetPosition );
                yield return SetMoveFlagAfterDelay ( MoveDelay );
            }
        }
        yield return null;
    }

    public void RaiseMoveQueryEvent ( Vector3 targetPosition ) {
        if ( Time.time > timeSinceLastClicked ) { // rate-limiting
            timeSinceLastClicked = setLastClickTime;
            if ( isExecutingMove ) {
                GameObject queuedMove = GetNextInactive ();
                if ( queuedMove != null ) {
                    queuedMove.transform.position = targetPosition;
                    queuedMove.transform.SetAsLastSibling ();
                    queuedMove.SetActive ( true );
                }
            } else {
                currentMove = MoveMarkerToCurrentMove ( targetPosition );
                StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
            }
        }
    }

    public void RaiseMoveComplete ( bool hasCompletedMove ) {
        if ( isExecutingMove ) {
            if ( hasCompletedMove ) {
                currentMove.SetActive ( false );
                GameObject queuedMove = GetNextActive ();
                if ( queuedMove != null ) {
                    currentMove = MoveMarkerToCurrentMove ( queuedMove.transform.position );
                    queuedMove.SetActive ( false );
                    StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
                } else {
                    isExecutingMove = false;
                }
            }
        }
    }

    public Vector3 GetMoveToTarget ( Vector3 currentPosition ) {
        Vector3 target = Vector3.zero;
        return Vector3.zero;
    }

    void Disabled_Start () {
        InitialiseMarkerPool ();
    }

    void Disabled_Update () { // TODO: use a linked-list instead of for-loops
        GameObject move = GetNextActive ();
        if ( move != null && isExecutingMove == false ) {
            currentMove = move;
        }
        if ( Input.GetMouseButton ( 0 ) ) {
            if ( Time.time > timeSinceLastClicked ) { // rate-limiting
                timeSinceLastClicked = setLastClickTime;
                Vector3 mousePos = MousePointer.Pos ();
                if ( isExecutingMove ) {
                    GameObject queuedMove = GetNextInactive ();
                    if ( queuedMove != null ) {
                        queuedMove.transform.position = mousePos;
                        queuedMove.transform.SetAsLastSibling ();
                        queuedMove.SetActive ( true );
                    }
                } else {
                    currentMove = MoveMarkerToCurrentMove ( mousePos );
                    StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
                }
            }
        }
        if ( isExecutingMove ) {
            if ( mc.MoveUntilArrived ( moverTransform, currentMove.transform.position, MoveSpeedMultiplier, Time.deltaTime ) ) {
                currentMove.SetActive ( false );
                GameObject queuedMove = GetNextActive ();
                if ( queuedMove != null ) {
                    currentMove = MoveMarkerToCurrentMove ( queuedMove.transform.position );
                    queuedMove.SetActive ( false );
                    StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
                } else {
                    isExecutingMove = false;
                }
            }
            mc.RotateUntilFacingTarget ( moverTransform, currentMove.transform.position );
        }
    }

    void InitialiseMarkerPool () {
        for (int i = 0; i < QueueBufferCapacity; i++) {
            GameObject marker = Instantiate<GameObject> ( QueueMarkerPrefab );
            marker.SetActive ( false );
            marker.transform.SetParent ( MarkerPoolTransform );
        }
    }

    GameObject MoveMarkerToCurrentMove ( Vector3 position ) {
        if ( moveMarker == null ) {
            moveMarker = Instantiate<GameObject> ( MoveMarkerPrefab );
        }
        moveMarker.transform.position = position;
        moveMarker.SetActive ( true );
        return moveMarker;
    }

    GameObject GetNextActive () {
        for ( int i = 0; i < MarkerPoolTransform.childCount; i++ ) {
            GameObject curr = MarkerPoolTransform.GetChild ( i ).gameObject;
            if ( curr.activeInHierarchy ) {
                return curr;
            }
        }
        return null;
    }

    GameObject GetNextInactive () {
        for ( int i = 0; i < MarkerPoolTransform.childCount; i++ ) {
            GameObject curr = MarkerPoolTransform.GetChild ( i ).gameObject;
            if ( curr.activeInHierarchy == false ) {
                return curr;
            }
        }
        return null;
    }

    IEnumerator SetMoveFlagAfterDelay ( float delay ) {
        yield return new WaitForSeconds ( delay );
        isExecutingMove = true;
    }
}
