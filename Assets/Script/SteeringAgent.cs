using UnityEngine;

public class SteeringAgent : MonoBehaviour {

    [System.Serializable]
    public class Data {

        internal Vector3 Velocity;
        internal Vector3 Heading;

        public float Mass;
        public float MaximumSpeed;
        public float MaximumForce;
        public float MaximumTurnRate;
    }

    public Vector3 Velocity { 
		get { return AgentData.Velocity; } 
		protected set { AgentData.Velocity = value; } 
	}

	public Vector3 Heading { 
		get { return AgentData.Heading; } 
		protected set { AgentData.Heading = value; } 
	}

	public Vector3 Position { get { return transform.position; } }

	public float Mass { get { return AgentData.Mass; } }
	public float MaximumSpeed { get { return AgentData.MaximumSpeed; } }
	public float MaximumForce { get { return AgentData.MaximumForce; } }
	public float MaximumTurnRate { get { return AgentData.MaximumTurnRate; } }

	[SerializeField]
    private Data AgentData;
}
