using UnityEngine;
using System.Collections.Generic;

public static class SteeringBehaviour {

    /* arrival decel rate */
    public enum DecelerationRate { none = 0, slow = 3, normal = 2, fast = 1 }

	/* acos(0.95) = 18 degrees */
    public const float ACosErrMargin = 0.95f;

    /* forumla for calculating a seeking force */
    public static Vector3 Seek (this SteeringAgent seeker, SteeringAgent target) {
        return ( ( target.Position - seeker.Position).normalized * seeker.MaximumSpeed ) - seeker.Velocity;
    }

	/* forumla for calculating a seeking force (overload) */
    public static Vector3 Seek (this SteeringAgent seeker, Vector3 target) {
        return ( ( target - seeker.Position).normalized * seeker.MaximumSpeed ) - seeker.Velocity;
    }

    /* forumla for calculating a fleeing force */
    public static Vector3 Flee (this SteeringAgent fleeing, SteeringAgent target) {
        return ( ( fleeing.Position - target.Position ).normalized * fleeing.MaximumSpeed ) - fleeing.Velocity;
    }

    /* forumla for calculating a fleeing force (overload) */
    public static Vector3 Flee (this SteeringAgent fleeing, Vector3 target) {
        return ( ( fleeing.Position - target ).normalized * fleeing.MaximumSpeed ) - fleeing.Velocity;
    }

    /* formula for calculating a decelerating, arrival force */
    public static Vector3 Arrive ( this SteeringAgent arriving, Vector3 destination, DecelerationRate decelerationRate = DecelerationRate.normal ) {

        Vector3 toTarget = destination - arriving.Position;

        float d = toTarget.Lengthf();
        if ( d <= 0 ) return Vector3.zero;

        float speed = Mathf.Min ( ( d / ( (float) decelerationRate * 0.3f ) ), arriving.MaximumSpeed ); // multiply decelerationRate by a 'tweaker' value
        return (toTarget * speed / d) - arriving.Velocity;
    }

    public static Vector3 Pursue (this SteeringAgent pursuing, SteeringAgent evading) {

        Vector3 a = pursuing.Position;
        Vector3 b = evading.Position;
        Vector3 toEvader = b - a;

        float relHeading = Vector3.Dot ( a, b );
        float absHeading = Vector3.Dot ( a, toEvader );

        if ( absHeading > 0 && relHeading < -ACosErrMargin ) { // acos(0.95) = 18 degrees
            return pursuing.Seek ( b ); // we're head2head so just seek to target
        }

        float tLookAhead = toEvader.Lengthf () / pursuing.MaximumSpeed + evading.MaximumSpeed;
        return pursuing.Seek(b + evading.Velocity * tLookAhead);
    }

	public static Vector3 Evade (this SteeringAgent evading, SteeringAgent pursuing) {

        Vector3 a = evading.Position;
        Vector3 b = pursuing.Position;
        Vector3 toPursuer = b - a;

        float tLookAhead = toPursuer.Lengthf() / evading.MaximumSpeed + pursuing.MaximumSpeed;

        return evading.Flee(b + pursuing.Velocity * tLookAhead);
    }

    public static Vector3 Wander (this SteeringAgent wandering, float wanderRadius = 0.01f, float wanderDistance = 0.01f, float wanderJitter = 0.01f){

		Vector3 wanderer = wandering.Position;

		float jitterTimeslice = wanderJitter * wandering.TElapsed;
		wandering.WanderTarget += new Vector3(ExtensionsFloat.RandClamped() * jitterTimeslice, ExtensionsFloat.RandClamped() * jitterTimeslice, wanderer.z);
		Vector3 wanderTarget = (wandering.WanderTarget.normalized * wanderRadius) + new Vector3(wanderDistance, 0, wanderer.z);

		return wandering.AgentTransform.TransformPoint(wanderTarget) - wandering.Position;
	}

	public static Vector3 AvoidObstacle (this SteeringAgent avoiding, List<Obstacle> obstacles, float minDetectionBoxLen = 0.1f ) {
        float detectionBoxLen = minDetectionBoxLen + ((avoiding.Speed / avoiding.MaximumSpeed) * minDetectionBoxLen);
        float distClosest = Mathf.Infinity;
        Obstacle closest = null;
        Vector3 closestPos = Vector3.zero;
        int i = 0;

		// todo: need a function to tag all closest objects -- static function for global calls

		while (i < obstacles.Count) {
            Obstacle curr = obstacles[i];
            
            Vector3 localPos = avoiding.AgentTransform.InverseTransformPoint(curr.gameObject.transform.position);
			if (localPos.x >= 0) { // ignore objects behind the agent
                float expandedRadius = curr.BRadius + avoiding.BoundingRadius;

				if (Mathf.Abs(localPos.y) < expandedRadius) { // line/circle test
                    float cx = localPos.x;
                    float cy = localPos.y;

					/* x = cx + -sqrt(r^2 - cy^2) for y = 0 */
                    float sqrPart = Mathf.Sqrt(expandedRadius * expandedRadius - cy * cy);
                    float intersectionPoint = cx - sqrPart;

					if (intersectionPoint <= 0) {
                        intersectionPoint = cx + sqrPart;
                    }

					if (intersectionPoint < distClosest) {
                        distClosest = intersectionPoint;
                        closest = curr;
                        closestPos = closest.gameObject.transform.position;
                    }
                }
            }
            i++;
        }

        Vector3 force = Vector3.zero;

		if (closest) {
            float multiplier = 1.0f + (detectionBoxLen - closestPos.x) / detectionBoxLen;
			force.y = (closest.BRadius - closestPos.y) * multiplier;
            float brakingWeight = 0.2f;
            force.x = (closest.BRadius - closestPos.x) * brakingWeight;
        }

        return avoiding.AgentTransform.TransformPoint(force);
    }

	public static Vector3 Interpose (this SteeringAgent interposing, SteeringAgent a, SteeringAgent b) {
        Vector3 midpoint = a.Position + b.Position / 2.0f;
        float timeToIntercept = (midpoint - interposing.Position).magnitude / interposing.MaximumSpeed;

        Vector3 aPos = a.Position + a.Velocity * timeToIntercept;
        Vector3 bPos = b.Position + b.Velocity * timeToIntercept;

        midpoint = (aPos + bPos) / 2.0f;

        return interposing.Arrive(midpoint, DecelerationRate.fast);
    }


	public static Vector3 GetPositionOffset (this SteeringAgent agent, Vector3 target, Vector3 point, float pointRadius = 1.0f) {
        float distanceFromBoundry = 3.0f;
        float distAway = pointRadius + distanceFromBoundry;
        Vector3 toTarget = (point - target).normalized;
        return (toTarget * distAway) + point;
    }

	public static Vector3 Hide (this SteeringAgent hiding, SteeringAgent target, List<Obstacle> obstacles) {
        float distToClosest = Mathf.Infinity;
        Vector3 bestPos = Vector3.zero;
        int i = 0;

		while (i < obstacles.Count) {
            Obstacle curr = obstacles[i];
            Vector3 spot = hiding.GetPositionOffset(target.Position, curr.gameObject.transform.position);
            float dSqr = (hiding.Position - spot).sqrMagnitude;

			if (dSqr < distToClosest) {
                distToClosest = dSqr;
                bestPos = spot;
            }
            i++;
        }

		if (distToClosest == Mathf.Infinity){
            return hiding.Evade(target);
        }

        return hiding.Arrive(bestPos, DecelerationRate.fast);
    }

	public static Vector3 OffsetPursuit (this SteeringAgent member, SteeringAgent leader, Vector3 offset) {
        Vector3 worldOffset = leader.AgentTransform.TransformPoint(offset);
        Vector3 toOffset = worldOffset - member.Position;
        float tLookAhead = toOffset.magnitude / (member.MaximumSpeed + leader.Speed);
        return leader.Arrive(worldOffset + leader.Velocity * tLookAhead, DecelerationRate.fast);
    }

	public static void EnforceNonPenatrationConstraint (this SteeringAgent agent, Vector3 pos) {
        Vector3 toPos = agent.Position - pos;
        float d = toPos.magnitude;
		//float overlap = //rad
    }

}
