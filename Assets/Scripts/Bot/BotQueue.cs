using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LilBotNamespace;

namespace LilBotNamespace
{

public class BotQueue : MonoBehaviour {

	public bool debug = false;
    public MovementControls controls;	
    public PathFinding pathFinder;
    public bool usePathfinding;
	public Vector3[] locations;
    
    private Reflexes reflexes;
    private bool hasPath = false;
	private Queue<Vector3> queue;
	
	void Start ()
	{
        reflexes = GetComponent<Reflexes> ();
		queue = new Queue<Vector3>(locations);
	}
	
	void FixedUpdate ()
	{
		if (queue.Count == 0 || reflexes.loading)
		{
			return;
		}
        if (controls.DriveTo(queue.Peek(), usePathfinding) == 1)
        {
            queue.Dequeue();
        }
	}
}
}
