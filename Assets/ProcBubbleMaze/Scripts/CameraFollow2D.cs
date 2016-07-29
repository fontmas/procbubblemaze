using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour {
	
	public bool followByTag = true;
	public string tagObject = "Player";
	public float speed = 3.0f;
	public GameObject targetObject = null;
	
	[Range(0.0f,1.0f)]
	public float propMax = 0.5f;
	
	[Range(0.0f,1.0f)]
	public float propMin = 0.05f;
	
	[Range(0.0f,1.0f)]
	public float propIncrement = 0.05f;

	string ZoomInputOut = "Fire1";
	string ZoomInputIn = "Fire3";

	private Utils.Map.MapGenerator mapGen = null;
	private float currentProp = 0.0f;

	void Start () {
		FindObjectTagged ();
		mapGen = FindObjectOfType<Utils.Map.MapGenerator> ();
		propMin = propMin / mapGen.tileScale;
		propMax = propMax * mapGen.tileScale;
		currentProp = ((propMin+propMax)*(2.0f/3.0f));
		ApplyPropCamera ();
	}
	
	void Update () {
		if (Input.GetButtonDown (ZoomInputOut)) {
			currentProp += propIncrement;
			currentProp = currentProp > propMax?propMax:currentProp;
		}
		if (Input.GetButtonDown (ZoomInputIn)) {
			currentProp -= propIncrement;
			currentProp = currentProp < propMin?propMin:currentProp;
		}
		if (targetObject == null) {
			FindObjectTagged ();
		}
		if (targetObject == null) {
			return;
		}
		ApplyPropCamera ();
	}
	void ApplyPropCamera ()	{
		if (targetObject == null)
			return;
		if (mapGen != null ) {
			if(GetComponent<Camera> ().orthographic) {
				GetComponent<Camera> ().orthographicSize = mapGen.GetMapSize() * currentProp;
			}
			transform.position = Vector3.Lerp(transform.position, new Vector3 (
				targetObject.transform.position.x, targetObject.transform.position.y, 
				-1.0f*(mapGen.GetMapSize() * currentProp)),Time.smoothDeltaTime*speed);
		}
	}
	void FindObjectTagged () {
		if (followByTag) {
			targetObject = (GameObject.FindWithTag (tagObject) as GameObject);
		}
	}
}
