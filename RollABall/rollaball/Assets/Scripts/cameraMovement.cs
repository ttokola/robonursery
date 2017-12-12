using UnityEngine;
using System.Collections;

public class cameraMovement : MonoBehaviour
{

    public bool cToMove = false;
    public float turnSpeed = 3.0f;
    public float panSpeed = 1.0f;
    public float zoomSpeed = 2.0f;

    private Vector3 mouseOrigin;
    private Vector3 initialPos;
    private Quaternion initialRot;
  
    private bool isPanning;
    private bool isRotating;
    private bool isZooming;

    CustomCollider collisionTest;

    void Start()
    {
        collisionTest = GetComponent<CustomCollider>();

        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
    }

    void Update()
    {
        if (cToMove && (!Input.GetKey(KeyCode.C)))
        {
            return;
        }


        if (Input.GetKeyDown( KeyCode.Space ))
        {
            transform.localPosition = initialPos;
            transform.localRotation = initialRot;
        }

        // Left mouse button
        if (!Input.GetMouseButton(0))
        {
            isRotating = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }


        // Right mouse button
        if (!Input.GetMouseButton(1))
        {
            isPanning = false;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }


        // Middle mouse button
        if (!Input.GetMouseButton(2))
        {
            isZooming = false;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }


        // Rotate camera along X and Y axis
        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
            transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
        }

        // Move the camera on it's XY plane
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);

            stopCollisionDirections(ref move);
            transform.Translate(move, Space.Self);
        }

        // Move the camera linearly along Z axis
        if (isZooming)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(0, 0, 1);
            move *= pos.y * zoomSpeed;

            //Debug.Log(Mathf.Sign(move.z));

            stopCollisionDirections(ref move);
            transform.Translate(move, Space.Self);
        }
    }

    void stopCollisionDirections( ref Vector3 move)
    {
        if (collisionTest.upHit && Mathf.Sign(move.y) == 1)
        {
            move.y = 0.0f;
        }

        if (collisionTest.downHit && Mathf.Sign(move.y) == -1)
        {
            move.y = 0.0f;
        }

        if (collisionTest.rightHit && Mathf.Sign(move.x) == 1)
        {
            move.x = 0.0f;
        }

        if (collisionTest.leftHit && Mathf.Sign(move.x) == -1)
        {
            move.x = 0.0f;
        }

        if (collisionTest.forwardHit && Mathf.Sign(move.z) == 1)
        {
            move.z = 0.0f;
        }

        if (collisionTest.backHit && Mathf.Sign(move.z) == -1)
        {
            move.z = 0.0f;
        }
    }
}
