using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public bool paused;

// Sprite Handling Variables
	private GameObject deathSpriteObj;
	private Animator animate;
	private SpriteRenderer deathSprite;

// Used for invisibility things; 5 second drain, 15 second refil
	public bool invisible;
	public Image stealthGauge;
	public float invisAlpha;
	private bool invisCoolDown;
	private RectTransform stealthTransform;
	private float invisPool, invisPoolMax, minGaugeX, maxGaugeX, gaugeY;

// Used for movement
	public float baseSpeed, invisSpeed;
	private float moveSpeed, vSpeed, hSpeed;

// Start is called before the first frame update
	void Start() {
		paused = false;

		deathSpriteObj = GameObject.Find("DeathSprite");
		animate = deathSpriteObj.GetComponent<Animator>();
		deathSprite = deathSpriteObj.GetComponent<SpriteRenderer>();

		invisible = invisCoolDown = false;
		invisPool = invisPoolMax = 100.0f;

		stealthTransform = stealthGauge.GetComponent<RectTransform>();

		gaugeY = stealthTransform.position.y;
		minGaugeX = stealthTransform.position.x;
		maxGaugeX = stealthTransform.position.x - stealthTransform.rect.width;

		moveSpeed = baseSpeed;
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

		// Ability Casting


		// Invisibility input handler
			if (Input.GetKey(KeyCode.LeftShift) && !invisCoolDown && invisPool > 0) {
				invisible = true;
				moveSpeed = invisSpeed;
				invisCoolDown = true;

				Color temp = deathSprite.color;
				temp.a = invisAlpha;
				deathSprite.color = temp;

			} else if (invisPool <= 0) {
				invisible = false;
				moveSpeed = baseSpeed;

				Color temp = deathSprite.color;
				temp.a = 1.0f;
				deathSprite.color = temp;

			} else if (invisPool == invisPoolMax && invisCoolDown) {
				invisCoolDown = false;
			}

		// Update Invisibility Gauge Visually
			var newPosX = (-invisPool) * (maxGaugeX - minGaugeX) / (invisPoolMax) + minGaugeX;
			Vector3 newGaugeX = new Vector3(newPosX, gaugeY);

			stealthGauge.transform.position = newGaugeX;
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