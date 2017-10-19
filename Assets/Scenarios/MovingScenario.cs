using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingScenario : MonoBehaviour, IScenario {

    public GameObject lilrobotBody;

    private GameObject ball;
    private bool done;
    private bool killcall = false;

	
	// Update is called once per frame
	void Update ()
    {
        if(ball != null)
        {
            if (Vector3.Distance(lilrobotBody.transform.position, ball.transform.position) < 2)
            {
                if (!done)
                {
                    lilrobotBody.GetComponentInParent<TemplateAgent>().AddReward(100);
                    done = true;
                }
                else
                {
                    if (!killcall)
                    {
                        StartCoroutine(EndWait());
                        killcall = true;
                    }
                    
                }
            }
        }
	}

    void InstantiateBall()
    {
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = new Vector3(4.14f, 0.5f, 1.78f);
        ball.tag = "TargetBall";
    }

    IEnumerator StartWait()
    {
        yield return new WaitForSeconds(5);
        InstantiateBall();
    }

    IEnumerator EndWait()
    {
        yield return new WaitForSeconds(5);

        Destroy(ball);

        //Add functionality for moving to next scenarion by DayController
        GameObject.FindGameObjectWithTag("GameController").GetComponent<DayController>().MovetoNextScenario(this.GetType(), true);
    }

    public void EnableScenario(bool enabled)
    {
        this.enabled = enabled;
    }

    public void ResetScenario()
    {
        killcall = false;
        Debug.Log(this.GetType() + " started");
        done = false;
        StartCoroutine(StartWait());
    }
}
