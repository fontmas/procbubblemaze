/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitApplication : MonoBehaviour {

    public bool activeOnUICancel = false;

    private ProcBubbleMazeActions input;

    private void Awake() {
        input = new ProcBubbleMazeActions();
    }

    private void OnEnable() {
        input.Enable();
        input.UI.Cancel.performed += Quit;
    }

    private void OnDisable() {
        input.Disable();
        input.UI.Cancel.performed -= Quit;
    }

    public void DoQuit() {
		Application.Quit();
	}

    private void Quit(InputAction.CallbackContext ctx) {
        if(activeOnUICancel) DoQuit();
    }
}
