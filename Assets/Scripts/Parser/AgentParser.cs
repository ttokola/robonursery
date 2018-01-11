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
    //Set default values for the Components variables.Change these if you want the initialize parser function to give different default values
    //If you want to set certain values to different components based on their name or some other factor ovveride InitializationSettings() function at the AgentProto.cs 
    private bool defaultMovable=false;
    private Component.Type_ defaultMotor = Component.Type_.AddRelativeTorque;
    private Component.ConnectJointTo_ defaultConnectJointTo= Component.ConnectJointTo_.None;
    private Component.JointType_ defaultJointType= Component.JointType_.ConfigurableJoints;
    private Component.Collider_ defaultCollider= Component.Collider_.None;
    private Vector3 defaultDimensionMultiplers = new Vector3(0, 0, 0);
    //Note that if you set this vectors default values to be >=0 it might cause unintended interactions with the agent.
    private Vector3 defaultActionIndices = new Vector3(-1, -1, -1);

        



    [System.Serializable]
    public class Component
    {
        //variables used by ResetAgentPose()
        [HideInInspector]
        public Vector3 transformsPosition;
        [HideInInspector]
        public Quaternion transformsRotation;
        //All enum type variable are declared here
        public enum Type_
        {
            AddRelativeTorque,
            AddTorque
        }
        public enum JointType_
        {

            ConfigurableJoints,
            HingeJoints
        }
        public enum ConnectJointTo_
        {

            None,
            ConnectToParent,
            ConnectToGrandparent

        }
        public enum Collider_
        {

            None,
            BoxCollider,
            MeshCollider,
            SphereCollider,
            CapsuleCollider

        }

        //Component variables visible at the inspector UI
        [Tooltip("Name of the gameobject")]
        public string PartName;
        [Tooltip("Link to the gameobject")]
        public GameObject gameObject;
        [Tooltip("Sets object to movable.")]
        public bool Movable;
        [Tooltip("If the part is movable it uses this type of motor to move")]
        public Type_ Motor;
        [Tooltip("Set where you want to connect this object. This only matters if you run the script with Link flag")]
        public ConnectJointTo_ ConnectJointTo;
        [Tooltip("If you connect this object to something it is done with this type of joint")]
        public JointType_ JointType;
        [Tooltip("Set what type of collider you want to add to the object.")]
        public Collider_ Collider;
        [Tooltip("Add multiplier to X,Y,Z movement.")]
        public Vector3 DimensionMultipliers;
        [Tooltip("Set what action from the AI corresponds to which above movements")]
        public Vector3 ActionIndeces;
        
        
        
    }

    private MovementControls motor;
    //Parser variables
    [Tooltip("When script export config is run from context menu it creates a text file with this name")]
    public string ConfigName = "Config";
    [Tooltip("When dimension multiplier flag active, add this value to object with AddTorque motor ")]
    public int AddTorqueMultiplier = 1;
    [Tooltip("When dimension multiplier flag active, add this value to object with AddRelativeTorque motor ")]
    public int AddRelativeTorqueMultiplier = 1;

    //Parser flags
    [Tooltip("Check to run checked attributes on start. With this you dont have to run the script from context menu. All the joint, colliders etc also disappear when session is stopped")]
    public Boolean Run;
    [Tooltip("Adds colliders to checked objects")]
    public Boolean AddColliders;
    [Tooltip("Links components to their desired target")]
    public Boolean Link;
    [Tooltip("Automatically assigns action indices to movable objects")]
    public Boolean AutoSetActionIndices;
    [Tooltip("Automatically assign dimension multiplier to movable objects")]
    public Boolean AutoSetDimensionMultipliers;
    public int actsize;
    



    public Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
    /**< \brief Keeps track of the agents which subscribe to this proto. NOT IN USE*/

    [SerializeField]
    public List<Component> components = new List<Component>();

    
    [HideInInspector]
    protected Vector3 defaultJointAxes = new Vector3(0, 1, 1);
    

    [HideInInspector]
    protected Vector3 defaultAddTorqueAxes = new Vector3(1, 0, 0);

    //This is executed when Unity is in play mode
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

    }
   
    //Initialization. Parses through all the children of the gameobject and sets default values to their corresponding component 
    [ContextMenu("Initialize AgentProto")]
    void InitializeParser()
    {
        //Add MovementControls script to this gameobject if it does not exist
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
                
                var component = new Component();
                
                component.PartName = child.name;
                component.gameObject = child.gameObject;
                component.transformsPosition = child.position;
                component.transformsRotation = child.rotation;
                //set default values. Assign them at the beginning   
                component.Movable= defaultMovable;
                component.Motor = defaultMotor;
                component.ConnectJointTo = defaultConnectJointTo;
                component.JointType = defaultJointType;
                component.Collider = defaultCollider;
                component.DimensionMultipliers = defaultDimensionMultiplers;
                component.ActionIndeces = defaultActionIndices;

                //Calls virtual function InitializationSettings. 
                //Modify this function in AgentProto if you want to Initialize certain components with different values
                component =InitializationSettings(component);


                components.Add(component);
            }
        }
    }
 //Parses through all the components and does the flagged operations.
    [ContextMenu("Run the flagged operations")]
    void AddIndeces()
    {
        motor = this.gameObject.GetComponent<MovementControls>();
        int number = 0;
        foreach (Component component in GetComponents())
        {
            if (component.Movable == true)
            {
                    CheckRigidbody(component.gameObject, component.Collider);

                switch (component.Motor)
                {

                    case Component.Type_.AddTorque:
                        {
                            if (AutoSetDimensionMultipliers == true)
                            {
                                AddTorqueMultipliers(component);
                                
                            }
                            if (AutoSetActionIndices == true)
                            {
                                number = AddTorqueAction_Indices(component,number);
                                
                            }
                            break;
                        }

                    case Component.Type_.AddRelativeTorque:
                        {
                            if (AutoSetDimensionMultipliers == true)
                            {
                                JointMultipliers(component);
                                
                               
                            }
                            if (AutoSetActionIndices == true)
                            {
                               number = JointAction_Indices(component, number);

                            }
                            break;
                        }

                }

            }
            if ((component.ConnectJointTo != Component.ConnectJointTo_.None) && Link == true)
            {
                if (component.JointType == Component.JointType_.ConfigurableJoints) {
                    LinkWithBallJoints(component);
                }
                else if (component.JointType == Component.JointType_.HingeJoints)
                {
                    LinkWithHingeJoints(component);
                }
            }
        }
        actsize = number;
    }
    [ContextMenu("Export config file")]

    //Note this function is not up to date. It is left here to function as a base if exporting is needed
    public void ExportConfig()
    {
        components = GetComponents();
        string data;
        string path = Environment.CurrentDirectory + @"\Assets\Scripts\Parser\" + ConfigName + ".txt";
        Debug.Log("Config saved to " + path);
        //clear file
        File.WriteAllText(path, String.Empty);

        using (StreamWriter file = new System.IO.StreamWriter(path, true)) {
            file.WriteLine(AddTorqueMultiplier);
            file.WriteLine(AddRelativeTorqueMultiplier);

            foreach (Component component in components) {
                data = component.PartName + ";" + component.Movable + ";" + component.Motor + ";" + component.DimensionMultipliers[0] + ";" + component.DimensionMultipliers[1] + ";" + component.DimensionMultipliers[2] + ";" + component.ActionIndeces[0] + ";" + component.ActionIndeces[1] + ";" + component.ActionIndeces[2] + ";";
                file.WriteLine(data);
            }
            file.Flush();
        }



    }

    //Parses through all the components and moves the ones that are marked movable by
    //feeding values from float act[] to motors, intended to be used in Agent.AgentStep()
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

                switch (component.Motor)
                {

                    case Component.Type_.AddTorque: {
                            for (int i = 0; i < 3; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0 && component.ActionIndeces[i] >= 0)
                                {
                                    motor.AddTorque(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[(int)component.ActionIndeces[i]] * component.DimensionMultipliers[i], i);

                                }
                            }
                            break;

                        }

                    case Component.Type_.AddRelativeTorque:
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (component.DimensionMultipliers[i] != 0 && component.ActionIndeces[i] >= 0)
                                {
                                    motor.Joint(component.gameObject.GetComponent<Rigidbody>(), component.gameObject.GetComponent<Transform>(), act[(int)component.ActionIndeces[i]] * component.DimensionMultipliers[i], i);

                                }
                            }

                            break;
                        }

                }

            }
        }
    }

    private void LinkWithBallJoints(Component component)
    {        
        if (component.gameObject.GetComponent<ConfigurableJoint>()== null)
        {
            component.gameObject.AddComponent<ConfigurableJoint>();
        }
        ConfigurableJoint joint = component.gameObject.GetComponent<ConfigurableJoint>();        
        //Add rigidbody and colliders + connects the object to its parent/grandparent
        if ((component.ConnectJointTo == Component.ConnectJointTo_.ConnectToGrandparent) && component.gameObject.transform.parent.parent.gameObject != this.gameObject)
        {
            //Connected object needs to have a collider. It has to be forced if its none
            if(GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider == Component.Collider_.None)
            {
                GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider = Component.Collider_.BoxCollider;
            }
            CheckRigidbody(component.gameObject.transform.parent.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
        }
        else if ((component.ConnectJointTo == Component.ConnectJointTo_.ConnectToParent) && component.gameObject.transform.parent.gameObject != this.gameObject)
        {
            //Connected object need to have a collider. It has to be forced if its none
            if (GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider == Component.Collider_.None)
            {
                GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider = Component.Collider_.BoxCollider;
            }
            CheckRigidbody(component.gameObject.transform.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("There seems to be a issue with linking in object " + component.PartName);
           
        }
        ConfigurableJointSettings(component, joint);
        
    }

    private void LinkWithHingeJoints(Component component)
    {
        CheckRigidbody(component.gameObject, component.Collider);
        
        if (component.gameObject.GetComponent<HingeJoint>() == null)
        {
            component.gameObject.AddComponent<HingeJoint>();    
        }
        HingeJoint joint = component.gameObject.GetComponent<HingeJoint>();
        
        //Add rigidbody to parent if it doesn't have it

        if ((component.ConnectJointTo == Component.ConnectJointTo_.ConnectToGrandparent) && component.gameObject.transform.parent.parent.gameObject != this.gameObject)
        {
            //Connected object need to have a collider. It has to be forced if its none
            if (GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider == Component.Collider_.None)
            {
                GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider = Component.Collider_.BoxCollider;
            }

            CheckRigidbody(component.gameObject.transform.parent.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
            joint.connectedAnchor = component.gameObject.transform.position;
        }
        else if ((component.ConnectJointTo == Component.ConnectJointTo_.ConnectToParent) && component.gameObject.transform.parent.gameObject != this.gameObject)
        {
            //Connected object need to have a collider. It has to be forced if its none
            if (GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider == Component.Collider_.None)
            {
                GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider = Component.Collider_.BoxCollider;
            }

            CheckRigidbody(component.gameObject.transform.parent.gameObject, GetComponentByName(component.gameObject.transform.parent.gameObject.name).Collider);
            joint.connectedBody = component.gameObject.transform.parent.GetComponent<Rigidbody>();
            joint.connectedAnchor = component.gameObject.transform.position;

        }
        else
        {
            Debug.Log("There seems to be a issue with linking in object " + component.PartName + " Check if you have both ConnectJointTo_parent and ConnectJointTo_to_grandparent active");
            
        }
        HingeJointSettings(component, joint);
        }

    private void CheckRigidbody(GameObject gameobject, Component.Collider_ collider)
    {
        if (gameobject.GetComponent<Rigidbody>() == null)
        {
            gameobject.AddComponent<Rigidbody>();
            
            RigidbodySettings(gameobject);
        }
        
       
        if (AddColliders == true && gameobject.GetComponent<BoxCollider>() == null && gameobject.GetComponent<SphereCollider>() == null && gameobject.GetComponent<MeshCollider>() == null && gameobject.GetComponent<CapsuleCollider>() == null)
        {
            Debug.Log("AddingCollider to" + gameobject.name);

            switch (collider)
            {
                case Component.Collider_.BoxCollider:
                    {
                        gameobject.AddComponent<BoxCollider>();
                        BoxColliderSettings(gameobject);
                        break;
                    }
                case Component.Collider_.MeshCollider:
                    {
                        MeshCollider mesh = gameobject.AddComponent<MeshCollider>();
                        MeshColliderSettings(gameobject);
                        
                        break;
                    }
                case Component.Collider_.CapsuleCollider:
                    {
                         gameobject.AddComponent<CapsuleCollider>();
                         CapsuleColliderSettings(gameobject);
                        break;
                    }
                case Component.Collider_.SphereCollider:
                    {
                        gameobject.AddComponent<SphereCollider>();
                        SphereColliderSettings(gameobject);
                        break;
                    }


            }

           
        }
    }
    //Resets the position and velocity of each GameObject in the agent to default values at Vector3 position location. Intended to be used in Agent.reset() when resetting the agent or environment for next step in training.

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
    //Resets components and velocities to the starting values.
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

    //Virtual functions
    //Modify these in here or in AgentProto.

    // With this function you can set specific values to your components during initialization.
    //If you want to modify the default values given to all variables see the top of AgentParser class for default variables


        //Modify components initialization values
    public virtual Component InitializationSettings(Component component)
    {
        /*
        if (component.PartName.Contains("_movable"))
        {
            component.Movable = true;
            component.Collider = Component.Collider_.BoxCollider;

        }

        //sets components with name containing AddTorque and movable variable true to have mesh_colliders value true. 
        if (component.PartName.Contains("AddTorque") && component.Movable == true)
        {
            component.Collider = Component.Collider_.MeshCollider;
        }


        //set default values to 0
        component.DimensionMultipliers = new Vector3(0, 0, 0);


        //Sets variables to certain values depending on their name
        if (component.PartName.Contains(motorname1) || component.PartName.Contains("Axle"))
        {
            component.Motor = Component.Type_.AddTorque;
        }

        if (component.PartName.Contains("Axle") || component.PartName.Contains("Neck") || component.PartName.Contains("Head") || component.PartName.Contains("Tooth") || component.PartName.Contains("Eye"))
        {
            component.ConnectJointTo = Component.ConnectJointTo_.ConnectToParent;
        }

        if (component.PartName.Contains("Arm") || component.PartName.Contains("AddTorque_movable"))
        {
            component.ConnectJointTo = Component.ConnectJointTo_.ConnectToGrandparent;
        }*/
         
    //Set default values to components variables
  

        return component;
    }

    
    //With this you can modify the parameters given to all Configurablejoints in your system
    public virtual void ConfigurableJointSettings(Component component, ConfigurableJoint joint)
    {
        //joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        //joint.angularXMotion = ConfigurableJointMotion.Locked;
        //joint.anchor = new Vector3(0f, 0f, 0f);
    }
    //With this you can modify the parameters given to all Hingejoints in your system
    public virtual void HingeJointSettings(Component component, HingeJoint joint)
    {
        //joint.anchor = new Vector3(0f, 0f, 0f);
    }

    //modify RigidBody settings
    public virtual void RigidbodySettings(GameObject gameobject)
    {
        //if (gameObject.name == "Body")
        //{
          //  gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
       // }
    }
    //Below settings for all of the colliders

    public virtual void BoxColliderSettings(GameObject gameobject)
    {

    }
    public virtual void MeshColliderSettings(GameObject gameobject)
    {
        //mesh = gameobject.GetComponent<MeshCollider>();
        //mesh.convex = true;
        //Mesh colliders sometimes don't appear without this. Remove/add when needed 
        //mesh.inflateMesh = true;
    }
    public virtual void SphereColliderSettings(GameObject gameobject)
    {
        
    }
    public virtual void CapsuleColliderSettings(GameObject gameobject)
    {

    }

    //These functions define how "Auto set action indices" and "Auto set dimension multipliers" flags work.
    public virtual void AddTorqueMultipliers(Component component)
    {
        if ((AutoSetActionIndices && AutoSetDimensionMultipliers) == true)
        {
            component.DimensionMultipliers = defaultAddTorqueAxes * AddTorqueMultiplier;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.ActionIndeces[i] >= 0)
                {
                    component.DimensionMultipliers[i] = AddTorqueMultiplier;
                }
            }
        }
    }
    public virtual int AddTorqueAction_Indices(Component component, int number)
    {
        if ((AutoSetActionIndices && AutoSetDimensionMultipliers) == true)
        {

            for (int i = 0; i < 3; i++)
            {
                if (defaultAddTorqueAxes[i] != 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.DimensionMultipliers[i] > 0)
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
        if ((AutoSetActionIndices && AutoSetDimensionMultipliers) == true)
        {
            component.DimensionMultipliers = defaultJointAxes * AddRelativeTorqueMultiplier;

        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.ActionIndeces[i] >= 0)
                {
                    component.DimensionMultipliers[i] = AddRelativeTorqueMultiplier;
                }
            }
        }

    }

    public virtual int JointAction_Indices(Component component, int number)
    {
        if ((AutoSetActionIndices && AutoSetDimensionMultipliers) == true)
        {

            for (int i = 0; i < 3; i++)
            {
                if (defaultJointAxes[i] != 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (component.DimensionMultipliers[i] > 0)
                {
                    component.ActionIndeces[i] = number;
                    number = number + 1;
                }

            }
        }
        return number;

    }
}
