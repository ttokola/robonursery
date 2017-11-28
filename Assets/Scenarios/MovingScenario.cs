using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingScenario : MonoBehaviour, IScenario {

    public GameObject lilrobotBody;

    private GameObject ball;
    private bool done;
    private bool killcall = false;
    private string[] requirements;
    private string[] newskills;

    private float startTime;
    private float endTime;

    private void Awake()
    {
        requirements = new string[] { };
        newskills = new string[] { "Moving" };
    }

    // Update is called once per frame
    void Update ()
    {
        if(Time.time > endTime && !done)
        {
            // Lilrobot didn't reach the objective of the scenario in given time
            // so end it and move to next scenario.
            Debug.Log("Scenario failed, move to next one.");

            Destroy(ball);

            //Add functionality for moving to next scenarion by DayController
            GameObject.FindGameObjectWithTag("GameController").GetComponent<DayController>().MovetoNextScenario(this.GetType(), false, new string[] { }, 0);
            done = true;
            killcall = true;
        }

        if(ball != null)
        {
            if (Vector3.Distance(lilrobotBody.transform.position, ball.transform.position) < 2)
            {
                if (!done)
                {
                    //In the future don't add the reward straight to the ML-Agents but through GameMananger
                    lilrobotBody.GetComponentInParent<TemplateAgent>().AddReward(100); 
                    done = true;
                }
                else
                {
                    if (!killcall)
                    {
                        StartCoroutine(EndWait());
                        Destroy(ball);

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

        //Add functionality for moving to next scenarion by DayController
        GameObject.FindGameObjectWithTag("GameController").GetComponent<DayController>().MovetoNextScenario(this.GetType(), true, newskills, 100);

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
        startTime = Time.time;
        endTime = startTime + 60 * 1;
    }

    public string[] GetRequirements()
    {
        return requirements;
    }
}
