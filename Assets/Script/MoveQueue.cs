using UnityEngine;

    /*
    ===
    ==
        queues positions for a mover to iterate over and move-to.
        utilizes object pooling and modeled as a linked-list.
    ==
    ===
    */

public class MoveQueue : MonoBehaviour {

    /* data container class that serves as a linked-list or priority-queue node */
    public class Move {

        public Move       Next              { get; set; }

        public GameObject MarkerObject     { get; set; }
        public Transform  MarkerTransform  { get { return MarkerObject.transform; } }
        public Vector3    Position         { get { return MarkerObject.transform.position; } }

        public int        ID               { get; set; }
        public bool       IsMasterMoveNode { get; set; }
        public bool       IsCurrentMove    { get; set; }
        public bool       IsMarkerActive   { get { return MarkerObject.activeInHierarchy; } }

        public Move ( GameObject markerPrefab, bool isMasterMoveNode, int id ) {
            ID = id;
            MarkerObject = markerPrefab;
            IsMasterMoveNode = isMasterMoveNode; // if this node is HEAD it is the master node
            if ( IsMasterMoveNode ) {
                MarkerObject.name = string.Format ( "[id: {0}] current_move_marker-(HEAD)", ID );
            } else {
                MarkerObject.name = string.Format ( "[id: {0}] queued_move_marker", ID );
            }
            IsCurrentMove = false;
        }

        public void ToggleMarkerVisibile ( bool shouldActivate ) {
            if ( MarkerObject != null ) {
                if ( shouldActivate ) {
                    MarkerObject.SetActive ( true );
                } else {
                    MarkerObject.SetActive ( false );
                }
            }
        }
    }

    /* references */
    [TooltipAttribute ( "~ the parent container for the marker pool and move queue-- warning: do NOT use a gameobject that changes position, use either the object this script is attached to or an empty gameobject" )]
    public Transform   MarkerPoolTransform   = null;
    [TooltipAttribute ( "~ this prefab is used to mark the current move-to target vector position" )]
    public GameObject  MoveMarkerPrefab      = null;
    [TooltipAttribute ( "~ this prefab is used to mark all moves in the move queue" )]
    public GameObject  QueueMarkerPrefab     = null;

    /* editor variables */
    [TooltipAttribute ( "~ adjusts the amount of time to wait before performing the next move" )]
    [RangeAttribute ( 0.1f, 10.1f )]
    public float        MoveDelay             = 0.35f;
    [TooltipAttribute ( "~ rate limits clicks registered as actual clicks" )]
    [RangeAttribute ( 0.1f, 1.1f )]
    public float        RateLimitInterval     = 0.25f;

    /* subscription events */
    public event System.Action<bool> StartMoveNow;

    /* public properties */
    public MoveQueue.Move Head                 { get; private set; }
    public MoveQueue.Move Tail                 { get; private set; }
    public MoveQueue.Move SentinalActiveHead   { get; private set; }
    public MoveQueue.Move SentinalInactiveHead { get; private set; }
    public MoveQueue.Move SentinalActiveTail   { get; private set; }
    public MoveQueue.Move SentinalInactiveTail { get; private set; }
    public string         OwnerName            { get; private set; } // make it easy to id the associated mover of this pool in the hierarchy
    public int            PoolBufferMaxSize    { get; private set; }

    /* local properties */
    float                 setLastClickTime  { get { return Time.time + RateLimitInterval; } }

    /* local variables */
    Transform          markerPoolParent      = null;
    Transform          markerSubpoolActive   = null;
    Transform          markerSubpoolInactive = null;
    float               timeSinceLastClicked  = 0.0f;

    /* use this as the class constructor -- performs all pool setup */
    public void CreateNewMoveQueuePool ( int poolBufferMaxSize, string ownerName = "move_queue: un-identified_owner", bool returnAllocation = false ) {
        OwnerName = ownerName;
        PoolBufferMaxSize = poolBufferMaxSize;
        InitialisePoolContainerTransforms ( MarkerPoolTransform );

        Debug.AssertFormat ( markerPoolParent != null, "{0} has not been assigned a marker pool transform reference!", gameObject.name );

        if ( PredicatePoolTransformsAreNull () == false ) {
            InitialiseMarkerObjectsAndLinkNodes ( MoveMarkerPrefab, QueueMarkerPrefab, markerSubpoolInactive );
        }
    }

    /* public wrapper method for calling the coroutine delay action */
    public void OnAfterSecondsSignalStartMove () {
        StartCoroutine ( RaiseAfterSecondsFlagNewMoveEvent ( MoveDelay, true ) );
    }

    /* returns a move or returns null if the move was queued instead */
    public MoveQueue.Move HandleMoveRequest (Vector3 targetPosition) {
        if (Time.time > timeSinceLastClicked) { // rate-limit input
            timeSinceLastClicked = setLastClickTime;
            if ( markerSubpoolInactive.childCount > 0 ) { // are there any move nodes available?
                MoveQueue.Move firstInactiveMove = GetFirstInactiveMove();
                if ( firstInactiveMove != null ) {
                    MoveQueue.Move move = MoveToActivePool ( firstInactiveMove, targetPosition );
                    if ( SentinalActiveHead == null ) {
                        SentinalActiveHead = move;
                        return move;
                    }
                }
            }
        }
        return null;
    }

    /* returns the next move in the queue, if any -- caller should always call on completing a move */
    public MoveQueue.Move HandleMoveEnd ( MoveQueue.Move completedMoveNode ) {
        MoveQueue.Move completedMove = MoveToInactivePool ( completedMoveNode );
        SentinalInactiveTail = completedMove;
        SentinalActiveHead = completedMove.Next;
        if (SentinalActiveHead != null) {
            return SentinalActiveHead;
        }
        Debug.Log("return null");
        return null;
    }

    MoveQueue.Move GetFirstActiveMove () {
        if (SentinalActiveHead == null) { // no moves to pop
            return null;
        } else {
            MoveQueue.Move currMove = SentinalActiveHead;
            SentinalActiveHead = SentinalActiveHead.Next;
            return currMove;
        }
    }

    MoveQueue.Move GetFirstInactiveMove () {
        if (SentinalInactiveHead == null) { // no moves to queue
            return null;
        } else {
            MoveQueue.Move queuedMove = SentinalInactiveHead;
            SentinalInactiveHead = SentinalInactiveHead.Next;
            return queuedMove;
        }
    }

    MoveQueue.Move MoveToActivePool (MoveQueue.Move move, Vector3 position) {
        move.MarkerTransform.parent = markerSubpoolActive;
        move.MarkerTransform.position = position;
        move.MarkerTransform.SetAsLastSibling ();
        move.ToggleMarkerVisibile ( true );
        return move;
    }

    MoveQueue.Move MoveToInactivePool (MoveQueue.Move move) {
        move.ToggleMarkerVisibile ( false );
        move.MarkerTransform.parent = markerSubpoolInactive;
        move.MarkerTransform.position = Vector3.zero;
        move.MarkerTransform.SetAsLastSibling ();
        return move;
    }

    /* instantiate the transforms for active and inactive pooled objects */
    void InitialisePoolContainerTransforms ( Transform parent ) {
        if ( PredicatePoolTransformsAreNull () == true ) {
            markerPoolParent = MarkerPoolTransform;
            markerSubpoolActive = markerSubpoolActive.InstantiateTransformWithParent ( parent, "move_pool-active" );
            markerSubpoolInactive = markerSubpoolInactive.InstantiateTransformWithParent ( parent, "move_pool-inactive" );
        }
    }

    /* each iteration of the while loop is executing a linked-list add() operation */
    void InitialiseMarkerObjectsAndLinkNodes ( GameObject moveMarker, GameObject queuedMoveMarker, Transform parentTransform ) {
        int i = 0;
        while ( i < ( PoolBufferMaxSize + 1 ) ) { // add one to account for HEAD
            GameObject markerObject = null;
            if ( Head == null ) {
                markerObject = moveMarker.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false );
                Head = new MoveQueue.Move ( markerObject, true, i );
                Tail = Head;
            } else {
                markerObject = queuedMoveMarker.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false );
                Tail.Next = new MoveQueue.Move ( markerObject, false, i );
                Tail = Tail.Next;
            }
            i++;
        }
        Tail.Next = Head;
        SentinalInactiveTail = Tail;
        SentinalInactiveHead = Head;
        SentinalActiveTail = null;
        SentinalActiveHead = null;
    }

    /* returns true if all the container pool transforms are null, false otherwise */
    bool PredicatePoolTransformsAreNull () {
        return markerPoolParent == null && markerSubpoolActive == null && markerSubpoolInactive == null;
    }

    /* notify the mover to start their move action */
    System.Collections.IEnumerator RaiseAfterSecondsFlagNewMoveEvent ( float delay, bool signal ) {
        yield return new WaitForSeconds ( delay );
        if ( StartMoveNow != null ) {
            StartMoveNow ( signal );
        }
    }
}