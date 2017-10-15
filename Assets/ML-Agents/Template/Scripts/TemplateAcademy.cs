using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateAcademy : Academy {

    public GameObject lilbot;
    public GameObject body;
    public GameObject axel;
    public GameObject neck;
    public GameObject head;
    public GameObject rwheel;
    public GameObject lwheel;
    public GameObject ball;

    public override void AcademyReset()
	{
        lilbot.SetActive(false);
        //Stop forces and movement
        body.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        body.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        axel.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        axel.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        neck.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        neck.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        head.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        head.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        rwheel.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        rwheel.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        lwheel.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        lwheel.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);

        //Teleport lilbot
        lilbot.transform.position = new Vector3(-1.75f, -0.1f, -1.75f);
        lilbot.transform.rotation = Quaternion.identity;
        body.transform.localPosition = new Vector3(0, 0.75f, 0);
        body.transform.localRotation = Quaternion.identity;
        axel.transform.localPosition = new Vector3(0, 0.4f, 0);
        axel.transform.localRotation = Quaternion.identity;
        neck.transform.localPosition = new Vector3(0, 1.238994f, 0);
        neck.transform.localRotation = Quaternion.identity;
        head.transform.localPosition = new Vector3(0, 1.45f, 0);
        head.transform.localRotation = Quaternion.identity;
        rwheel.transform.localPosition = new Vector3(0.40015f, -6.3e-14f, 2.5e-14f);
        rwheel.transform.localRotation = Quaternion.identity;
        lwheel.transform.localPosition = new Vector3(-0.4001489f, -6.3e-14f, 2.5e-14f);
        lwheel.transform.localRotation = Quaternion.identity;

        lilbot.SetActive(true);

        //Teleport ball
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        ball.transform.position = new Vector3(-29.83f, 0.5f, -6.84f);

    }

	public override void AcademyStep()
	{


	}

}
