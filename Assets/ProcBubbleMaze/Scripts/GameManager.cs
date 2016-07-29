/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Utils.Map;

public class GameManager : MonoBehaviour {

	public int playerNumberLifes = 3;
	public int extraLifeLevels = 5;
	public Text levelsGUI;
	public Text lifesGUI;
	public Text TotalHitsGUI;
	public Text TotalLifesGainedGUI;
	public GameObject GameOverGUI;
	public GameObject ExtraLifeGUI;
	private int countLevels = 1;
	private int countHits = 0;
	private int countLifeGained = 0;
	private MapGenerator mapGen;

	void Start () {
		mapGen = FindObjectOfType<MapGenerator> ();
		if(GameOverGUI !=null) GameOverGUI.SetActive (false);
		if (ExtraLifeGUI != null) ExtraLifeGUI.SetActive(false);
	}

	public void IncrementHits () {
		countHits++;
	}

	public IEnumerator IncrementLevels () {
		countLevels++;
		if (countLevels % extraLifeLevels == 0) {
			ExtraLifeGUI.SetActive(true);
			yield return new WaitForSeconds(1.0f);
			playerNumberLifes++;
			countLifeGained++;
			ExtraLifeGUI.SetActive(false);
		}
		UpdateMenuGUI();
	}

	public void DecrementLife () {
		playerNumberLifes--;
		UpdateMenuGUI ();
	}

	void UpdateMenuGUI () {
		lifesGUI.text = "X " + playerNumberLifes;
		levelsGUI.text = "Level: " + countLevels;
		if (playerNumberLifes <= 0) {
			if(mapGen !=null) Destroy(mapGen.gameObject);
			if(TotalHitsGUI !=null) TotalHitsGUI.text += countHits;
			if(TotalLifesGainedGUI !=null) TotalLifesGainedGUI.text += countLifeGained;
			if(GameOverGUI !=null) GameOverGUI.SetActive(true);
		}
	}
}