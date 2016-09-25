using UnityEngine;
using System.Collections;

public static class Utils
{
	public static float Vec3FullAngle(Vector3 from, Vector3 to)
	// Return the angle between from and to, from -180 to 180 degrees,
	// positive if clockwise
	// http://answers.unity3d.com/questions/24983/how-to-calculate-the-angle-between-two-vectors.html
	{
		Vector3 _ref = Vector3.Cross(Vector3.up, from);
		float angle = Vector3.Angle(to, from);
		float sign = Mathf.Sign(Vector3.Dot(to, _ref));
		return sign * angle;
	}
}