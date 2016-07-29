using UnityEngine;
using System.Collections;
using Utils.Map;

public class EndLevel : MonoBehaviour {

	public string targetEndLevel = "Player";
	private GameManager gm = null;
	private MapGenerator mapGen = null;
	//private PlaceObjectMap placement = null;
	
	void Start () {
		mapGen = FindObjectOfType<MapGenerator> ();
		//placement = FindObjectOfType<PlaceObjectMap> ();
		gm = FindObjectOfType<GameManager> ();
	}

	IEnumerator OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals (targetEndLevel)) {
			if(gm!=null) yield return StartCoroutine(gm.IncrementLevels());
			yield return StartCoroutine(mapGen.GenerateMap());
			//placement.ApplyLevelAjustment();
		}
	}
}
