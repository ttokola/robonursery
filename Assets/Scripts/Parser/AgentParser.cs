using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;
using System;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;


public abstract class AgentParser : MonoBehaviour {

    

    [System.Serializable]
    public struct Component
    {
        public bool Link;
        public string PartName;
        public GameObject gameObject;
        
        public bool Movable;
        public enum Type_
        {
            Joint,
            Wheel
        }
        public Type_ Type;
        public float[] DimensionMultipliers;
        public int[] ActionIndeces;
    }

    [System.Serializable]
    public struct AgentParameters
    {
        //public GameObject[] InUse;

    }

    // Use this for initialization
    private MovementControls motor;
    private string motorname1= "Wheel";
    private string motorname2 = "Joint";

    public string ConfigName = "Config"; 
   public int WheelMultiplier=1;
   public int JointMultiplier=1;

    //Parser flags
    public Boolean AddColliders;
    public Boolean Link;
    public Boolean Action_Indicies;
    public Boolean Multipliers; 

    public AgentParameters agentParameters;

    public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
    /**< \brief Keeps track of the agents which subscribe to this proto*/

    [SerializeField]
    public List<Component> components = new List<Component>();


    void Start()
    {
        /*
        Debug.Log("Start run");
        foreach (Component component in components)
        {
            if (component.gameObject.GetComponent<Rigidbody>() == null && component.Movable)
            {
                component.gameObject.AddComponent<Rigidbody>();
                component.gameObject.AddComponent<BoxCollider>();
                if (component.Type == Component.Type_.Joint)
                {
                    component.gameObject.AddComponent<ConfigurableJoint>();
                }
            }
        }
        */
    }

    public List<Component> GetComponents()
    {
        return components;
    }
    
    public Component GetComponentByName(string name)
    {
        Component result = new Component();
        foreach(Component component in components)
        {
            if (component.PartName == name)
            {
                return component;
            }
        }
        Debug.LogError("No component exists by given name");
        return result;
    }

    public Dictionary<int, List<Component>> CollectComponents()
    {
        Dictionary<int, List<Component>> result = new Dictionary<int, List<Component>>();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                var component = new Component();
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.Movable = false;
                components.Add(component);
            }
        }
        return result;
    }
	void OnEnable()
    {
        //id = gameObject.GetInstanceID();

    }
    //This is called by the Academy at the start of the environment
    [ContextMenu("Initialize AgentProto")]
    void InitializeParser()
    {
        //
        

        components.Clear();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                var component = new Component();
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.Movable = false;

                component.ActionIndeces = new int[3];
                component.DimensionMultipliers = new float[3];
                //if objects name contains word "Wheel", set default type to wheel
                if (component.PartName.Contains(motorname1))
                {
                    component.Type = Component.Type_.Wheel; 
                }
                components.Add(component);
            }
        }
    }

    [ContextMenu("Auto add action indicies and action multiplier to movable objects")]
    void AddIndicies()
    {
        motor = this.gameObject.GetComponent<MovementControls>();
        int number = 0;
        foreach (Component component in GetComponents())
        {
            if (component.Movable == true)
            {
                
                CheckRigidbody(component.gameObject);

                switch (component.Type)
                {

                    case Component.Type_.Wheel:
                        {
                            if (Multipliers == true)
                            {
                                component.DimensionMultipliers[0] = WheelMultiplier;
                            }
                            if (Action_Indicies == true)
                            { 
                                component.ActionIndeces[0] = number;
                                number = number + 1;
                            }
                            break;
                        }

                    case Component.Type_.Joint:
                        {
                            if (Multipliers == true)
                            {
                                component.DimensionMultipliers[0] = JointMultiplier;
                                component.DimensionMultipliers[1] = JointMultiplier;
                                component.DimensionMultipliers[2] = JointMultiplier;
                            }
                    if (Action_Indicies == true)
                    {
                        component.ActionIndeces[0] = number;
                        number = number + 1;
                        component.ActionIndeces[1] = number;
                        number = number + 1;
                        component.ActionIndeces[2] = number;
                        number = number + 1;
                    }
                            break;
                            }

                }

             }
            if (component.Link == true && Link == true)
            {
                LinkWithBallJoints(component);
            }
        }
      }
    [ContextMenu("Export config file")]
    public void ExportConfig()
    {
        components = GetComponents();
        string data;
        string path = Environment.CurrentDirectory + @"\Assets\Scripts\Parser\" + ConfigName + ".txt";
        Debug.Log("Config saved to " + path);
        //clear file
        File.WriteAllText(path, String.Empty);

        using (StreamWriter file = new System.IO.StreamWriter(path, true)) {
            file.WriteLine(WheelMultiplier);
            file.WriteLine(JointMultiplier);

            foreach (Component component in components) {
                data = component.PartName + ";" + component.Movable + ";" + component.Type + ";" + component.DimensionMultipliers[0] + ";" + component.DimensionMultipliers[1] + ";" + component.DimensionMultipliers[2] + ";" + component.ActionIndeces[0] + ";" + component.ActionIndeces[1] + ";" + component.ActionIndeces[2] + ";";
                file.WriteLine(data);
            }
            file.Flush();
        }
        
        

    }
        

    //Moves all movable parts
    public void MoveMovableParts(float[] act)
    {

        motor = this.gameObject.GetComponent<MovementControls>();
        foreach (Component component in GetComponents())
        {
            if(component.Movable == true)
            {
                
                switch (component.Type)
                {

                    case Component.Type_.Wheel: {
                            motor.Wheel(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[component.ActionIndeces[0]]*component.DimensionMultipliers[0]);
                            break;
                        }
                    
                    case Component.Type_.Joint:
                        {
                            for(int i=0; i<component.DimensionMultipliers.Length; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0)
                                {
                                    motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[component.ActionIndeces[i]]*component.DimensionMultipliers[i],i);

                                }
                            }
                            
                         //   motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(),);
                            break;
                        }

                }
             
            }
        }
    }

    void LinkWithBallJoints(Component component)
    {
        if (component.gameObject.GetComponent<BallJoint>() == null)
        {
            component.gameObject.AddComponent<BallJoint>();
        }
        BallJoint joint = component.gameObject.GetComponent<BallJoint>();
        //Add rigidbody to parent if it doesn't have it
        CheckRigidbody(component.gameObject.transform.parent.gameObject);

        joint.connected = component.gameObject.transform.parent.GetComponent<Rigidbody>();
        //give balljoint script some default values
        joint.maxVerticalForce = 550;
        joint.maxHorizontalForce = 550;
        joint.angleLimit = 175;
        joint.errorThreshold = 5;
    }


   void CheckRigidbody(GameObject component)
    {
        if (component.GetComponent<Rigidbody>() == null)
        {
            component.AddComponent<Rigidbody>();
        }

        if (AddColliders == true && component.GetComponent<BoxCollider>() == null && component.GetComponent<SphereCollider>() == null && component.GetComponent<MeshCollider>() == null)
        {
            Debug.Log("AddingCollider to" + component.name);
            component.AddComponent<BoxCollider>();
            //get colliders size from Renderer
            if (component.GetComponent<Renderer>() != null) {
                component.GetComponent<BoxCollider>().size = component.GetComponent<Renderer>().bounds.size;
                }
            //set default small values if renderer does not exist. For example in case of bones
            else 
            {
                Vector3 vector = new Vector3 (0.5f, 0.5f, 0.5f);
                component.GetComponent<BoxCollider>().size = vector; 
            }
        }
    } 




}
