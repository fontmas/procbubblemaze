using UnityEngine;
using System.Collections;

public class GotoScene : MonoBehaviour {

	public void DoGotoScene (string Scene) {
		UnityEngine.SceneManagement.SceneManager.LoadScene(Scene);
	}
}
