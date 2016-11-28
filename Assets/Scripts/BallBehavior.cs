using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {
    
    public int team1score = 0;
    public int team2score = 0;
    private Rigidbody rb; 

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}
    
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "goal1")
        {
            team2score++;
            ResetBall();
        }
        else if (col.gameObject.name == "goal2")
        {
            team1score++;
            ResetBall();
        }
        else if (col.gameObject.name == "outOfField")
        {
            ResetBall();
        }
    }
    
    void ResetBall ()
    {
        transform.position = new Vector3(0f, 4f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
        rb.velocity = new Vector3(0f, 0f, 0f);
    }
}


