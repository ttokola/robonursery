using UnityEngine;
using System.Collections;

public class DockLoader : MonoBehaviour {
	
	private Battery bat;

	void OnTriggerStay (Collider other)
	{
		bat = other.transform.root.gameObject.GetComponent<Battery> ();
		bat.level += Time.deltaTime * 5;
	}
}