/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;

public class Player2D : MonoBehaviour {

	public float speed = 5.0f;

	Rigidbody2D rigidbodyPlayer;
	Vector2 velocity;
	private Utils.Map.MapGenerator mapGen = null;

    private ProcBubbleMazeActions input;

    private void Awake() {
        input = new ProcBubbleMazeActions();
    }

    private void Start() {
        mapGen = FindObjectOfType<Utils.Map.MapGenerator>();
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        input.Enable();
        input.Player.Move.performed += MovePlayer;
        input.Player.Move.canceled += StopPlayer;
    }


    private void OnDisable() {
        input.Disable();
        input.Player.Move.performed -= MovePlayer;
        input.Player.Move.canceled -= StopPlayer;
    }

    private void StopPlayer(InputAction.CallbackContext obj) {
        velocity = Vector2.zero;
    }

    private void MovePlayer(InputAction.CallbackContext obj) {
        velocity = ((Vector2)obj.ReadValueAsObject()).normalized * (speed * (mapGen == null ? 1 : mapGen.tileScale));
    }

	private void FixedUpdate() {
        //rigidbodyPlayer.MovePosition(Vector3.Lerp( rigidbodyPlayer.position,  rigidbodyPlayer.position + velocity, Time.fixedDeltaTime));
        rigidbodyPlayer.MovePosition(rigidbodyPlayer.position + velocity * Time.fixedDeltaTime);
	}
}