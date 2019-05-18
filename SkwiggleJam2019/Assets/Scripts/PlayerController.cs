using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public bool invisible;

// 5 second drain, 15 second refil
	private bool invisCoolDown;
	public float invisPool, invisPoolMax;

// Start is called before the first frame update
	void Start() {
		invisible = invisCoolDown = false;
		invisPool = invisPoolMax = 100.0f;

		StartCoroutine(InvisFill());
		StartCoroutine(InvisDrain());
	}

// Update is called once per frame
	void Update() {

		if (Input.GetKey(KeyCode.LeftShift) && !invisCoolDown && invisPool > 0) {
			invisible = true;
			invisCoolDown = true;

		} else if (invisPool <= 0) {
			invisible = false;

		} else if (invisPool == invisPoolMax && invisCoolDown) {
			invisCoolDown = false;
		}
	}

// @ 1/4 seconds it takes 60 tiks for it to fill up over 15 seconds
	IEnumerator InvisFill() {
		yield return new WaitForSeconds (0.25f);

		if (!invisible && invisPool < invisPoolMax) {
			invisPool += (100f/60f);

			if (invisPool > invisPoolMax) {
				invisPool = invisPoolMax;
			}
		}

		StartCoroutine(InvisFill());
	}

// @ 1/4 seconds it takes 20 tiks for it to drain over 5 seconds
	IEnumerator InvisDrain() {
		yield return new WaitForSeconds (0.25f);

		if (invisible && invisPool > 0) {
			invisPool -= 5.0f;
		}

		StartCoroutine(InvisDrain());
	}
}