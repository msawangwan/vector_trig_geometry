using UnityEngine;

    /*
    ===
    ==
        queues positions for a mover to iterate over and move-to.
        modeled as a linked-list
    ==
    ===
    */

public class MoveQueue : MonoBehaviour {

    /* data container class that serves as a linked-list or priority queue node */
    public class Move {
        public int        ID               { get; set; }
        public bool       IsCurrent        { get; set; }
        public Move       Next             { get; set; }
        public Move       Previous         { get; set; }
        public GameObject MarkerObject     { get; set; }
        public Transform  MarkerTransform  { get { return MarkerObject.transform; } }
        public Vector3    Position         { get { return MarkerObject.transform.position; } }
        public bool       IsActive         { get { return MarkerObject.activeInHierarchy; } }
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
    public string         MoverName         { get; private set; } // who does this pool belong to?
    public int            PoolBufferMaxSize { get; private set; }

    /* local properties */
    float                  setLastClickTime  { get { return Time.time + RateLimitInterval; } }

    /* local variables */
    Transform          markerSubpoolActive   = null;
    Transform          markerSubpoolInactive = null;
    GameObject         moveMarker            = null;
    GameObject         currentMove           = null;
    float               timeSinceLastClicked  = 0.0f;
    bool               isExecutingMove       = false;

    public void CreateNewMoveQueuePool ( int poolBufferMaxSize, string owner = "move_queue: un-identified_owner", bool returnAllocation = false ) {
        MoverName = owner;
        PoolBufferMaxSize = poolBufferMaxSize;
        InitialiseSubPoolContainerTransforms ( MarkerPoolTransform );
        if ( markerSubpoolActive != null && markerSubpoolInactive != null ) {
            InitialisePooledObjectLinks ( QueueMarkerPrefab, markerSubpoolInactive );
        }
    }

    public MoveQueue.Move GetNextMove ( Vector3 targetPosition ) {
        Debug.LogFormat("movequeue: move requested {0}, {1}, {2}", timeSinceLastClicked, Time.time, isExecutingMove);
        if ( Time.time > timeSinceLastClicked ) { // rate-limit saftey
            timeSinceLastClicked = setLastClickTime;
            GameObject m = null;
            MoveQueue.Move n = Head;
            while (n != null) {
                if (n.IsActive) {
                    m = n.MarkerObject;
                    break;
                }
                n = n.Next;
            }
            if (m == null) {
                return null;
            }
            return null; // TODO: NOT IMPLEMENTED YET THIS IS WHERE WORK NEEDS TO RESUME!!


            
            GameObject moveMarkerGameObject = markerSubpoolInactive.GetNextInactiveSibling ();
            if ( moveMarkerGameObject != null ) {
                moveMarkerGameObject.transform.parent = markerSubpoolActive;
                moveMarkerGameObject.transform.position = targetPosition;
                if ( isExecutingMove ) {
                    moveMarkerGameObject.transform.SetAsLastSibling ();
                    moveMarkerGameObject.SetActive ( true );
                } else {
                    Move move = moveMarkerGameObject.GetComponent<Move> ();
                    move.MarkerObject = GetAndReplaceQueueMarkerWithMoveMarker ( move.MarkerObject, targetPosition );
                    StartCoroutine ( RaiseAfterSecondsFlagNewMoveEvent ( MoveDelay ) );
                    return move;
                }
            }
        }
        return null;
    }

    void InitialiseSubPoolContainerTransforms ( Transform parent ) {
        if ( PredicateSubpoolsAreNull () ) {
            markerSubpoolActive = markerSubpoolActive.InstantiateTransformWithParent ( parent, "move_pool-active" );
            markerSubpoolInactive = markerSubpoolInactive.InstantiateTransformWithParent ( parent, "move_pool-inactive" );
        }
    }

    /* a custom linked-list add() operation */
    void InitialisePooledObjectLinks ( GameObject markerPrefab, Transform parentTransform ) {
        int i = 0;
        while ( i < PoolBufferMaxSize ) {
            MoveQueue.Move pooledQueuedMoveMarker = new MoveQueue.Move { 
                MarkerObject = markerPrefab.InstantiateAtPositionWithParent<GameObject> ( parentTransform, Vector3.zero, false ),
                IsCurrent = false,
                ID = i
            };
            if ( Head == null ) {
                Head = pooledQueuedMoveMarker;
                Tail = Head;
            } else {
                Tail.Next = pooledQueuedMoveMarker;
                Tail = Tail.Next;
            }
            i++;
        }
    }

    bool PredicateSubpoolsAreNull () {
        return markerSubpoolActive == null && markerSubpoolInactive == null;
    }

    GameObject GetAndReplaceQueueMarkerWithMoveMarker ( GameObject queueMarker, Vector3 position ) {
        queueMarker.SetActive(false);
        queueMarker.transform.parent = markerSubpoolInactive;
        queueMarker.GetComponent<Move> ().IsCurrent = false;
        if ( moveMarker == null ) {
            moveMarker = Instantiate<GameObject> ( MoveMarkerPrefab );
        }
        moveMarker.transform.position = position;
        moveMarker.SetActive ( true );
        // TODO: add is current?
        return moveMarker;
    }

    System.Collections.IEnumerator RaiseAfterSecondsFlagNewMoveEvent ( float delay ) {
        yield return new WaitForSeconds ( delay );
        isExecutingMove = true;
        if (StartMoveNow != null) {
            StartMoveNow(isExecutingMove);
        }
    }
}