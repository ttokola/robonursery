/*
    Attempt to score a goal.
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;
using UnityEngine.AI;

namespace LilBotNamespace
{

public class PlayFootball : MonoBehaviour {
    
    private int state;
    
    public Transform pos;
    private GameObject ball;
    private NavMeshObstacle obstacle;
    private Vector3 goal;
    private Vector3 destination;
    private MovementControls movementControls;
    private ArmControls armControls;

    void Start ()
    {
        state = 0;
        ball = GameObject.Find("Football");
        goal = GameObject.Find("Goal_001").transform.position;
        movementControls = gameObject.GetComponent<MovementControls>();
        armControls = gameObject.GetComponent<ArmControls>();
        destination = new Vector3(-23, 0, -4);
        obstacle = ball.GetComponent<NavMeshObstacle>();
    }
	
	void FixedUpdate ()
    {
        switch (state)
        {
        // Move to starting position    
        case 0:
            if (movementControls.DriveTo(destination, true) == 0)
            {
                // Destination calculated so that the ball is between the goal and the destination
                destination = ball.transform.position + 2 * (ball.transform.position - goal).normalized;
                state = 1;
            }
            break;
        // Move to kicking position
        case 1:
            if (movementControls.DriveTo(destination, true) == 0)
            {
                obstacle.enabled = false;
                state = 2;
                // Bot moves towards the goal, hopefully hitting the ball in the right direction
                destination = ball.transform.position - 1 * (ball.transform.position - goal).normalized;
            }
            break;
        // Wait for ball to be available
        // Agent seems to react to NavMesh changes with a delay, this seems to fix it
        case 2:
            if (!obstacle.enabled)
            {
                state = 3;
            }
            break;  
        // Bump the ball towards the goal
        case 3:
            // Calculates the difference in direction towards the ball and the goal
            float offCourse = ((goal - pos.position).normalized - (ball.transform.position - pos.position).normalized).magnitude;                        
            // Reposition behind the ball if too far off course
            if (offCourse > 0.3f)
            {
                destination = ball.transform.position + 1 * (ball.transform.position - goal).normalized;
                state = 1;
            }   
            movementControls.DriveTo(ball.transform.position - 2 * (ball.transform.position - goal).normalized, true);
            break;                 
        case 4:
            // Celebrate
            armControls.SetStaticPosition("up");
            break;  
        }
        
        // Check if the ball has scored
        if ((ball.transform.position - goal).magnitude < 1.8f)
        {
            state = 4;
        }
    }
}

} // End namespace