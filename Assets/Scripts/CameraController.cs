using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public GameObject robot;
    private Vector3 offset;
    // Use this for initialization
    void Start()
    {
        offset = robot.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = robot.transform.position + offset;
    }
    void LateUpdate()
    {
        float desiredAngle = robot.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = robot.transform.position + (rotation * offset);
        transform.LookAt(robot.transform);
    }
}
