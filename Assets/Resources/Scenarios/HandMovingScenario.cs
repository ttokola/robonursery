using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovingScenario : MonoBehaviour, IScenario {

    public GameObject lilrobotbody;
    public GameObject lilrobotlefthand;
    public GameObject lilrobotrighthand;
    public GameObject nanyrobot;

    private bool done;
    private int state;
    private bool start = false;
    private bool reached = false;
    private bool killcall = false;
    private bool callNany = false;
    private string[] requirements;
    private string[] newskills;

    private Vector3 target = new Vector3(0,0,0);

    private float startTime;
    private float endTime = 0.0f;

    private void Awake()
    {
        requirements = new string[] { };
        newskills = new string[] { this.name };
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (start)
        {
            if(target.x == 0) target = lilrobotbody.transform.position + new Vector3(4, 0, 0);
            if (!reached)
            {
                state = nanyrobot.GetComponent<LilBotNamespace.MovementControls>().DriveTo(target, true);
                Debug.Log("State: " + state);
            }
            if (state == 0 || reached)
            {
                if (endTime == 0.0f) endTime = Time.time + 90 * 1;
                if (Time.time > endTime && !done)
                {
                    // Lilrobot didn't reach the objective of the scenario in given time
                    // so end it and move to next scenario.
                    Debug.Log("Scenario failed, move to next one.");
                    nanyrobot.GetComponent<SayHello>().state = 0;
                    nanyrobot.GetComponent<SayHello>().enabled = false;
                    nanyrobot.SetActive(false);
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<DayController>().MovetoNextScenario(this.GetType(), false, new string[] { }, 0);
                    done = true;
                    killcall = true;
                    start = false;
                }
                Debug.Log("Target reached");
                reached = true;
                if (!callNany)
                {
                    nanyrobot.GetComponent<SayHello>().enabled = true;
                    nanyrobot.GetComponent<SayHello>().setTask(lilrobotbody, 1, false);
                    callNany = true;
                }

                Debug.Log(nanyrobot.GetComponent<SayHello>().state);

                //Not the best solution but need to be refactored when proper hand movement is available
                if (lilrobotlefthand.gameObject.transform.position.z > 0 || lilrobotrighthand.gameObject.transform.position.z > 0)
                {
                    if (!done)
                    {
                        //In the future don't add the reward straight to the ML-Agents but through GameMananger
                        lilrobotbody.GetComponentInParent<TemplateAgent>().AddReward(100);
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
    }

    IEnumerator StartWait()
    {
        yield return new WaitForSeconds(1);
        start = true;
        //startTime = Time.time;
        //endTime = startTime + 60 * 3;
        Debug.Log("Indside StartWait");

    }

    IEnumerator EndWait()
    {
        nanyrobot.GetComponent<SayHello>().state = 0;
        nanyrobot.GetComponent<SayHello>().enabled = false;
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
        start = false;
        endTime = 0.0f;
        nanyrobot.SetActive(true);
        StartCoroutine(StartWait());

    }

    public string[] GetRequirements()
    {
        return requirements;
    }
}
