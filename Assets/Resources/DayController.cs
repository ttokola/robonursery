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

    public int numOfScenarios = 10;

    public string status;
    public bool capture = false;

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

    private List<string> scenarioslist = new List<string>();
    private IScenario[] dayscenarios;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this.transform.gameObject);
        wlintensityday = worldLight.intensity;
        clintensityday = ceilingLight.intensity;
        status = "Day";
        activate = false;
        index = 0;
        nextscript = true;
        rd = new System.Random();

        //Video Capture
        //RockVR.Video.VideoCaptureCtrl.instance.StartCapture();

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
                dayscenarios[index].EnableScenario(true);
                dayscenarios[index].ResetScenario();
                nextscript = false;
            }
        }
        if (capture)
        {
            RockVR.Video.VideoCaptureCtrl.instance.StopCapture();
            capture = false;
        }
	}

    void Initialize()
    {
        Object[] objects = Resources.LoadAll("Scenarios");
        
        foreach (Object obj in objects)
        {
            Debug.Log(obj.name);
            scenarioslist.Add(obj.name);
        }
        Resources.UnloadUnusedAssets();

        int num = scenarioslist.Count;
        Debug.Log("Number of IScenario files: " + num);
        LoadScenarios();
        Debug.Log("Initialization done");
        Debug.Log("DayScenarios len: " + dayscenarios.Length);
    }

    void LoadScenarios()
    {
        int count = 0;
        bool flag = false;
        for (int i=0; count < numOfScenarios; i++)
        {
            flag = false;
            try
            {
                var list = scenarioslist.ToArray();
                Debug.Log(System.Type.GetType(list[i]));
                if (skills.Contains(list[i])) continue;
                transform.Find("Scenarios").gameObject.AddComponent(System.Type.GetType(list[i]));
                var reqs = ((IScenario)transform.Find("Scenarios").GetComponent(System.Type.GetType(list[i]))).GetRequirements();
                if (reqs.Length < 1) { count++; continue; }
                foreach (string req in reqs) { if (!skills.Contains(req)) { Destroy(transform.Find("Scenarios").GetComponent(System.Type.GetType(list[i]))); flag = true; break;  } }
                if (flag) continue;
                count++;
                //In the future check if Unity scene needs to be changed
                //change with ChangeUnityScene(number);
            }
            catch (System.IndexOutOfRangeException e)
            {
                //Number of implemented scenarios is smaller than number of required scenarios for one day
                break;
            }
        }
        dayscenarios = transform.Find("Scenarios").GetComponents<IScenario>();
    }

    //Remove previous days scenarios
    void RemoveScenarios()
    {
        var sces = transform.Find("Scenarios").GetComponents<IScenario>();
        foreach(Object sce in sces) { Destroy(sce); }
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
        RemoveScenarios();
        LoadScenarios();
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

        dayscenarios[index].EnableScenario(false);
    SELECT_SCENARIO:
        int next = rd.Next(0, dayscenarios.Length);
        index = next;
        //Check if already done for this day
        if (completed.Contains(index))
        {
            if (completed.Count == dayscenarios.Length)
            {
                Activate();
                index = 0;
            }
            else
                goto SELECT_SCENARIO;
        }
        Debug.Log(next);
        Debug.Log(index);
        if(dayscenarios[index].GetRequirements().Length >0)
        {
            foreach (string skill in dayscenarios[index].GetRequirements())
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

    //For future if environment is needed to change for scenario
    //(this) GameMananger doesn't destroy on change 
    void ChangeUnityScene(int number)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(number);
    }

    //Use these as a wrapper to give points to ML-Agents
    public int GetMaxDayPoints()
    {
        return maxDayPoints;
    }
    public int GetMaxRunPoints()
    {
        return maxRunPoints;
    }
}
