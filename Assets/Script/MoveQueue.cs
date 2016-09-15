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

        public static int TotalActiveCount { get; set; }
        public int        Priority         { get; set; }

        public Move       Next             { get; private set; }
        public Move       Previous         { get; private set; }

        public GameObject MarkerObject     { get; private set;}
        public Transform  MarkerTransform  { get { return MarkerObject.transform; } }
        public Vector3    Position         { get { return MarkerObject.transform.position; } }

        public Move () {}

        public Move ( Move next, Move previous, GameObject markerObject, int priority ) {
            Next = next;
            Previous = previous;
            MarkerObject = markerObject;
            Priority = priority;
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
    Transform poolActive                    = null;
    Transform poolInactve                   = null;
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
    const int indexFirst = 0;
    const int indexInactive = maxBufferSize * 100; // can not be a valid ID, represents the id/priority of an inactive move
    const int indexSwap = maxBufferSize + 1;

    public MoveQueue.Move Head = null;
    public MoveQueue.Move Tail = null;

    public GameObject[] markers = null;

    public string MoverName  { get; private set; }
    public int ActiveCount { get { return MoveQueue.Move.TotalActiveCount; } }

    public GameObject[] CreateNewMoveQueuePool ( int numPooledObjects = 1, string owner = "move_queue: un-identified_owner", bool returnAllocation = false ) {
        InitialiseSubPoolContainerTransforms();
        if (poolActive != null && poolInactve != null) {
            switch (returnAllocation) {
                case true:
                    return QueueMarkerPrefab.InstantiatePoolAlloc<GameObject>(poolInactve, maxBufferSize, false);
                default:
                    QueueMarkerPrefab.InstantiatePool<GameObject>(poolInactve, maxBufferSize, false);
                    return null;
            }
        } else { Debug.LogError ( "movequeue has no subpools for active and inactive pooled objects" ); }
        return null;
    }

    public void EnqueueMove ( MoveQueue.Move move ) {
        int leftChild = MarkerPoolTransform.ChildCountActive () - 1;
        while ( leftChild > 0 ) {
            int parent = ( leftChild - 1 ) / 2;
            MoveQueue.Move priorityLeft = MarkerPoolTransform.GetChild (leftChild).gameObject.GetComponent<Move> ();
            MoveQueue.Move priorityParent = MarkerPoolTransform.GetChild (parent).gameObject.GetComponent<Move> ();
            if (priorityLeft.Priority > priorityParent.Priority) {
                break;
            }
            priorityParent.MarkerTransform.SetSiblingIndex(indexSwap);
            priorityLeft.MarkerTransform.SetSiblingIndex(parent);
            priorityParent.MarkerTransform.SetSiblingIndex(leftChild);
            leftChild = parent;
        }
    }

    public MoveQueue.Move DequeueNextMove ( Vector3 targetPosition ) {
        return null;
    }

    public MoveQueue.Move GetNextMove ( Vector3 targetPosition ) {
        if ( Time.time > timeSinceLastClicked ) { // rate-limit saftey
            timeSinceLastClicked = setLastClickTime;
            if ( isExecutingMove ) {
                GameObject queuedMove = MarkerPoolTransform.GetNextInactiveSibling ();
                if ( queuedMove != null ) {
                    queuedMove.transform.position = targetPosition;
                    queuedMove.transform.SetAsLastSibling ();
                    queuedMove.SetActive ( true );
                }
            } else {
                currentMove = GetAndReplaceQueueMarkerWithMoveMarker ( targetPosition );
                StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
            }
        }
        return null;
    }

    void InitialiseSubPoolContainerTransforms () {
        if ( AreSubPoolsInstantiated () == false ) {
            Debug.LogFormat ( gameObject, "instantiating subpool transforms for queue pool: {0}", MarkerPoolTransform.name );
            poolActive.InstantiateTransformWithParent(MarkerPoolTransform, "move_pool-active");
            poolInactve.InstantiateTransformWithParent(MarkerPoolTransform, "move_pool-inactive");
        }
    }

    GameObject GetAndReplaceQueueMarkerWithMoveMarker ( Vector3 position ) {
        if ( moveMarker == null ) {
            moveMarker = Instantiate<GameObject> ( MoveMarkerPrefab );
        }
        moveMarker.transform.position = position;
        moveMarker.SetActive ( true );
        return moveMarker;
    }

    IEnumerator SetMoveFlagAfterDelay ( float delay ) {
        yield return new WaitForSeconds ( delay );
        isExecutingMove = true;
    }

    bool AreSubPoolsInstantiated () {
        return poolActive == null && poolInactve == null;
    }
}
