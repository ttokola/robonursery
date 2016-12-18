// Autonomous actions which can possibly override anything else, like manual control or queue executing

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class LoadBattery : MonoBehaviour 
{
    public Battery battery;
    public DockLoader[] loadingAreas;
    public MovementControls movementControls;
    
    private Vector3 destination;
    private bool hasDest = false;
	
	public int Execute ()
    // Attempt to load at a loading station
    // Return codes:
    // 0: Successfully loaded or already full battery
    // 1: Cannot find a free loading station
    // 2: moving towards loading station
	{
        if (battery.normLevel >= 1.0f)
        {
            hasDest = false;
			return 0;
		}
        
        if (hasDest)
        {
            movementControls.DriveTo(destination, true);
            return 2;
        }   
        
        foreach (var area in loadingAreas)
        {
            if (area.IsFree())
            {
                destination = area.transform.position;
                hasDest = true;
                break;
            }
        }
        if (!hasDest)
        {
            return 1;
        }
		return 2;
	}
}

} // End namespace