/*
    Pathfinding using the Unity internal pathfinding engine
*/

using UnityEngine;
using System.Collections;

public class PathFinding : MonoBehaviour {

    [Tooltip("Enable display of debug mesh to view the next waypoint")]
    public bool showWaypoint = false;
    
    [Tooltip("Drag the NavMesh agent attached to the robot here")]
    public NavMeshAgent agent;

    private int waypointIndex = 0;
    
    private GameObject waypointDummy;
    private NavMeshPath path;

	void Start ()
    {
        // Create a dummy waypoint object for debugging
        waypointDummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        waypointDummy.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        Destroy (waypointDummy.GetComponent<Collider> ());
        waypointDummy.SetActive(showWaypoint);
        // We want to move with driving controls instead of moving with the NavMeshAgent
        agent.updatePosition = false;
        agent.updateRotation = false;
        path = new NavMeshPath();
	}
    
    void OnValidate ()
    // Change waypoint visibility if it's changed from inspector
    {
        if (waypointDummy != null)
        {
            waypointDummy.SetActive(showWaypoint);
        }
    }
    
    public void SetDestination(Vector3 destination)
    /*
        Calculate the path for the agent
        Simulation is halted until the path calculation is complete,
        so this might become problematic with many bots in the future
    */
    {
        agent.CalculatePath(destination, path);
        waypointIndex = 0;
    }
    
    public Vector3 NextWaypoint()
    /*
        Get the next waypoint of the path if possible,
        otherwise return the last waypoint
    */
    {
        // We want to start from index 1 (second waypoint).
        // First waypoint is the position of the agent at the beginning
        // of the path calculation, we don't care about that
        waypointIndex = Mathf.Min(path.corners.Length+1, waypointIndex+1);
        waypointDummy.transform.position = path.corners[waypointIndex];
        //Debug.Log("returning wp");
        return path.corners[waypointIndex];
    }    
    
    void FixedUpdate ()
    {
        // Always move agent with the transform
        // This throws errors if the agent has no transform,
        // which is probably what we want since we are using the agent
        // to just calculate paths for the actual rigidbody
        agent.Warp(agent.transform.position);
    }
}
