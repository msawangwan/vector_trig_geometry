using UnityEngine;
using System.Collections;

public class MoveQueue : MonoBehaviour {

    // has a pool of x markers
    // markers default to available (ie, inactive)

    // on each update we:
    // start by checking:
    //      any moves left to execute? (ie, active markers)
    //      if yes:
    //          set target position to the next active marker
    //      if no:
    //          wait for a mouse click
    // if the mouse is clicked check:
    //      are we currently executing a move?
    //      if yes:
    //          do we have an available marker? (ie, inactive) <- Call GetNextAvailableMove
    //          if yes:
    //              take the next available marker and set as active
    //          if no:
    //              that means we have moves to finish
    //      if no:
    //          we can move to the click position

    public Transform MarkerPoolTransform = null;
    public Transform MarkerQueueTransform = null;
    public GameObject MoveMarkerPrefab = null;
    public GameObject QueueMarkerPrefab = null;
    public int QueueBufferCapacity = 10;
    public float MoveSpeedMultiplier = 5.0f;
    public float MoveDelay = 0.15f;
    public float RateLimitInterval = 0.5f;

    MoveController2D mc = new MoveController2D ();
    Vector3 targetMove = Vector3.zero;
    float timeSinceLastClicked = 0.0f;
    bool isExecutingMove = false;
    bool hasMove = false;

    Transform moverTransform { get { return gameObject.transform; } }
    float setLastClickTime { get { return Time.time + RateLimitInterval; } }
    float t { get { return Time.time; } }
    float dt { get { return Time.deltaTime; } }

    void Start () {
        InitialiseMarkerPool ();
    }

    void Update () {
        Debug.Log(hasMove + "\n" + isExecutingMove);
        if ( hasMove == false ) {
            GameObject move = GetNextMoveToExecute (); // returns an active move marker
            if ( move != null ) {
                targetMove = move.transform.position;
                hasMove = true;
                move.SetActive(false);
            }
        }
        if ( Input.GetMouseButton ( 0 ) ) {
            if ( Time.time > timeSinceLastClicked ) {
                timeSinceLastClicked = setLastClickTime;
                Vector3 mousePos = MousePointer.Pos();
                if ( isExecutingMove ) {
                    GameObject queuedMove = GetNextAvailableMove ();
                    if ( queuedMove != null ) {
                        queuedMove.transform.position = mousePos;
                        queuedMove.SetActive(true);
                    }
                } else if (isExecutingMove == false && hasMove == false) {
                    targetMove = mousePos;
                    StartCoroutine ( SetMoveFlagAfterDelay ( MoveDelay ) );
                }
            }
        }
        if (isExecutingMove && hasMove) {
            if ( mc.MoveUntilArrived(moverTransform, targetMove, MoveSpeedMultiplier, dt ) ) {
                GameObject queuedMove = GetNextMoveToExecute();
                if ( queuedMove != null ) {
                    targetMove = queuedMove.transform.position;
                } else {
                    hasMove = false;
                    isExecutingMove = false;
                }
            }
            mc.RotateUntilFacingTarget ( moverTransform, targetMove );
        }
    }

    void InitialiseMarkerPool () {
        for (int i = 0; i < QueueBufferCapacity; i++) {
            GameObject marker = Instantiate<GameObject>(QueueMarkerPrefab);
            marker.SetActive(false);
            marker.transform.SetParent(MarkerPoolTransform);
        }
    }

    GameObject GetNextMoveToExecute () {
        for (int i = 0; i < MarkerPoolTransform.childCount; i++) {
            GameObject curr = MarkerPoolTransform.GetChild(i).gameObject;
            if (curr.activeInHierarchy) {
                return curr;
            }
        }
        return null;
    }

    GameObject GetNextAvailableMove () {
        for (int i = 0; i < MarkerPoolTransform.childCount; i++) {
            GameObject curr = MarkerPoolTransform.GetChild(i).gameObject;
            if (curr.activeInHierarchy == false) {
                return curr;
            }
        }
        return null;
    }

    IEnumerator SetMoveFlagAfterDelay ( float delay ) {
        yield return new WaitForSeconds ( delay );
        hasMove = true;
        isExecutingMove = true;
    }
}
