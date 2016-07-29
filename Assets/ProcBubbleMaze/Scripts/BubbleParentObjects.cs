using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleParentObjects : MonoBehaviour {

	public string InputBubble = "Jump";
	public int numberChilds = 1;

	void OnTriggerEnter2D(Collider2D other) {
		for (int cChild = 0; cChild < transform.childCount; cChild++) {
			if(transform.GetChild(cChild).GetComponent<Rigidbody2D>() != null && transform.GetChild(cChild).GetComponent<Rigidbody2D>().IsTouching(gameObject.GetComponent<Collider2D>())) {
				transform.GetChild(cChild).transform.parent = null;
			}
		}
		if (transform.childCount < numberChilds && Input.GetButton (InputBubble)) {
			other.gameObject.transform.parent = transform;
		}
	}
	
	void Update() {
		if (transform.childCount >= numberChilds && Input.GetButtonUp (InputBubble)) {
			for (int cChild = 0; cChild < transform.childCount; cChild++) {
				transform.GetChild(cChild).transform.parent = transform.parent.transform.parent;
			}
		}
	}
	
}