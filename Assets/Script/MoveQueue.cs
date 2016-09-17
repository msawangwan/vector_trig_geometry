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

        public GameObject MarkerObject     { get; set; }
        public Transform  MarkerTransform  { get { return MarkerObject.transform; } }
        public Vector3    Position         { get { return MarkerObject.transform.position; } }

        public Move       Next              { get; set; }
        public Move       Previous          { get; set; }

        public int        ID               { get; set; }
        public bool       IsMasterMoveNode { get; set; }
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
    public MoveQueue.Move Head              { get; private set; }
    public MoveQueue.Move Tail              { get; private set; }
    public MoveQueue.Move SentinalActive    { get; private set; }
    public MoveQueue.Move SentinalInactive  { get; private set; } // sentinals should be null when not moving
    public string         OwnerName         { get; private set; } // make it easy to id the associated mover of this pool in the hierarchy
    public int            PoolBufferMaxSize { get; private set; }

    /* local properties */
    float                 setLastClickTime  { get { return Time.time + RateLimitInterval; } }

    /* local variables */
    Transform          markerPoolParent      = null;
    Transform          markerSubpoolActive   = null;
    Transform          markerSubpoolInactive = null;
    float               timeSinceLastClicked  = 0.0f;

    /* wrapper method for public access */
    public void OnSignalAfterSecondsStartMove () {
        StartCoroutine ( RaiseAfterSecondsFlagNewMoveEvent ( MoveDelay, true ) );
    }

    public MoveQueue.Move OnMoveEndCheckForNext ( MoveQueue.Move completedMoveNode ) {
        completedMoveNode.ToggleMarkerVisibile ( false );
        completedMoveNode.MarkerTransform.parent = markerSubpoolInactive;
        completedMoveNode.MarkerTransform.position = Vector3.zero;
        completedMoveNode.MarkerTransform.SetAsLastSibling ();
        if ( completedMoveNode.Next != null ) {
            if ( completedMoveNode.Next.IsMarkerActive == true ) {
                return completedMoveNode.Next;
            }
        } else {
            //SentinalInactive
        }
        return null;
    }

    public void CreateNewMoveQueuePool ( int poolBufferMaxSize, string ownerName = "move_queue: un-identified_owner", bool returnAllocation = false ) {
        OwnerName = ownerName;
        PoolBufferMaxSize = poolBufferMaxSize;
        InitialisePoolContainerTransforms ( MarkerPoolTransform );

        Debug.AssertFormat ( markerPoolParent != null, "{0} has not been assigned a marker pool transform reference!", gameObject.name );

        if ( PredicatePoolTransformsAreNull () == false ) {
            InitialiseMarkerObjectsAndLinkNodes ( MoveMarkerPrefab, QueueMarkerPrefab, markerSubpoolInactive );
        }
    }

    public MoveQueue.Move GetNextMoveFromInput ( Vector3 targetPosition ) {
        if ( Time.time > timeSinceLastClicked ) { // should we treat this call as a valid click?
            timeSinceLastClicked = setLastClickTime;
            if ( Head.IsMarkerActive == true && SentinalInactive != null ) {
                SentinalInactive.MarkerTransform.parent = markerSubpoolActive;
                SentinalInactive.MarkerTransform.position = targetPosition;
                SentinalInactive.MarkerTransform.SetAsLastSibling ();
                SentinalInactive.ToggleMarkerVisibile ( true );
                SentinalInactive = SentinalInactive.Next;
                return null;
            } else if ( Head.IsMarkerActive == false &&) {

            } else {
                Head.MarkerTransform.parent = markerSubpoolActive;
                Head.MarkerTransform.position = targetPosition;
                Head.MarkerTransform.SetAsFirstSibling ();
                Head.ToggleMarkerVisibile ( true );
                SentinalActive = Head;
                SentinalInactive = Head.Next; // set the ptr to the first queue node
                return Head;
            }
        } else {
            return null;
        }
    }

    void InitialisePoolContainerTransforms ( Transform parent ) {
        if ( PredicatePoolTransformsAreNull () == true ) {
            markerPoolParent = MarkerPoolTransform;
            markerSubpoolActive = markerSubpoolActive.InstantiateTransformWithParent ( parent, "move_pool-active" );
            markerSubpoolInactive = markerSubpoolInactive.InstantiateTransformWithParent ( parent, "move_pool-inactive" );
        }
    }

    /* each iteration of the while loop is executing a linked-list add() operation */
    void InitialiseMarkerObjectsAndLinkNodes ( GameObject moveMarker, GameObject queuedMoveMarker, Transform parentTransform ) {
        SentinalInactive = null;
        int i = 0;
        while ( i < PoolBufferMaxSize + 1 ) { // add one to account for the actual active current move (will always be = to HEAD)
            GameObject markerObject = null;
            if ( Head == null ) {
                markerObject = moveMarker.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false );
                Head = new MoveQueue.Move ( markerObject, true, i );
                Head.Previous = null;
                Tail = Head;
            } else {
                markerObject = queuedMoveMarker.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false );
                Tail.Next = new MoveQueue.Move ( markerObject, false, i );
                Tail.Previous = Tail;
                Tail = Tail.Next;
            }
            i++;
        }
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