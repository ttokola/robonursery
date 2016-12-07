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
    
    public static float AngleNorm360(float angle)
    // Return the angle normalized to the range [0, 360]
    {
        angle = angle % 360;
        if (angle < 0)
        {
            return 360 + angle;
        }
        return angle;
    }
    
    public static Vector3 AngleNorm360(Vector3 angle)
    {
        angle.x = AngleNorm360(angle.x);
        angle.y = AngleNorm360(angle.y);
        angle.z = AngleNorm360(angle.z);
        return angle;
    }
    
    public static float AngleDiff180(float angleFrom, float angleTo)
    // Return the angle between from and to, normalized to the range [-180,180]
    {
        angleFrom = AngleNorm360(angleFrom);
        angleTo = AngleNorm360(angleTo);
        // First two special cases handle the situation where we need to rotate
        // past the zero-point on a circle limited to 360 degrees of rotation
        if ((angleTo + (360-angleFrom)) < 180) // Clockwise rotation
        {
            return angleTo + (360-angleFrom);
        }
        else if ((angleFrom + (360-angleTo)) < 180) // Counterclockwise
        {
            return -(angleFrom + (360-angleTo));
        }
        return angleTo-angleFrom;
    }
    
    public static Vector3 AngleDiff180(Vector3 angleFrom, Vector3 angleTo)
    {
        var angle = new Vector3();
        angle.x = AngleDiff180(angleFrom.x, angleTo.x);
        angle.y = AngleDiff180(angleFrom.y, angleTo.y);
        angle.z = AngleDiff180(angleFrom.z, angleTo.z);
        return angle;
    }
}