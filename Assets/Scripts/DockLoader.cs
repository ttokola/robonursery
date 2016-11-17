using UnityEngine;
using System.Collections;

public class DockLoader : MonoBehaviour {
 
    private int trigCount = 0;
    
	private Battery bat;
    
	void OnTriggerStay (Collider other)
	{
		bat = other.transform.root.gameObject.GetComponent<Battery> ();
        bat.Charge(10);
	}
    
    void OnTriggerEnter (Collider other)
	{
		trigCount++;
	}
    
    void OnTriggerExit (Collider other)
	{
		trigCount--;
	}
    
    public bool IsFree ()
    {
        return (trigCount == 0) ? true : false;
    }
}