using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderInputFieldPlayerPrefs : MonoBehaviour {

	public string PlayerPrefsKey = "Slider";
	public Slider inputFieldValue;

	public void Start() {
		if (inputFieldValue == null) {
			inputFieldValue = GetComponent<Slider> ();
		}
		inputFieldValue.value = PlayerPrefs.GetFloat(PlayerPrefsKey,inputFieldValue.value);
		DoSave();
	}

	public void DoSave() {
		if (inputFieldValue == null)
			return;
		PlayerPrefs.SetFloat(PlayerPrefsKey,inputFieldValue.value);
	}

}
