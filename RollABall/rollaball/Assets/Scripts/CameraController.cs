using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject robot;
	private Vector3 offset;
	// Use this for initialization
	void Start () {
		offset = transform.position - robot.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = robot.transform.position + offset;
	}
}
