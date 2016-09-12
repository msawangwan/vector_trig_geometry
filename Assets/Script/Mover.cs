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

    [TooltipAttribute ( "~ assign a gameobject that will serve as a parent container for all marker objects -- tip: use an empty gameobject" )]
    public Transform  MarkerContainer        = null;
    [TooltipAttribute ( "~ this prefab is used to mark the current move-to target vector position" )]
    public GameObject MoveMarkerPrefab       = null;
    [TooltipAttribute ( "~ this prefab is used to mark all moves in the move queue" )]
    public GameObject QueuedMoveMarkerPrefab = null;

    [TooltipAttribute ( "~ maximum number of moves allowed to be queued up" )]
    [RangeAttribute ( 1, 50 )]
    public int        MoveBufferCapacity     = 10;
    [TooltipAttribute ( "~ adjusts the move-to time of the moving object" )]
    [RangeAttribute ( 0.1f, 1000.1f )]
    public float       MoveSpeedMultiplier    = 5.0f;
    [TooltipAttribute ( "~ adjusts the amount of time to wait before performing the next move" )]
    [RangeAttribute ( 0.1f, 10.1f )]
    public float       MoveDelay              = 0.15f;
    [TooltipAttribute ( "~ rate limits clicks registered as actual clicks" )]
    [RangeAttribute ( 0.1f, 1.1f )]
    public float       ClickInterval          = 0.5f;

    GameObject        moveMarkerObject       = null;
    MoveController2D  mc                     = new MoveController2D();
    Vector3           currentMovePos         = Vector3.zero;
    float              timeSinceLastClick     = 0.0f;
    bool              isMoving               = false;

    Transform         mover                  { get { return gameObject.transform; } }
    GameObject        moveMarker             { get { return MarkerContainer.GetChild ( 0 ).gameObject; } }
    Vector3           moveMarkerPos          { get { return moveMarkerObject.transform.position; } }
    int               markerCount            { get { return MarkerContainer.childCount; } }
    float              setLastClickTime       { get { return Time.time + ClickInterval; } }

    void Update () {
        if ( MarkerContainer == null ) {
            return;
        }
        if ( Input.GetMouseButton ( 0 ) ) {
            if ( Time.time > timeSinceLastClick ) { // rate limit saftey
                timeSinceLastClick = setLastClickTime;
                if ( markerCount < MoveBufferCapacity ) {
                    GameObject marker = QueuedMoveMarkerPrefab.InstantiateAtPosition ( MousePointer.Pos () ); // TODO: pool rather than instantiate/destroy
                    marker.transform.SetParent ( MarkerContainer );
                    marker.transform.SetAsLastSibling ();
                    if ( isMoving == false && ( moveMarkerObject == null && markerCount == 0 ) ) {
                        moveMarkerObject = moveMarker;
                        currentMovePos = moveMarkerPos;
                        StartCoroutine ( BeginMoveAfterDelay ( MoveDelay ) );
                    }
                }
            }
        }
        if ( isMoving || markerCount > 0 ) {
            if ( moveMarkerObject == null ) {
                moveMarkerObject = moveMarker;
                currentMovePos = moveMarkerPos;
            }
            if ( mc.MoveUntilArrived ( mover, currentMovePos, MoveSpeedMultiplier, Time.deltaTime ) ) {
                if ( moveMarkerObject != null ) {
                    Destroy ( moveMarkerObject );
                    if ( markerCount > 0 && moveMarkerObject == null ) {
                        moveMarkerObject = moveMarker;
                        currentMovePos = moveMarkerPos;
                    } else {
                        isMoving = false;
                    }
                }
            }
            mc.RotateUntilFacingTarget ( mover, currentMovePos );
        }
    }

    IEnumerator BeginMoveAfterDelay ( float delay ) {
        yield return new WaitForSeconds ( delay );
        isMoving = true;
    }
}