/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;

public class QuitApplication : MonoBehaviour {

	public string InputQuit = "Cancel";

	public void DoQuit() {
		Application.Quit();
	}

	void Update () {
		if (Input.GetButtonDown (InputQuit)) {
			Application.Quit();
		}
	}
}
