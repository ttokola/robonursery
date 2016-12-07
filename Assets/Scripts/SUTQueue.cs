using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SUTQueue : MonoBehaviour {

	public bool debug = false;
    public NavMeshAgent agent;
    public Transform body;
    public SUTFunctions controls;	
	public Vector3[] locations;
    public GameObject waypoint;
    public bool hasDestination = false;
    
    private SUTReflexes reflexes;
    private bool hasPath = false;
	private Queue<Vector3> queue;
	private Vector3 next;
    private int waypointNumber;
	
	void Start ()
	{
        reflexes = GetComponent<SUTReflexes> ();
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
        
        if (!hasPath || !hasDestination)
        {
            if (!hasDestination)
            {
                //agent.Warp(body.position);
                hasDestination = true;
                waypointNumber = 1;            
                if (reflexes.loading)
                {
                    agent.destination = reflexes.destination;
                }
                else
                {
                    agent.destination = queue.Peek();
                }   
            }
            if (agent.pathPending)
            // Need to wait for path calculation
            {
                hasPath = false;
                return;
            }
            hasPath = true;
            next = agent.path.corners[waypointNumber];
            waypoint.transform.position = next;            
        }
        
		if (controls.DriveTo(next) == 1)
		{
            if (agent.path.corners.Length - waypointNumber == 1)
            // Current waypoint is the last one
            {
                if (reflexes.loading)
                {
                    return;
                }
                else
                {
                    if (debug) { Debug.Log(string.Format("Arrived at {0}", queue.Peek())); }
                    queue.Dequeue();                    
                }
                hasDestination = false;
                return;
            }
            waypointNumber++;
            next = agent.path.corners[waypointNumber];
            waypoint.transform.position = next;      
		}
	}
}
