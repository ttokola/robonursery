using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class config : AgentParser {

	public override Component InitializationSettings(Component component)
    {
        component.ConnectJointTo = Component.Link_.None;
        if (component.PartName.Contains("Body"))
        {
            component.gameObject.AddComponent<Rigidbody>();
            component.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        }
        if (component.PartName.Contains("_movable"))
        {
            component.Movable = true;
            component.Collider = Component.Collider_.BoxCollider;
        }
        component.DimensionMultipliers = new Vector3(0, 0, 0);
        if (component.PartName.Contains("Wheel_movable"))
        {
            component.ConnectJointTo = Component.Link_.ConnectToGrandparent;
            component.DimensionMultipliers = new Vector3(1, 0, 0) * AddTorqueMultiplier;
            component.Collider = Component.Collider_.MeshCollider;
            component.Motor = Component.Type_.AddRelativeTorque;

        }
        if(component.PartName.Contains("Wheel_movable") || component.PartName.Contains("Axle") || component.PartName.Contains("Neck") || component.PartName.Contains("Head"))
        {
            component.JointType = Component.Connection_.HingeJoints;
        }
        if(component.PartName.Contains("Axle") || component.PartName.Contains("Neck") || component.PartName.Contains("Head") || component.PartName.Contains("Tooth") || component.PartName.Contains("Eye"))
        {
            component.ConnectJointTo = Component.Link_.ConnectToParent;
        }
        if (component.PartName.Contains("Arm"))
        {
            component.DimensionMultipliers = new Vector3(0, 1, 1) * AddRelativeTorqueMultiplier;
            component.ConnectJointTo = Component.Link_.ConnectToGrandparent;
            component.Motor = Component.Type_.AddRelativeTorque;
        }
        return component;
    }
}
