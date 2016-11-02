using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class BotQueue : MonoBehaviour {

	public bool debug = false;
    public Transform body;
    public MovementControls controls;	
    public string[] objectives;
    
    private Reflexes reflexes;
    private int currentObjectiveIndex;
	
	void Start ()
	{
        reflexes = GetComponent<Reflexes> ();
        currentObjectiveIndex = 0;
	}
    /*
    int Execute(string objective)
    {
        string[] obj = objective.Split(new char[]{" "}));
        if (obj[0].ToLower == "goto")
        {
            string[] coord = obj[1].Split(",");
            return controls.DriveTo(new Vector3(float.Parse(coord[0]), 0, float.Parse(coord[1])));
        }
        return -1;
    }
            
	
	void FixedUpdate ()
	{
        if (currentObjectiveIndex >= objectives.length)
        {
            if (loop)
            {
                currentObjectiveIndex = 0;
            }
            else
            {
                return;
            }
        }
        
        if (Execute(objectives[currentObjectiveIndex]) == 0)
        {
            currentObjectiveIndex += 1;
        }
	}*/
}

} // End namespace