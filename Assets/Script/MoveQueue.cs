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

        public GameObject MoveMarkerObject      { get; set; }
        public GameObject QueuedMarkerObject    { get; set; }
        public Transform  MoveMarkerTransform   { get { return MoveMarkerObject.transform; } } // need to add null checks for all of these
        public Transform  QueuedMarkerTransform { get { return QueuedMarkerObject.transform; } }
        public Vector3    Position              { get { return MoveMarkerObject.transform.position; } }
        public Vector3    QueuedPosition        { get { return QueuedMarkerObject.transform.position; } }

        public int        ID                    { get; set; }
        public bool       IsMasterMoveNode      { get; set; }
        public bool       IsMoveMarkerActive    { get { return MoveMarkerObject.activeInHierarchy; } }
        public bool       IsQueueMarkerActive   { get { return QueuedMarkerObject.activeInHierarchy; } }

        public Move       Next                  { get; set; }
        public Move       Previous              { get; set; } // do we need this??

        public Move () {
            QueuedMarkerObject.name = "queued_move_marker [id: " + ID + "]";
            MoveMarkerObject = null;
        }

        public Move ( GameObject markerPrefab, bool isHeadNode, int id ) {
            ID = id;
            IsMasterMoveNode = isHeadNode;
            if ( IsMasterMoveNode ) {
                QueuedMarkerObject = null;
                MoveMarkerObject = markerPrefab;
                MoveMarkerObject.name = string.Format ( "[id: {0}] current_move_marker-(HEAD)", ID );
            } else {
                MoveMarkerObject = null;
                QueuedMarkerObject = markerPrefab;
                QueuedMarkerObject.name = string.Format ( "[id: {0}] queued_move_marker", ID );
            }
        }

        public void ToggleMoveMarkerVisibile ( bool shouldActivate ) {
            if ( MoveMarkerObject != null ) {
                if ( shouldActivate ) {
                    MoveMarkerObject.SetActive ( true );
                } else {
                    MoveMarkerObject.SetActive ( false );
                }
            }
        }

        public void ToggleQueueMarkerVisibile ( bool shouldActivate ) {
            if ( QueuedMarkerObject != null ) {
                if ( shouldActivate ) {
                    QueuedMarkerObject.SetActive ( true );
                } else {
                    QueuedMarkerObject.SetActive ( false );
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
    public MoveQueue.Move Head              { get; private set; }
    public MoveQueue.Move Tail              { get; private set; }
    public MoveQueue.Move Sentinal          { get; private set; } // keep a ptr reference to the node of the most recent queued move, is null if not moving
    public string         OwnerName         { get; private set; } // make it easy to id the associated mover of this pool in the hierarchy
    public int            PoolBufferMaxSize { get; private set; }

    /* local properties */
    float                 setLastClickTime  { get { return Time.time + RateLimitInterval; } }

    /* local variables */
    Transform          markerPoolParent      = null;
    Transform          markerSubpoolActive   = null;
    Transform          markerSubpoolInactive = null;
    float              timeSinceLastClicked  = 0.0f;
    bool               isExecutingMove       = false;

    public void CreateNewMoveQueuePool ( int poolBufferMaxSize, string ownerName = "move_queue: un-identified_owner", bool returnAllocation = false ) {
        OwnerName = ownerName;
        PoolBufferMaxSize = poolBufferMaxSize;
        InitialisePoolContainerTransforms ( MarkerPoolTransform );

        Debug.AssertFormat ( markerPoolParent != null, "{0} has not been assigned a marker pool transform reference!", gameObject.name );

        if ( PredicatePoolTransformsAreNull () == false ) {
            InitialiseMarkerObjectsAndLinkNodes ( MoveMarkerPrefab, QueueMarkerPrefab, markerSubpoolInactive );
        }
    }

    public MoveQueue.Move GetNextMove ( Vector3 targetPosition ) {
        Debug.LogFormat("move requested: [last: {0}], [curr: {1}], [isExecuting {2}]", timeSinceLastClicked, Time.time, isExecutingMove);
        if ( Time.time > timeSinceLastClicked ) { // should we treat this as a valid click?
            timeSinceLastClicked = setLastClickTime;
            if ( Head.IsMoveMarkerActive == true && Sentinal != null ) { // we're currently executing a move, so queue the current move
                MoveQueue.Move queuedMoveNode = Sentinal.Next;
                queuedMoveNode.QueuedMarkerTransform.position = targetPosition;
                queuedMoveNode.QueuedMarkerTransform.parent = markerSubpoolActive;
                queuedMoveNode.QueuedMarkerTransform.SetAsLastSibling ();
                queuedMoveNode.ToggleQueueMarkerVisibile ( true );
                Sentinal = queuedMoveNode;
            } else {
                Head.MoveMarkerTransform.position = targetPosition;
                Head.ToggleMoveMarkerVisibile ( true );
                Sentinal = Head.Next;
                StartCoroutine ( RaiseAfterSecondsFlagNewMoveEvent ( MoveDelay ) );
                return Head;
            }
        }
        return null;
    }

    public MoveQueue.Move CheckForQueuedMove () {
        return null;
    }

    void InitialisePoolContainerTransforms ( Transform parent ) {
        if ( PredicatePoolTransformsAreNull () == true ) {
            markerPoolParent = MarkerPoolTransform;
            markerSubpoolActive = markerSubpoolActive.InstantiateTransformWithParent ( parent, "move_pool-active" );
            markerSubpoolInactive = markerSubpoolInactive.InstantiateTransformWithParent ( parent, "move_pool-inactive" );
        }
    }

    /* a custom linked-list add() operation (original DEPRECATED version) */
    void InitialisePooledObjectLinks ( GameObject markerPrefab, Transform parentTransform ) {
        int i = 0;
        while ( i < PoolBufferMaxSize ) {
            MoveQueue.Move pooledQueuedMoveMarker = new MoveQueue.Move { // pair a gameobject with the move reference node
                QueuedMarkerObject = markerPrefab.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false ),
                IsMasterMoveNode = false,
                ID = i
            };
            if ( Head == null ) {
                Head = pooledQueuedMoveMarker;
                Head.Previous = null; // dno if i will need this but put it in for now
                Tail = Head;
            } else {
                Tail.Next = pooledQueuedMoveMarker;
                Tail.Previous = Tail; // dno if i will need this but put it in for now
                Tail = Tail.Next;
            }
            i++;
        }
    }

    /* each iteration of the while loop is executing a linked-list add() operation */
    void InitialiseMarkerObjectsAndLinkNodes ( GameObject moveMarker, GameObject queuedMoveMarker, Transform parentTransform ) {
        Sentinal = null;
        int i = 0;
        while ( i < PoolBufferMaxSize + 1 ) { // add one to account for the actual active current move (will always be = to HEAD)
            GameObject markerObject = null;
            if ( Head == null ) {
                markerObject = moveMarker.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false );
                Head = new MoveQueue.Move ( markerObject, true, i );
                Head.Previous = null; // dno if i will need this but put it in for now
                Tail = Head;
            } else {
                markerObject = queuedMoveMarker.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false );
                Tail.Next = new MoveQueue.Move ( markerObject, false, i );
                Tail.Previous = Tail; // dno if i will need this but put it in for now
                Tail = Tail.Next;
            }
            i++;
        }
    }

    /* returns true if all the container pool transforms are null, false otherwise */
    bool PredicatePoolTransformsAreNull () {
        return markerPoolParent == null && markerSubpoolActive == null && markerSubpoolInactive == null;
    }

    MoveQueue.Move GetNextInactive ( MoveQueue.Move node ) {
        MoveQueue.Move move = null;
        while ( node != null ) { // find an inactive node
            if ( node.IsQueueMarkerActive == false ) {
                move = node;
                break;
            }
            node = node.Next;
        }
        return move;
    }

    System.Collections.IEnumerator RaiseAfterSecondsFlagNewMoveEvent ( float delay ) {
        yield return new WaitForSeconds ( delay );
        isExecutingMove = true;
        if (StartMoveNow != null) {
            StartMoveNow(isExecutingMove);
        }
    }
}