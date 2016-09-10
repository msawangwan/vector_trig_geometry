using UnityEngine;

public class PlayerAgent : MonoBehaviour
{

    public Vector3 Position { get { return transform.position; } }

    Vector3 targetPosition { get; set; }
    Vector3 lerpPosition { get; set; }
    Vector3 toPosition { get; set; }
    Vector3 velocity { get; set; }
    Vector3 heading { get { return toPosition.normalized; } }

    float speed { get { return velocity.magnitude; } }
    float speedMultiplier { get { return 5.0f; } }
    float maxSpeed { get { return 3.0f; } }

    float tStart { get; set; }
    float tElapsed { get; set; }
    float t { get; set; }
    float dt { get { return Time.deltaTime; } }
}