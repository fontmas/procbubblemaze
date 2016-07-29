/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;
using Utils.Map;

public class MenuCamera : MonoBehaviour {

	public float timeNextGenerationMap = 30.0f;
	public bool turnAround = false;
	public float speedTurnAround = 30.0f;
	private float countTime = 0.0f;
	private MapGenerator mapGen = null;
	private PlaceObjectMap placement = null;

	IEnumerator Start () {
		mapGen = FindObjectOfType<MapGenerator> ();
		placement = FindObjectOfType<PlaceObjectMap> ();
		yield return StartCoroutine( DoPreviewCamera());
	}
	
	// Update is called once per frame
	void Update () {
		if (mapGen != null) {
			if(turnAround) transform.RotateAround(mapGen.transform.position,Vector3.forward,Time.deltaTime*speedTurnAround);
			countTime += Time.deltaTime;
			if(countTime >= timeNextGenerationMap){
				DoPreviewCamera();
			}
			transform.position = new Vector3 (mapGen.transform.position.x,mapGen.transform.position.y,-1.0f*(mapGen.GetMapSize()));
		}
	}

	public IEnumerator DoPreviewCamera() {
		countTime = 0.0f;
		yield return StartCoroutine( placement.RefreshPlaceObjectMap() );
		yield return StartCoroutine( mapGen.GenerateMap () );
	}
}
