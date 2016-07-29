using UnityEngine;
using System.Collections;

public class BehaviourFireBall : MonoBehaviour {

	public GameObject prefab = null;
	public float timeRefreshFire = 1.0f;
	public Vector2 directionFire = Vector2.right;

	void Start () {
		InvokeRepeating ("DoFire", timeRefreshFire, timeRefreshFire);
	}
	
	void DoFire () {
		GameObject clone = Instantiate (prefab);
		clone.transform.position = gameObject.transform.position;
		clone.transform.rotation = gameObject.transform.rotation;
		Rigidbody2D cloneRigidbody2D = clone.GetComponent<Rigidbody2D> ();
		if (cloneRigidbody2D != null) {
			cloneRigidbody2D.AddForce (directionFire,ForceMode2D.Impulse);
		}
	}
}
