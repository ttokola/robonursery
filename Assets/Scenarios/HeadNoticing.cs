using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadNoticing : MonoBehaviour, IScenario {

    public GameObject lilrobotHead;

    private Vector3 orgPosition;
    private GameObject ball;
    private Ray lilbotRay;
    private RaycastHit hit;
    private bool done;
    private bool killcall = false;

	
	// Update is called once per frame
	void Update ()
    {
        orgPosition = lilrobotHead.transform.position;
        lilbotRay = new Ray(orgPosition, lilrobotHead.transform.forward);

        Debug.DrawRay(orgPosition, lilrobotHead.transform.forward * 10);

		if(Physics.Raycast(lilbotRay, out hit, 10f))
        {
            Debug.Log("Raycast hit "+hit.collider.name);
            if(hit.collider.tag == "TargetBall")
            {
                if (!done)
                {
                    lilrobotHead.GetComponentInParent<TemplateAgent>().AddReward(100);
                    Debug.Log("Reward earned!");
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
        ball.transform.position = new Vector3(4.14f, 1.38f, 1.78f);
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
