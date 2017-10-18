﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour {

    public Light worldLight;
    public Light ceilingLight;

    public float wlintensity = 0.1f;
    public float clintensity = 0.75f;
    public float lerpPercent = 0.2f;
    public float nightLength = 20f;

    public string status;

    private float wlintensityday;
    private float clintensityday;
    private bool activate;
    private float startTime;
    private int index;
    private bool nextscript;

    private MonoBehaviour[] scenarios;

	// Use this for initialization
	void Start ()
    {
        wlintensityday = worldLight.intensity;
        clintensityday = ceilingLight.intensity;
        status = "Day";
        activate = false;
        index = 0;
        nextscript = true;
        Initialize();
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (activate)
        {
            worldLight.intensity = Mathf.Lerp(worldLight.intensity, wlintensity, lerpPercent);
            ceilingLight.intensity = Mathf.Lerp(ceilingLight.intensity, clintensity, lerpPercent);
            if ((startTime + nightLength) <= Time.time)
            {
                Deactivate(); //End Night
            }
        }
        else
        {
            worldLight.intensity = Mathf.Lerp(worldLight.intensity, wlintensityday, lerpPercent);
            ceilingLight.intensity = Mathf.Lerp(ceilingLight.intensity, clintensityday, lerpPercent);
            if (nextscript)
            {
                Debug.Log("Scenario setactive called");
                scenarios[index].enabled = true;
                nextscript = false;
            }
        }
	}

    void Initialize()
    {
        scenarios = transform.Find("Scenarios").GetComponents<MonoBehaviour>();
        Debug.Log("Initialization done");
        Debug.Log("Scenarios len: " + scenarios.Length);
    }

    public void Activate()
    {
        activate = true;
        startTime = Time.time;
        status = "Night";
        GameObject.FindGameObjectWithTag("Academy").GetComponent<TemplateAcademy>().AcademyReset(); //Reset the environment
    }
    public void Deactivate()
    {
        activate = false;
        status = "Day";
    }

    public void MovetoNextScenario(object scripttype, bool status)
    {
        scenarios[index].enabled = false;
        index++;
        if(index >= scenarios.Length)
        {
            Activate();
            index = 0;
        }
        nextscript = true;
    }
}
