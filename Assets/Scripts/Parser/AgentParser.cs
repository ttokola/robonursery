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
    public class Component
    {
        [Tooltip("When this is set with link flag, links the object to its grandparent")]
        public bool Link_to_grandparent;
        [Tooltip("If you have Link_grandparents active but want to link specific objects to their parents instead")]
        public Boolean Link_parent;
       // [Tooltip("Link this object to its parent when parser is run with Link flag")]
        //public bool Link;
        [Tooltip("Name of the gameobject")]
        public string PartName;
        [Tooltip("Link to the gameobject")]
        public GameObject gameObject;
        [Tooltip("Set this to be a movable part. ")]
        public bool Movable;

        public enum Type_
        {
            Joint,
            Wheel
        }
        public Vector3 transformsPosition;
        public Quaternion transformsRotation;
        [Tooltip("Sets the motor type of the object. See documentation for details")]
        public Type_ Type;
        [Tooltip("Add multiplier to X,Y,Z movement.")]
        public Vector3 DimensionMultipliers;
        [Tooltip("Set what action from the AI corresponds to which above movements")]
        public Vector3 ActionIndeces;
        [Tooltip("When adding box colliders, instead add a mesh collider")]
        public bool Mesh_collider;
        
        
    }

    

    // Use this for initialization
    private MovementControls motor;
    private string motorname1 = "Wheel";
    private string motorname2 = "Joint";
    [Tooltip("When script export config is run from context menu it creates a text file with this name")]
    public string ConfigName = "Config";
    [Tooltip("When dimension multiplier flag active, add this value to object with wheel motor ")]
    public int WheelMultiplier = 1;
    [Tooltip("When dimension multiplier flag active, add this value to object with Joint motor ")]
    public int JointMultiplier = 1;

    //Parser flags
    [Tooltip("Check to run checked attributes on start. With this you dont have to run the script from context menu. All the joint, colliders etc also disappear when session is stopped")]
    public Boolean Run;
    [Tooltip("Adds colliders to checked objects")]
    public Boolean AddColliders;
    [Tooltip("Links compponents which link attribute is true to their parent")]
    public Boolean Link;
    [Tooltip("Automatically assigns autoindicies to movable objects")]
    public Boolean Action_Indicies;
    [Tooltip("Automatically assign dimension multiplier to movable objects")]
    public Boolean Multipliers;
    //[Tooltip("Instead of linking the components with link attribute true to their parents, instead link them to their grand parent")]
   // public Boolean Link_grandparent;

   

    public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
    /**< \brief Keeps track of the agents which subscribe to this proto. NOT IN USE*/

    [SerializeField]
    public List<Component> components = new List<Component>();

    public int actsize;
    //[HideInInspector]
    protected Vector3 defaultJointAxes = new Vector3(0, 1, 1);
    

    //[HideInInspector]
    protected Vector3 defaultWheelAxes = new Vector3(0, 1, 1);

    void Start()
    {

        if (Run == true)
        {
            foreach(Component component in components){
                component.transformsPosition = component.gameObject.transform.position;
                component.transformsRotation = component.gameObject.transform.rotation;
            }
            AddIndeces();
        }

    }

    public List<Component> GetComponents()
    {
        return components;
    }

    public Component GetComponentByName(string name)
    {
        Component result = new Component();
        foreach (Component component in components)
        {
            if (component.PartName == name)
            {
                return component;
            }
        }
        Debug.LogError("No component exists by given name");
        return result;
    }

    private Dictionary<int, List<Component>> CollectComponents()
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
        if (this.gameObject.GetComponent<MovementControls>() == null)
        {
            this.gameObject.AddComponent<MovementControls>();
        }

        components.Clear();



        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != allChildren[0])
            {
                //Here we set the default values for all the components variables.
                ////////
                ///If you want to set default values to certain variables based on their name make if statements here which sets the value.
                ///This can be a handly way to speed up the process of deploying new robots. GameObjects name can be obtained from PartName.Contains(string name) see below for example.
                ///////See below for examples 
                var component = new Component();
                
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.transformsPosition = child.position;
                component.transformsRotation = child.rotation;
                ///
                //// stuff below are optional. Boolean variables set themselves to false as default- It is preferred to keep 
                ///
                //if components name contains string "_movable" set objects movable variable to true
                if (component.PartName.Contains("_movable"))
                {
                    component.Movable = true;
                }
                else {
                    component.Movable = false;
                }
                //sets components with name containing wheel and movable variable true to have mesh_colliders value true. 
                if (component.PartName.Contains("Wheel") && component.Movable == true)
                {
                    component.Mesh_collider = true;
                }
                 //Set default values to -1    
                component.ActionIndeces = new Vector3(-1,-1,-1);
                
                //set default values to 0
                component.DimensionMultipliers = new Vector3(0, 0, 0); 
               

                //if objects name contains word "Wheel", set default type to wheel
                if (component.PartName.Contains(motorname1) || component.PartName.Contains("Axle"))
                {
                    component.Type = Component.Type_.Wheel;
                }

                //// optinal stuff ends
                components.Add(component);
            }
        }
    }

    [ContextMenu("Run the flagged operations")]
    void AddIndeces()
    {

        motor = this.gameObject.GetComponent<MovementControls>();
        int number = 0;
        foreach (Component component in GetComponents())
        {
            if (component.Movable == true)
            {
                if (component.Mesh_collider == true) {
                    CheckRigidbody(component.gameObject, 1);
                }
                else {
                    CheckRigidbody(component.gameObject, 0);
                }

                switch (component.Type)
                {

                    case Component.Type_.Wheel:
                        {
                            if (Multipliers == true)
                            {
                                WheelMultipliers(component);
                                
                            }
                            if (Action_Indicies == true)
                            {
                                number = WheelAction_Indicies(component,number);
                                
                            }
                            break;
                        }

                    case Component.Type_.Joint:
                        {
                            if (Multipliers == true)
                            {
                                JointMultipliers(component);
                                
                               
                            }
                            if (Action_Indicies == true)
                            {
                               number = JointAction_Indicies(component, number);

                            }
                            break;
                        }

                }

            }
            if ((component.Link_parent || component.Link_to_grandparent) == true && Link == true)
            {
                if (component.Type == Component.Type_.Joint) {
                    LinkWithBallJoints(component);
                }
                else if (component.Type == Component.Type_.Wheel)
                {
                    LinkWithHingeJoints(component);
                }
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
        if (this.gameObject.GetComponent<MovementControls>() == null)
        {
            this.gameObject.AddComponent<MovementControls>();
        }
        motor = this.gameObject.GetComponent<MovementControls>();
        foreach (Component component in GetComponents())
        {
            if (component.Movable == true)
            {

                switch (component.Type)
                {

                    case Component.Type_.Wheel: {
                            for (int i = 0; i < 3; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0 && component.ActionIndeces[i] >= 0)
                                {
                                    motor.Wheel(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[(int)component.ActionIndeces[i]] * component.DimensionMultipliers[i], i);

                                }
                            }
                            break;

                        }

                    case Component.Type_.Joint:
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0 && component.ActionIndeces[i] >= 0)
                                {
                                    motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[(int)component.ActionIndeces[i]] * component.DimensionMultipliers[i], i);

                                }
                            }

                            //   motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(),);
                            break;
                        }

                }

            }
        }
    }

    private void LinkWithBallJoints(Component component)
    {
        /*
        if (component.gameObject.GetComponent<BallJoint>() == null)
        {
            component.gameObject.AddComponent<BallJoint>();
        }
        BallJoint joint = component.gameObject.GetComponent<BallJoint>();
        //give balljoint script some default values
        joint.maxVerticalForce = 550;
        joint.maxHorizontalForce = 550;
        joint.angleLimit = 175;
        joint.errorThreshold = 5;
        */
        if (component.gameObject.GetComponent<ConfigurableJoint>()== null)
        {
            component.gameObject.AddComponent<ConfigurableJoint>();
        }
        ConfigurableJoint joint = component.gameObject.GetComponent<ConfigurableJoint>();
        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        

        
        //Add rigidbody and colliders + connects the object to its parent/grandparent

        if (((component.Link_to_grandparent == true && component.Link_parent == false) || component.Link_to_grandparent == true) && component.gameObject.transform.parent.parent.gameObject != this.gameObject)
        {
            CheckRigidbody(component.gameObject.transform.parent.parent.gameObject, 0);
            joint.connectedBody = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
           // joint.connected = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
        }
        else if ((component.Link_parent == true && component.Link_to_grandparent == false)&& component.gameObject.transform.parent.gameObject != this.gameObject)
        {
            CheckRigidbody(component.gameObject.transform.parent.gameObject, 0);
            joint.connectedBody = component.gameObject.transform.parent.GetComponent<Rigidbody>();
            //joint.connected = component.gameObject.transform.parent.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("There seems to be a issue with linking in object " + component.PartName + " Check if you have both link_parent and link_to_grandparent active");
            Debug.Log("Linking with parents as a default");
            //CheckRigidbody(component.gameObject.transform.parent.gameObject, 0);
            //joint.connected = component.gameObject.transform.parent.GetComponent<Rigidbody>();
        }
    }

    private void LinkWithHingeJoints(Component component)
    {
        if (component.Mesh_collider == true) {

            CheckRigidbody(component.gameObject, 1);
        }
        else
        {
            CheckRigidbody(component.gameObject, 0);
        }
        if (component.gameObject.GetComponent<HingeJoint>() == null)
        {
            component.gameObject.AddComponent<HingeJoint>();    
        }
        HingeJoint joint = component.gameObject.GetComponent<HingeJoint>();
        
        //Add rigidbody to parent if it doesn't have it

        if (((component.Link_to_grandparent == true && component.Link_parent == false) || component.Link_to_grandparent == true) && component.gameObject.transform.parent.parent.gameObject != this.gameObject)
        {

            CheckRigidbody(component.gameObject.transform.parent.parent.gameObject, 0);
            joint.connectedBody = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
            joint.connectedAnchor = component.gameObject.transform.position;
        }
        else if ((component.Link_parent == true && component.Link_to_grandparent == false)&& component.gameObject.transform.parent.gameObject != this.gameObject)
        {
            CheckRigidbody(component.gameObject.transform.parent.gameObject, 0);
            joint.connectedBody = component.gameObject.transform.parent.GetComponent<Rigidbody>();
            joint.connectedAnchor = component.gameObject.transform.position;

        }
        else
        {
            Debug.Log("There seems to be a issue with linking in object " + component.PartName + " Check if you have both link_parent and link_to_grandparent active");
            
        }
        //force anchor to be 0 0 0. This makes the wheels connect properly
        joint.anchor = new Vector3(0f,0f,0f);
        }

    private void CheckRigidbody(GameObject component, int collider)
    {
        if (component.GetComponent<Rigidbody>() == null)
        {
            component.AddComponent<Rigidbody>();
        }
        
       
        if (AddColliders == true && component.GetComponent<BoxCollider>() == null && component.GetComponent<SphereCollider>() == null && component.GetComponent<MeshCollider>() == null)
        {
            Debug.Log("AddingCollider to" + component.name);

            if (collider == 1)
            {
                MeshCollider mesh = component.AddComponent<MeshCollider>();
                mesh.convex = true;
                //Mesh colliders sometimes don't appear without this. Remove/add when needed 
                mesh.inflateMesh = true;
                
            }
            else {
                component.AddComponent<BoxCollider>();
                //get colliders size from Renderer
                if (component.GetComponent<Renderer>() != null) {
                    //component.GetComponent<BoxCollider>().size = component.GetComponent<Renderer>().bounds.size;

                    Debug.Log(component.name + component.GetComponent<Renderer>().bounds.size);
                }
                //set default small values if renderer does not exist. For example in case of bones
                else
                {
                    Vector3 vector = new Vector3(0.5f, 0.5f, 0.5f);
                    component.GetComponent<BoxCollider>().size = vector;
                }
            }
        }
    } 

    public void ResetAgentPose(Vector3 position)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Component component in components)
        {
            component.gameObject.transform.position = component.transformsPosition+position;
            component.gameObject.transform.rotation = component.transformsRotation;
            if (component.Movable)
            {
                component.gameObject.GetComponent<Rigidbody>().velocity = default(Vector3);
                component.gameObject.GetComponent<Rigidbody>().angularVelocity = default(Vector3);
            }

        }
    }

    public void ResetAgentPose()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Component component in components)
        {
            component.gameObject.transform.position = component.transformsPosition;
            component.gameObject.transform.rotation = component.transformsRotation;
            if (component.Movable)
            {
                component.gameObject.GetComponent<Rigidbody>().velocity = default(Vector3);
                component.gameObject.GetComponent<Rigidbody>().angularVelocity = default(Vector3);
            }

        }
    }
    public virtual void WheelMultipliers(Component component)
    {
        if ((Action_Indicies && Multipliers) == true) {
            component.DimensionMultipliers.x = WheelMultiplier;
            }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if(component.ActionIndeces[i] >= 0){
                    component.DimensionMultipliers[i] = WheelMultiplier; 
            }
            }
        }
    }
    public virtual int WheelAction_Indicies(Component component, int number) {
        if ((Action_Indicies && Multipliers) == true) {
            component.ActionIndeces.Set(number, -1, -1);

            number = number + 1;
        }
        else {
            for (int i = 0; i < 3; i++)
            {
                if (component.DimensionMultipliers[i] >= 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }

            }
            }
        return number;
    }

     public virtual void JointMultipliers(Component component)
    {
        if ((Action_Indicies && Multipliers) == true)
        {
            component.DimensionMultipliers.y = JointMultiplier;
            component.DimensionMultipliers.z = JointMultiplier;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.ActionIndeces[i] >= 0)
                {
                    component.DimensionMultipliers[i] = JointMultiplier;
                }
            }
        }

    }

    public virtual int JointAction_Indicies(Component component, int number)
    {
        if ((Action_Indicies && Multipliers) == true)
        {
            component.ActionIndeces.Set(-1, number, number+1);

            number = number + 2;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.DimensionMultipliers[i] >= 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }

            }
        }
        return number;

    }
    /*Add default axis multipliers based on the axes that have action_indeces set*/
    public virtual int AddMultipliers(Component component)
    {
        if ((Action_Indicies && Multipliers) == true)
        {
            if (component.Type == Component.Type_.Joint)
            {
                //component.DimensionMultipliers.y = JointMultiplier;
                //component.DimensionMultipliers.z = JointMultiplier;
                component.DimensionMultipliers = WheelMultiplier * defaultJointAxes;
            }
            if (component.Type == Component.Type_.Wheel)
            {
                //component.DimensionMultipliers.x = WheelMultiplier;
                component.DimensionMultipliers = WheelMultiplier * defaultWheelAxes;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.ActionIndeces[i] >= 0)
                {
                    if (component.Type == Component.Type_.Joint)
                    {
                        component.DimensionMultipliers[i] = JointMultiplier;
                    }
                    if (component.Type == Component.Type_.Wheel)
                    {
                        component.DimensionMultipliers[i] = WheelMultiplier;
                    }
                    
                }
            }
        }

        return -1;
    }

    public virtual int AddActionIndeces(Component component)
    {
        if (component.Type == Component.Type_.Joint)
        {
            if ((Action_Indicies && Multipliers) == true)
            {
                
                Vector3 a_i = Vector3.Scale(defaultJointAxes, new Vector3(1f, 1f, 1f));
                component.ActionIndeces.Set(actsize, -1, -1);

                actsize += 1;
            }
        }
        for (int i=0; 0<3; i++)
        {
            if (component.DimensionMultipliers[i] >= 0)
            {
                component.ActionIndeces[i] = actsize;
                actsize += 1;
            }
        }
        return -1;
    }

}
