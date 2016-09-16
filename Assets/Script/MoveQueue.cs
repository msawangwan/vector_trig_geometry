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

        public int        Priority         { get; private set; }
        public Move       Next             { get; private set; }
        public Move       Previous         { get; private set; }
        public GameObject MarkerObject     { get; private set;}
        public Transform  MarkerTransform  { get { return MarkerObject.transform; } }
        public Vector3    Position         { get { return MarkerObject.transform.position; } }

        public Move () {}

        public Move ( GameObject markerObject, int priority, Move next = null, Move previous = null ) {
            MarkerObject = markerObject;
            Priority = priority;
            Next = next;
            Previous = previous;
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
    MoveController     mc                    = new MoveController ();
    Transform          markerSubpoolActive   = null;
    Transform          markerSubpoolInactive = null;
    GameObject         moveMarker            = null;
    GameObject         currentMove           = null;
    float              timeSinceLastClicked  = 0.0f;
    bool               isExecutingMove       = false;

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
    const int indexSwap = maxBufferSize + 100;

    public MoveQueue.Move Head = null;
    public MoveQueue.Move Tail = null;

    public event System.Action<bool> StartMoveNow;

    public GameObject[] markers = null;

    public string MoverName         { get; private set; }
    public int    PoolBufferMaxSize { get; private set; }

    public GameObject[] CreateNewMoveQueuePool ( int poolBufferMaxSize, string owner = "move_queue: un-identified_owner", bool returnAllocation = false ) {
        MoverName = owner;
        PoolBufferMaxSize = poolBufferMaxSize;
        InitialiseSubPoolContainerTransforms ( MarkerPoolTransform );
        if ( markerSubpoolActive != null && markerSubpoolInactive != null ) {
            switch (returnAllocation) {
                case true:
                    markers = QueueMarkerPrefab.InstantiatePoolAlloc<GameObject> ( markerSubpoolInactive, maxBufferSize, false );
                    return markers;
                default:
                    QueueMarkerPrefab.InstantiatePool<GameObject> ( markerSubpoolInactive, maxBufferSize, false );
                    return null;
            }
        } else { Debug.LogError ( "movequeue has no subpools for active and inactive pooled objects" ); }
        return null;
    }

    public MoveQueue.Move GetNextMove ( Vector3 targetPosition ) {
        Debug.LogFormat("movequeue: move requested {0}, {1}, {2}", timeSinceLastClicked, Time.time, isExecutingMove);
        if ( Time.time > timeSinceLastClicked ) { // rate-limit saftey
            timeSinceLastClicked = setLastClickTime;
            if ( isExecutingMove ) {
                GameObject queuedMove = markerSubpoolInactive.GetNextInactiveSibling ();
                if ( queuedMove != null ) {
                    queuedMove.transform.position = targetPosition;
                    queuedMove.transform.SetAsLastSibling ();
                    queuedMove.SetActive ( true );
                }
            } else {
                currentMove = GetAndReplaceQueueMarkerWithMoveMarker ( targetPosition );
                StartCoroutine ( RaiseAfterSecondsFlagNewMoveEvent ( MoveDelay ) );
                return new MoveQueue.Move (currentMove, indexFirst);
            }
        }
        return null;
    }

    void InitialiseSubPoolContainerTransforms ( Transform parent ) {
        if ( PredicateSubpoolsAreNull () ) {
            markerSubpoolActive = markerSubpoolActive.InstantiateTransformWithParent ( parent, "move_pool-active" );
            markerSubpoolInactive = markerSubpoolInactive.InstantiateTransformWithParent ( parent, "move_pool-inactive" );
            Debug.LogFormat ( gameObject, "instantiated subpool transforms \n\t queue pool: [{0}] \n\t owned by: [{1}]", parent.name, MoverName );
        }
    }

    void EnqueueMove ( MoveQueue.Move move ) {
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

    MoveQueue.Move DequeueNextMove ( Vector3 targetPosition ) {
        return null;
    }

    GameObject GetAndReplaceQueueMarkerWithMoveMarker ( Vector3 position ) {
        if ( moveMarker == null ) {
            moveMarker = Instantiate<GameObject> ( MoveMarkerPrefab );
        }
        moveMarker.transform.position = position;
        moveMarker.SetActive ( true );
        return moveMarker;
    }

    IEnumerator RaiseAfterSecondsFlagNewMoveEvent ( float delay ) {
        yield return new WaitForSeconds ( delay );
        if (StartMoveNow != null) {
            StartMoveNow(true);
        }
        isExecutingMove = true;
    }

    bool PredicateSubpoolsAreNull () {
        return markerSubpoolActive == null && markerSubpoolInactive == null;
    }
}
