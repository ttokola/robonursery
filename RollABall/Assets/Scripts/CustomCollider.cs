using UnityEngine;
using System.Collections;

public class CustomCollider : MonoBehaviour {

    public bool showRay = true;
    public float lengthOfRay = 1.0f;
    // private
    Vector3 origin;

    [HideInInspector]
    public bool upHit = false;
    [HideInInspector]
    public bool downHit = false;
    [HideInInspector]
    public bool rightHit = false;
    [HideInInspector]
    public bool leftHit = false;
    [HideInInspector]
    public bool forwardHit = false;
    [HideInInspector]
    public bool backHit = false;

    // Use this for initialization
    void Start () {
        //lengthOfRay = GetComponent<Collider>().bounds.extents.y;
        //directionFactor = Mathf.Sign(Vector3.down.y);

    }

    // Update is called once per frame
    void Update()
    {
        origin = transform.localPosition;

        Vector3 up = transform.up;
        Vector3 down = -up;
        Vector3 right = transform.right;
        Vector3 left = -right;
        Vector3 forward = transform.forward;
        Vector3 back = -forward;

        if (showRay)
        {
            // Draw ray on screen to see visually.
            Debug.DrawRay(origin, up * lengthOfRay, Color.yellow);
            Debug.DrawRay(origin, back * lengthOfRay, Color.yellow);
            Debug.DrawRay(origin, right * lengthOfRay, Color.yellow);
            Debug.DrawRay(origin, left * lengthOfRay, Color.yellow);
            Debug.DrawRay(origin, forward * lengthOfRay, Color.yellow);
            Debug.DrawRay(origin, back * lengthOfRay, Color.yellow);
        }

        if (Physics.Raycast(origin, up, lengthOfRay))
        {
            upHit = true;
            //Debug.Log("Up Hit");
        }
        else
        {
            upHit = false;
        }


        if (Physics.Raycast(origin, down, lengthOfRay))
        {
            downHit = true;
            //Debug.Log("Down Hit");
        }
        else
        {
            downHit = false;
        }

        if (Physics.Raycast(origin, right, lengthOfRay))
        {
            rightHit = true;
            //Debug.Log("Right Hit");
        }
        else
        {
            rightHit = false;
        }


        if (Physics.Raycast(origin, left, lengthOfRay))
        {
            leftHit = true;
            //Debug.Log("Left Hit");
        }
        else
        {
            leftHit = false;
        }

        if (Physics.Raycast(origin, forward, lengthOfRay))
        {
            forwardHit = true;
            //Debug.Log("Forward Hit");
        }
        else
        {
            forwardHit = false;
        }

        if (Physics.Raycast(origin, back, lengthOfRay))
        {
            backHit = true;
            //Debug.Log("Back Hit");
        }
        else
        {
            backHit = false;
        }

    }


      
}
