using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SUTQueue : MonoBehaviour {

	public bool debug = false;
	
	public Vector3[] locations;
	private Queue<Vector3> queue;
	private Vector3 next;
	
	public SUTFunctions controls;

	void Start ()
	{
		queue = new Queue<Vector3>(locations);
	}
	
	void FixedUpdate ()
	{
		if (queue.Count == 0)
		{
			return;
		}
		next = queue.Peek();
		if (controls.DriveTo(next) == 0)
		{
			if (debug) { Debug.Log(string.Format("Arrived at {0}", next)); }
			queue.Dequeue();
		}
	}
}
