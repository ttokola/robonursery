using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SUTQueue : MonoBehaviour {

	public bool debug = false;
    public NavMeshAgent agent;
    public SUTFunctions controls;	
	public Vector3[] locations;
    public GameObject waypoint;
    
	private Queue<Vector3> queue;
	private Vector3 next;
    private bool hasDestination = false;
    private int waypointNumber;
	
	void Start ()
	{
		queue = new Queue<Vector3>(locations);
        agent.updatePosition = false;
        agent.updateRotation = false;
	}
	
	void FixedUpdate ()
	{
		if (queue.Count == 0)
		{
			return;
		}
        if (!hasDestination)
        {
            agent.destination = queue.Peek();
            hasDestination = true;
            waypointNumber = 1;
        }
        if (agent.pathPending)
        {
            return;
        }
		next = agent.path.corners[waypointNumber];
        waypoint.transform.position = next;
		if (controls.DriveTo(next) == 1)
		{
            waypointNumber++;
            if (agent.path.corners.Length - waypointNumber == 0)
            {
                if (debug) { Debug.Log(string.Format("Arrived at {0}", queue.Peek())); }
                queue.Dequeue();
                hasDestination = false;                
            }
		}
	}
}
