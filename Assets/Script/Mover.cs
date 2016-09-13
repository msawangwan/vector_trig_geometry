using UnityEngine;
using System.Collections;

    /*
    ===
    ==
        moves an object to a position designated by a move marker. will queue up subsequent move commands
        and executes them in order.
    ==
    ===
    */

public class Mover : MonoBehaviour {

    /* references */
    [TooltipAttribute ( "~ assign a gameobject that will serve as a parent container for all marker objects -- tip: use an empty gameobject" )]
    public Transform  MarkerQueueContainer   = null;
    [TooltipAttribute ( "~ this prefab is used to mark the current move-to target vector position" )]
    public GameObject MoveMarkerPrefab       = null;
    [TooltipAttribute ( "~ this prefab is used to mark all moves in the move queue" )]
    public GameObject QueuedMoveMarkerPrefab = null;

    /* editor variables */
    [TooltipAttribute ( "~ maximum number of moves allowed to be queued up" )]
    [RangeAttribute ( 1, 50 )]
    public int        MoveBufferCapacity     = 10;
    [TooltipAttribute ( "~ adjusts the move-to time of the moving object" )]
    [RangeAttribute ( 0.1f, 1000.1f )]
    public float      MoveSpeedMultiplier    = 5.0f;
    [TooltipAttribute ( "~ adjusts the amount of time to wait before performing the next move" )]
    [RangeAttribute ( 0.1f, 10.1f )]
    public float      MoveDelay              = 0.15f;
    [TooltipAttribute ( "~ rate limits clicks registered as actual clicks" )]
    [RangeAttribute ( 0.1f, 1.1f )]
    public float      ClickInterval          = 0.5f;

    /* local variables */
    GameObject        moveMarker             = null;
    MoveController2D  mc                     = new MoveController2D();
    Vector3           targetPosition         = Vector3.zero;
    float             timeSinceLastClick     = 0.0f;
    bool              isMoving               = false;

    /* local properties */
    Transform         mover                  { get { return gameObject.transform; } }
    GameObject        getNextMarker          { get { return MarkerQueueContainer.GetChild ( 0 ).gameObject; } }
    Vector3           moveMarkerPos          { get { return moveMarker.transform.position; } }
    int               markerCount            { get { return MarkerQueueContainer.childCount; } }
    float             setLastClickTime       { get { return Time.time + ClickInterval; } }

    void Start () {
        CreateMarkerPool(MoveBufferCapacity);
    }

    void Update () {
        if ( MarkerQueueContainer == null ) {
            return; // TODO: create one rather than return?
        }
        if ( Input.GetMouseButton ( 0 ) ) {
            if ( Time.time > timeSinceLastClick ) { // rate limit saftey
                timeSinceLastClick = setLastClickTime;
                if ( markerCount < MoveBufferCapacity ) {
                    GameObject marker = QueuedMoveMarkerPrefab.InstantiateAtPosition ( MousePointer.Pos () ); // TODO: pool rather than instantiate/destroy
                    marker.transform.SetParent ( MarkerQueueContainer );
                    marker.transform.SetAsLastSibling ();
                    if ( isMoving == false && ( moveMarker == null && markerCount == 0 ) ) {
                        moveMarker = getNextMarker;
                        targetPosition = moveMarkerPos;
                        StartCoroutine ( BeginMoveAfterDelay ( MoveDelay ) );
                    }
                }
            }
        }
        if ( isMoving || markerCount > 0 ) {
            if ( moveMarker == null ) {
                moveMarker = getNextMarker;
                targetPosition = moveMarkerPos;
            }
            if ( mc.MoveUntilArrived ( mover, targetPosition, MoveSpeedMultiplier, Time.deltaTime ) ) {
                if ( moveMarker != null ) {
                    Destroy ( moveMarker );
                    if ( markerCount > 0 && moveMarker == null ) {
                        moveMarker = getNextMarker;
                        targetPosition = moveMarkerPos;
                    } else {
                        isMoving = false;
                    }
                }
            }
            mc.RotateUntilFacingTarget ( mover, targetPosition );
        }
    }

    IEnumerator BeginMoveAfterDelay ( float delay ) {
        yield return new WaitForSeconds ( delay );
        isMoving = true;
    }

    void CreateMarkerPool (int queuedMovesMax) {
        for (int i = 0; i < queuedMovesMax; i++) {
            GameObject go = Instantiate<GameObject>(QueuedMoveMarkerPrefab);
            go.SetActive(false);
            go.transform.SetParent(PlayerAssetContainer.s.transform);
        }
    }
}