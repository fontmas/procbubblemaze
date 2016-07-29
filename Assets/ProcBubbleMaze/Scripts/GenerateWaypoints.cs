using UnityEngine;
using System.Collections;

public class GenerateWaypoints : MonoBehaviour {

	public int total = 8;
	public float forceMin = 1.0f;
	public float forceMax = 1.0f;
	public Vector3 defaultAxiRotation = Vector3.forward;
	public GameObject prefabWayPoint;

	void Start() {
		DoWayPoints ();
	}

	void DoWayPoints () {
		for (int i = 0; i < total; i++) {
			GameObject obj = Instantiate(prefabWayPoint) as GameObject;
			obj.name = name + " W"+(i+1);
			obj.transform.parent = transform;
			// Position to rotate around this object
			obj.transform.position = transform.position + Vector3.right*(transform.localScale.x+0.1f);
			// Rotate arond
			obj.transform.RotateAround(transform.position,defaultAxiRotation,i*(360/total));
			// add the waypoints as child of this object
			obj.GetComponent<Rigidbody2D>().AddForce(obj.transform.right*Random.Range(forceMin,forceMax),ForceMode2D.Impulse);
		}
	}
	
}
