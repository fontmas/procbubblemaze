using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {

	public bool FadeMaterial = true;
	public float timeToLive;
	private float startTime = 0.0f;
	private MeshRenderer myTargetRender = null;

	void Start () {
		if(FadeMaterial) myTargetRender = GetComponent<MeshRenderer> ();
		startTime = Time.time;
	}

	void Update () {
		if (timeToLive <= Time.time - startTime) {
			if(myTargetRender != null) myTargetRender.material.color = 
				new Color ( myTargetRender.material.color.r,
					myTargetRender.material.color.g,
					myTargetRender.material.color.b,
					(Time.time - startTime) / (timeToLive));
		} else {
			Destroy (gameObject);
		}
	}
}

