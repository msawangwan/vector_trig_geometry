using UnityEngine;

public class SteeringAgent : MonoBehaviour {

    [System.Serializable]
    public class Data {

        internal Vector3 Velocity;
        internal Vector3 Heading;

		[RangeAttribute(1,1000)]
        public float Mass;
		[RangeAttribute(0.1f,1f)]
        public float MaximumSpeed;
		[RangeAttribute(1f,100f)]
        public float MaximumForce;
		[RangeAttribute(1f,500f)]
        public float MaximumTurnRate;

        public bool IsWanderingEnabled;
    }

	public Transform AgentTransform { get { return gameObject.transform; } }

    public Vector3 Velocity { 
		get { return agentData.Velocity; } 
		protected set { agentData.Velocity = value; } 
	}

	public Vector3 Heading { 
		get { return agentData.Heading; } 
		protected set { agentData.Heading = value; } 
	}

	public Vector3 Position { get { return transform.position; } }
	public Vector3 WanderTarget { get; set; } // todo: initialised by: VectorExtension.PointRandomOnUnitCircle();

    public float Mass { get { return agentData.Mass; } }
	public float MaximumSpeed { get { return agentData.MaximumSpeed; } }
	public float MaximumForce { get { return agentData.MaximumForce; } }
	public float MaximumTurnRate { get { return agentData.MaximumTurnRate; } }

	public float Speed { get { return Velocity.magnitude; } }
	public float SpeedSqr { get { return Velocity.sqrMagnitude; } }

	public float BoundingRadius { get { return 1.0f; } } // should read collider bounds

	public float TElapsed { get { return Time.deltaTime; } }

	[SerializeField]
    private Data agentData;

	void Awake () {
		if (agentData.IsWanderingEnabled) {
			WanderTarget = ExtensionsVector.PointRandomOnUnitCircle();
		}
	}
}
