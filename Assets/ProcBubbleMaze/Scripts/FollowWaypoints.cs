using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GenerateWaypoints))]
public class FollowWaypoints : MonoBehaviour {
	
	public List<Vector3> listPositions;
	private Vector3 target;
	private int index = 0;
	public bool random = false;
	public float timeWait = 1.0f;
	public float countTime = 0.0f;
	public float MinSpeed = 1.0f;
	public float MaxSpeed = 4.0f;

	void DoFollowWayPoints () {
		Transform[] listTransform = gameObject.GetComponentsInChildren<Transform> (true);
		listPositions = new List<Vector3>();
		for(int i = 0; i < listTransform.Length;i++) {
			if(listTransform[i].gameObject.Equals(gameObject)==false) listPositions.Add( listTransform[i].position );
		}
		for(int i = 0; i < listTransform.Length;i++) {
			if(listTransform[i].gameObject.Equals(gameObject)==false)Destroy(listTransform[i].gameObject);
		}
		index = random?Random.Range(0,listPositions.Count-1):0;
		target = listPositions [index];
	}

	void Update () {
		if (countTime <= timeWait) {
			countTime += Time.deltaTime;
			if (countTime >= timeWait) {
				DoFollowWayPoints ();
			} else {
				return;
			}
		} else {
			if(listPositions.Count <=0) return;
			if (Vector3.Distance (GetComponent<Transform> ().position, target) <= 0.5) {
				index = random ? Random.Range (0, listPositions.Count - 1) : index++;
				if (index >= listPositions.Count)
					index = 0;
				target = listPositions [index];
			} else {
				GetComponent<Transform> ().position = Vector3.MoveTowards(GetComponent<Transform> ().position, target, Time.deltaTime * Random.Range(MinSpeed,MaxSpeed));
			}
		}
	}
}