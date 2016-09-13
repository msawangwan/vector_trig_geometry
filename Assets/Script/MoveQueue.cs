using UnityEngine;
using System.Collections;

public class MoveQueue : MonoBehaviour {

    public Transform  MarkerPoolTransform  = null;
    public GameObject MoveMarkerPrefab     = null;
    public GameObject QueueMarkerPrefab    = null;
    public int        QueueBufferCapacity  = 10;
    public float       MoveSpeedMultiplier  = 5.0f;
    public float       MoveDelay            = 0.35f;
    public float       RateLimitInterval    = 0.25f;

    MoveController2D  mc                   = new MoveController2D ();
    GameObject        moveMarker           = null;
    GameObject        currentMove          = null;
    float              timeSinceLastClicked = 0.0f;
    bool              isExecutingMove      = false;

    Transform         moverTransform       { get { return gameObject.transform; } }
    float              setLastClickTime     { get { return Time.time + RateLimitInterval; } }

    void Start () {
        InitialiseMarkerPool ();
    }

    void Update () { // TODO: use a linked-list instead of for-loops
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
                } else if ( isExecutingMove == false ) {
                    currentMove = MoveMarkerToCurrentMove ( mousePos );
                    StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
                }
            }
        }
        if ( isExecutingMove ) {
            if ( mc.MoveUntilArrived(moverTransform, currentMove.transform.position, MoveSpeedMultiplier, Time.deltaTime ) ) {
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
            GameObject marker = Instantiate<GameObject>(QueueMarkerPrefab);
            marker.SetActive(false);
            marker.transform.SetParent(MarkerPoolTransform);
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
