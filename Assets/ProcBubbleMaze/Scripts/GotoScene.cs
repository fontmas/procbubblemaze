using UnityEngine;
using System.Collections;

public class GotoScene : MonoBehaviour {

	public bool activeByInput = false;
	public string InputName = "Cancel";
	public string targetScene = "Main";

	void Update() {
		if (activeByInput) {
			if(Input.GetButtonDown(InputName)){
				DoGotoScene(targetScene);
			}
		}
	}

	public void DoGotoScene (string Scene) {
		UnityEngine.SceneManagement.SceneManager.LoadScene(Scene);
		//Application.LoadLevel (Scene);
	}
}
