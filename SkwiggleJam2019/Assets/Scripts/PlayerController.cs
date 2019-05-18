using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public bool paused;

// Used for invisibility 5 second drain, 15 second refil
	public bool invisible;
	private bool invisCoolDown;
	private float invisPool, invisPoolMax;

// Used for movement
	private float vSpeed, hSpeed, moveSpeed;

// Start is called before the first frame update
	void Start() {
		paused = false;

		invisible = invisCoolDown = false;
		invisPool = invisPoolMax = 100.0f;

		moveSpeed = 2.5f;
		hSpeed = vSpeed = 0.0f;

		StartCoroutine(InvisHandler());
	}

// Update is called once per frame
	void Update() {

	// Controls
		if (!paused) {
		// Movement stuff
			if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
			// Forward
				vSpeed = moveSpeed;

			} else if (!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) {
			// Backward
				vSpeed = -moveSpeed;

			} else {
				vSpeed = 0.0f;
			}

			if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
			// Forward
				hSpeed = moveSpeed;

			} else if (!Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) {
			// Backward
				hSpeed = -moveSpeed;

			} else {
				hSpeed = 0.0f;
			}

			var toMove = this.transform.forward * vSpeed + this.transform.right * hSpeed;

			if (vSpeed != 0.0f && hSpeed != 0.0f) {
				toMove *= Mathf.Sqrt(2)/2;
			}

			transform.Translate(toMove * Time.deltaTime);

		// Invisibility input handler
			if (Input.GetKey(KeyCode.LeftShift) && !invisCoolDown && invisPool > 0) {
				invisible = true;
				invisCoolDown = true;

			} else if (invisPool <= 0) {
				invisible = false;

			} else if (invisPool == invisPoolMax && invisCoolDown) {
				invisCoolDown = false;
			}
		}
	}

	IEnumerator InvisHandler() {
		yield return new WaitForSeconds (0.25f);

		if (!invisible && invisPool < invisPoolMax) {
		// @ 1/4 seconds it takes 60 tiks for it to fill up over 15 seconds
			invisPool += (100f/60f);

			if (invisPool > invisPoolMax) {
				invisPool = invisPoolMax;
			}
		} else if (invisible && invisPool > 0) {
		// @ 1/4 seconds it takes 20 tiks for it to drain over 5 seconds
			invisPool -= 5.0f;
		}

		StartCoroutine(InvisHandler());
	}
}