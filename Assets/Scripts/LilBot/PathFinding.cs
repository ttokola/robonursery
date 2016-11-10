using UnityEngine;
using System.Collections;

public class PathFinding : MonoBehaviour {

    public GameObject waypoint;
    public NavMeshAgent agent;
    
    public Vector3 destination;
    public Vector3 nextWaypoint;
    public bool ready;
    
	void Start ()
    {
        // We want to move with driving controls instead of moving with the navmesh
        agent.updatePosition = false;
        agent.updateRotation = false;
        ready = false;
	}
    
    void FixedUpdate ()
    {
        agent.destination = destination;
        if (! agent.pathPending)
        {
            ready = true;
            nextWaypoint = agent.path.corners[0];
            waypoint.transform.position = nextWaypoint;
        }
        else
        {
            ready = false;
        }
    }
}
