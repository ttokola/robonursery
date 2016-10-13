using UnityEngine;
using System.Collections;

public static class Utils
{
	public static float PointAngle(Vector3 from, Vector3 to)
	// Return the angle between points from and to,
	// relative to unit vector in the x-direction
	{
		float s1 = to.x - from.x;
		float s2 = to.z - from.z;
		float angle = Mathf.Abs(Mathf.Atan(s2 / s1));
		if (from.x > to.x) {
			angle = Mathf.PI - angle;
		}
		if (from.z < to.z) {
			angle = -angle;
		}	
		return Mathf.Rad2Deg * angle;
	}
	
	public static float AngleTo(Vector3 pointFrom, Vector3 reference, Vector3 pointTo)
	// Return the angle between points from and to,
	// relative to reference vector
	{
		float refAngle = Vector3.Angle(Vector3.right, reference);
		if (reference.z > 0) { // It's on the left side
			refAngle = -refAngle;
		}
		float destPointAngle = PointAngle(pointFrom, pointTo);
		float destAngle = destPointAngle - refAngle;
		if (Mathf.Abs(destAngle) > 180) {
			return Mathf.Sign(destAngle) * -1 * (360 - Mathf.Abs(destAngle));
		}
		return destAngle;
	}
}