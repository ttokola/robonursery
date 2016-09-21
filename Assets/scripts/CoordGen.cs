using UnityEngine;
using System.Collections;

public class CoordGen : MonoBehaviour {

	public GameObject coordtext;
	
	private Renderer rend;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		Debug.Log (rend.bounds.extents);
		int zmin = (int) (rend.bounds.center.z - rend.bounds.extents.z);
		int xmin = (int) (rend.bounds.center.x - rend.bounds.extents.x);
		int zmax = (int) (rend.bounds.extents.z + rend.bounds.center.z);
		int xmax = (int) (rend.bounds.extents.x + rend.bounds.center.x);
		for (int z = zmin; z <= zmax; z++) {
			for (int x = xmin; x <= xmax; x++) {
				GameObject g = Instantiate(coordtext);
				g.transform.position = new Vector3(x, g.transform.position.y, z);
				g.GetComponent<TextMesh> ().text = string.Format("{0},{1}", x, z);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
