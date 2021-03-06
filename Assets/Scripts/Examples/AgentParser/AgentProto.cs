using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Example of a class that inherits AgentParser. Utilize classes like this to automate your robot building.
//Following code build robot "Ruttunen" to a functioning state excluding eyes and mouth. Script keeps the default weights for all of the parts
//for more realistic version of the bot tinker with RigidBody weights and give each part different amounts of force to make robots movements more natural 
public class AgentProto : AgentParser {
    
    //Write your override functions here.
    //See below for an examples

    override public Component InitializationSettings(Component component)
    {
        //Movable
        if (component.PartName == "LWheel" || component.PartName.Contains("Arm") || component.PartName == "RWheel" || component.PartName == "Head")
        {
            component.Movable = true;

        }

        //MeshColliders 
        if (component.PartName.Contains("Wheel") && component.Movable == true)
        {
            component.Collider = Component.Collider_.MeshCollider;

        }

        //BoxColliders
        if (component.PartName.Contains("Arm") || component.PartName.Contains("Neck") || component.PartName.Contains("Head"))
        {
            component.Collider = Component.Collider_.BoxCollider;
        }

        
        //Connect to parent
        if (component.PartName.Contains("Axle") || component.PartName.Contains("Head") || component.PartName == "Neck")
        {
            component.ConnectJointTo = Component.ConnectJointTo_.ConnectToParent;

        }
        //Connect to grandParent
        if (component.PartName.Contains("Arm") || component.PartName == "RWheel" || component.PartName == "LWheel")
        {
            component.ConnectJointTo = Component.ConnectJointTo_.ConnectToGrandparent;
        }

        //hingeJoints
        if (component.PartName.Contains("Axle") || component.PartName == "LWheel" || component.PartName == "RWheel")
        {
            component.JointType = Component.JointType_.HingeJoints;
        }



        return component;
    }

    override public void ConfigurableJointSettings(Component component, ConfigurableJoint joint)
    {
        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        if (component.PartName == "Neck")
        {
            joint.angularZMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
        }

        if (component.PartName == "Head")
        {
            joint.angularZMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = ConfigurableJointMotion.Limited;
        }
        joint.anchor = new Vector3(0f, 0f, 0f);

    }

    public override void RigidbodySettings(GameObject gameobject)
    {

        if (gameobject.name == "Body")
        {    
            //This locks the robot in to a standing position. Remove these and the robot behaves more naturally 
            gameobject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
        }
    }
    public override void HingeJointSettings(Component component, HingeJoint joint)
    {
        joint.anchor = new Vector3(0f, 0f, 0f);
    }

    public override void MeshColliderSettings(GameObject gameobject)
    {
        MeshCollider mesh = gameobject.GetComponent<MeshCollider>();
        mesh.convex = true;
        //Mesh colliders sometimes don't appear without this. Remove/add when needed 
        mesh.inflateMesh = true;
    }

    public override void JointMultipliers(Component component)
    {
        if ((AutoSetActionIndices && AutoSetDimensionMultipliers) == true)
        {
            if (component.PartName == "LWheel" || component.PartName == "RWheel")
            {
                component.DimensionMultipliers = new Vector3(1, 0, 0) * AddRelativeTorqueMultiplier;
            }
            else
            {
                component.DimensionMultipliers = defaultJointAxes * AddRelativeTorqueMultiplier;
            }
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

    public override int JointAction_Indices(Component component, int number)
    {
        if ((AutoSetActionIndices && AutoSetDimensionMultipliers) == true)
        {
            if (component.PartName == "LWheel" || component.PartName == "RWheel")
            {
                component.ActionIndeces[0] = number;
                number = number + 1;
            }
            else {
                for (int i = 0; i < 3; i++)
                {

                    if (defaultJointAxes[i] != 0)
                    {
                        component.ActionIndeces[i] = number;
                        number = number + 1;
                    }
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