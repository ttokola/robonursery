using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Using this kind of One SUPER game controller isn't the best solution
 * in the lonf run but during this early development good enough.
 * 
 * Day is controlled by the amount of scenarios and each day runs all of them. 
 * No time based days. 
 * 
 * Scenarios are picked to run randomly. 
 * 
 */
public class DayController : MonoBehaviour {

    public Light worldLight;
    public Light ceilingLight;

    public float wlintensity = 0.1f;
    public float clintensity = 0.75f;
    public float lerpPercent = 0.2f;
    public float nightLength = 20f;

    public string status;
    private int maxRunPoints = 0;
    private int maxDayPoints = 0;
    private List<string> skills = new List<string>(); //Define better value type in the future


    private float wlintensityday;
    private float clintensityday;
    private bool activate;
    private float startTime;
    private int index;
    private bool nextscript;
    private System.Random rd;
    private List<int> completed = new List<int>();

    private IScenario[] scenarios;

	// Use this for initialization
	void Start ()
    {
 
        wlintensityday = worldLight.intensity;
        clintensityday = ceilingLight.intensity;
        status = "Day";
        activate = false;
        index = 0;
        nextscript = true;
        rd = new System.Random();
        
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
                scenarios[index].EnableScenario(true);
                scenarios[index].ResetScenario();
                nextscript = false;
            }
        }
	}

    void Initialize()
    {
        scenarios = transform.Find("Scenarios").GetComponents<IScenario>();
        Debug.Log("Initialization done");
        Debug.Log("Scenarios len: " + scenarios.Length);
    }

    public void Activate()
    {
        Debug.Log("Activate called");
        activate = true;
        startTime = Time.time;
        status = "Night";
        maxRunPoints += maxDayPoints;
        maxDayPoints = 0;
        //TODO: Disable lilrobot moving and AI steps!!
        GameObject.FindGameObjectWithTag("Academy").GetComponent<TemplateAcademy>().AcademyReset(); //Reset the environment | maybe create some dayreset and not the whole ML-Agents reset
    }
    public void Deactivate()
    {
        activate = false;
        status = "Day";
        nextscript = true;
        index = 0;
    }

    public void MovetoNextScenario(object scripttype, bool status, string[] learnedskills, int points)
    {
        //Change index to scenario ID in the future
        completed.Add(index);

        //Add obtained skill and points
        if (status)
        {
            if(learnedskills.Length > 0)
            {
                Debug.Log("Learnedskills: ");
                foreach(string t in learnedskills) { Debug.Log(t); }
                foreach (string skill in learnedskills)
                {
                    skills.Add(skill);
                }
            }
            maxDayPoints += points;
            
        }

        scenarios[index].EnableScenario(false);
    SELECT_SCENARIO:
        int next = rd.Next(0, scenarios.Length);
        index = next;
        //Check if already done for this day
        if (completed.Contains(index))
        {
            if (completed.Count == scenarios.Length)
            {
                Activate();
                index = 0;
            }
            else
                goto SELECT_SCENARIO;
        }
        Debug.Log(next);
        Debug.Log(index);
        if(scenarios[index].GetRequirements().Length >0)
        {
            foreach (string skill in scenarios[index].GetRequirements())
            {
                if (!skills.Contains(skill))
                {
                    //Doesn't have required skill yet, go back
                    goto SELECT_SCENARIO;
                }
            }
        }
        Debug.Log(index);
        nextscript = true;
    }

    public int GetMaxDayPoints()
    {
        return maxDayPoints;
    }
    public int GetMaxRunPoints()
    {
        return maxRunPoints;
    }
}
