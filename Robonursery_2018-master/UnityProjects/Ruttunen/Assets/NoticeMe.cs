using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeMe : MonoBehaviour {

    public GameObject Robot;
    public SphereCollider Sphere;
    public float ResetTime = 3.0f;

    private float initialHeight;
    private Camera roboCamera;
    private GameObject ball;
    private float resetCounter;

    private Vector3 startPosition;
    private const float Y_MIN = 0;
    private const float Y_MAX = 3;

    private const float X_MIN = -4;
    private const float X_MAX = 4;

	// Use this for initialization
	void Start () {
        if (Robot == null)
        {
            Robot = FindObjectOfType<RoboAgent>().gameObject;
        }
        roboCamera = Robot.GetComponentInChildren<Camera>();
        ball = GameObject.Find("Sphere");
        Sphere = ball.GetComponent<SphereCollider>();
        Sphere = Robot.GetComponentInChildren<SphereCollider>();
        startPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if(Sphere == null)
        {
            Debug.Log("Start search");
            ball = GameObject.Find("Sphere");
            Sphere = ball.GetComponent<SphereCollider>();
        }
        
        var cameraPosition = roboCamera.transform.position;
        var cameraForward = roboCamera.transform.forward;
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit ;
        Physics.Raycast(cameraPosition, cameraForward, out hit, float.MaxValue, layerMask);
        if (hit.collider != null && hit.collider.gameObject.GetComponent<NoticeMe>() != null)
        {
            // Add reward if focused on sphere
            Robot.GetComponent<RoboAgent>().AddReward(0.05f);
            ball.GetComponent<Renderer>().material.color = new Color(255,155,0);
            resetCounter += Time.deltaTime;
        }
        else
        {
            ball.GetComponent<Renderer>().material.color = new Color(0, 25, 255);
            if (Sphere.GetComponent<Renderer>().isVisible)
            {
                // Add smaller penalty if object is visible but not in focus
                Robot.GetComponent<RoboAgent>().AddReward(-0.01f);
            }
            else
            {
                // Add larger penalty if object is not visible
                Robot.GetComponent<RoboAgent>().AddReward(-0.05f);
            }
        }

        if(resetCounter >= ResetTime)
        {
            // Add reward for finishing task
            Robot.GetComponent<RoboAgent>().AddReward(1f);
            // Reset agent and get new position for sphere
            transform.position = new Vector3( startPosition.x + Random.Range(X_MIN, X_MAX), transform.position.y , transform.position.z);
            resetCounter = 0;
            Robot.GetComponent<RoboAgent>().Done();
        }
    }
}
