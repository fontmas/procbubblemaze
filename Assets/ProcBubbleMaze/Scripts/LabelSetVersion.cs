using UnityEngine;
using UnityEngine.UI;

public class LabelSetVersion : MonoBehaviour {

    public Text text;

    void Start() {
        text.text = Application.version;
    }
}
