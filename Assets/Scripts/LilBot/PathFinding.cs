using UnityEngine;
using System.Collections;

public class PathFinding : MonoBehaviour {

    private NavMeshPath path;
    private int waypointIndex = 0;
    
    public GameObject waypoint;
    public NavMeshAgent agent;
    public Transform parent;
    
    public Vector3 destination;
    public Vector3 nextWaypoint;
    public bool ready;
    
	void Start ()
    {
        // We want to move with driving controls instead of moving with the navmesh
        agent.updatePosition = false;
        agent.updateRotation = false;
        ready = false;
        path = new NavMeshPath();
	}
    
    public void SetDestination(Vector3 destination)
    {
        agent.CalculatePath(destination, path);
        Debug.Log("set new dest");
        waypointIndex = 0;
    }
    
    public Vector3 NextWaypoint()
    {
        // First, this gets the second waypoint. First waypoint is the position
        // of the agent in the beginning of the path calculation, we don't
        // care about that
        waypointIndex = Mathf.Min(path.corners.Length+1, waypointIndex+1);
        Debug.Log(path.corners[waypointIndex]);
        waypoint.transform.position = path.corners[waypointIndex];
        return path.corners[waypointIndex];
    }
        
    
    void FixedUpdate ()
    {
        // Always move agent with the parent transform
        agent.Warp(parent.transform.position);
    }
}
