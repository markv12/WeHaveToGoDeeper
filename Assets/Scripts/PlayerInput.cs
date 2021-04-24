using UnityEngine;

[RequireComponent(typeof(OldPlayer))]
public class PlayerInput : MonoBehaviour {

	OldPlayer player;

	void Start() {
		player = GetComponent<OldPlayer>();
	}

	void Update() {
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);

		if (Input.GetKeyDown(KeyCode.Space)) {
			player.OnJumpInputDown();
		}
		if (Input.GetKeyUp(KeyCode.Space)) {
			player.OnJumpInputUp();
		}
	}
}
