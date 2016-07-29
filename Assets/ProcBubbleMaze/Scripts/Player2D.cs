/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;

public class Player2D : MonoBehaviour {

	public float speed = 10.0f;
	public string InputHorizontal = "Horizontal";
	public string InputVertical = "Vertical";

	Rigidbody2D rigidbodyPlayer;
	Vector2 velocity;
	private Utils.Map.MapGenerator mapGen = null;

	void Start () {
		mapGen = GameObject.FindObjectOfType<Utils.Map.MapGenerator> ();
		rigidbodyPlayer = GetComponent<Rigidbody2D> ();
	}
	
	void Update () {
		velocity = new Vector2 (Input.GetAxisRaw (InputHorizontal), Input.GetAxisRaw (InputVertical)).normalized * (speed*(mapGen==null?1:mapGen.tileScale));
	}
	
	void FixedUpdate() {
		rigidbodyPlayer.MovePosition(Vector3.Lerp( rigidbodyPlayer.position,  rigidbodyPlayer.position + velocity, Time.fixedDeltaTime));
	}

}