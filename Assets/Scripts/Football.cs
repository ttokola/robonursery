using UnityEngine;
using System.Collections;

public class Football : MonoBehaviour {

    private GameObject ball;
    private Vector3 goal1;
    private Vector3 goal2;
    private Vector3 dest;
    
    void Start ()
    {
        ball = GameObject.Find("ball");
        goal1 = GameObject.Find("goal1").transform.position;
        goal2 = GameObject.Find("goal2").transform.position;
    }
    
	public Vector3 getDestination (bool team, int role, Vector3 pos)
    {
        Vector3 objective = getObjective(team, role);
        if (role == 1)
        {
            dest = ball.transform.position + (ball.transform.position - objective).normalized;
            if (Vector3.Distance(pos, dest) < 1)
            {
                dest = objective + (ball.transform.position - objective).normalized;
                
            } 
        }
        else if (role == 0)
        {
            dest = objective + (ball.transform.position - objective).normalized; 
        }
        return dest;
	}
    
    Vector3 getObjective(bool team, int role)
    {
        if ((team && role == 1) || (!team && role == 0))
            return goal1;
        else 
            return goal2;
    }

}