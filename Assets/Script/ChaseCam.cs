using UnityEngine;

public class ChaseCam : MonoBehaviour {

    const int xPerimeter = 5;
    const int yPerimeter = 5;
    const float zDepth = -10f;

    public Transform ChaseTarget;
    
    bool chasing = false;
    float t = 0f;

    Vector3 cameraPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;
    Vector3 vel = Vector3.zero;
    Vector3 interpolation = Vector3.zero;

    void Update() {

        cameraPos = transform.position;
        targetPos = ChaseTarget.position;

        if (CheckBounds (cameraPos, targetPos) && !chasing) {
            transform.position = cameraPos;
        } else t = StartChase(cameraPos, targetPos);

        if (chasing) {
            if (Chase(cameraPos, targetPos, vel, t)) {
                chasing = false;
            }
        }
    }

    float StartChase (Vector3 start, Vector3 end) {
        chasing = true;

        cameraPos = start;
        targetPos = end;
        end.z = zDepth;

        return Time.deltaTime;
    }

    bool Chase (Vector3 s, Vector3 e, Vector3 v, float t) {
        float p = (Time.deltaTime - t ) / 1.0f;
        e.z = zDepth;
        transform.position = Vector3.SmoothDamp(s, e, ref v, t);

        if (p >= 1.0f) return true;
        else return false;
    }

    Vector3 GetInterpolationVector (Vector3 startingFrom, Vector3 to, Vector3 v, float t = 0.25f) {
        to.z = zDepth;
        return Vector3.SmoothDamp ( startingFrom, to, ref v, t );
    }

    bool CheckBounds (Vector3 a, Vector3 b) {
        Debug.Log("a" + (a.x + xPerimeter));
        Debug.Log("b" + b.x);
        return ( ( a.x + xPerimeter ) < b.x ) || ( ( a.y + yPerimeter ) < b.y );
    }
}
