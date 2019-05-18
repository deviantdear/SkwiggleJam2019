﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
// Meta Variables: Things like game states and menus and the DEADLINE TIMER
	public int deadline;
	public Text timerText;
	public bool paused;
	public GameObject pauseCanvas;
	private GameObject failureCanvas, victoryCanvas;

// Sprite Handling Variables
	private GameObject deathSpriteObj;
	private Animator animate;
	private SpriteRenderer deathSprite;
	private bool up, down, left, right;

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

// Casting variables for Death's ailments
	private bool casting, cast1, cast2, cast3;

// Start is called before the first frame update
	void Start() {
		paused = casting = cast1 = cast2 = cast3 = false;

		pauseCanvas = GameObject.Find("PauseCanvas");
		failureCanvas = GameObject.Find("FailureCanvas");
		victoryCanvas = GameObject.Find("VictoryCanvas");

		pauseCanvas.SetActive(false);
		failureCanvas.SetActive(false);
		victoryCanvas.SetActive(false);

		up = down = left = right = false;

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

		if (deadline%60 > 9) {
			timerText.text = deadline/60 + ":" + deadline%60;

		} else {
			timerText.text = deadline/60 + ":0" + deadline%60;
		}

		StartCoroutine(InvisHandler());

		StartCoroutine(DeadlineCounter());
	}

// Update is called once per frame
	void Update() {
	// Pause Button
		if (Input.GetKeyDown(KeyCode.Escape)) {
			paused = !paused;
			pauseCanvas.SetActive(paused);

			if (paused) {
				Time.timeScale = 0.0f;
			} else {
				Time.timeScale = 1.0f;
			}
		}

	// Controls
		if (!paused && !casting) {
		// Movement stuff
			if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
			// Forward
				vSpeed = moveSpeed;

				if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
					up = true;
					down = false;
					left = false;
					right = false;
				}

			} else if (!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) {
			// Backward
				vSpeed = -moveSpeed;

				if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
					up = false;
					down = true;
					left = false;
					right = false;
				}

			} else {
				vSpeed = 0.0f;
			}

			if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
			// Forward
				hSpeed = moveSpeed;

				if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
					up = false;
					down = false;
					left = false;
					right = true;
				}

			} else if (!Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) {
			// Backward
				hSpeed = -moveSpeed;

				if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
					up = false;
					down = false;
					left = true;
					right = false;
				}

			} else {
				hSpeed = 0.0f;
			}

			var toMove = this.transform.forward * vSpeed + this.transform.right * hSpeed;

			if (vSpeed != 0.0f && hSpeed != 0.0f) {
				toMove *= Mathf.Sqrt(2)/2;
			}

			transform.Translate(toMove * Time.deltaTime);

		// Animate player moving and idle states
			if ((vSpeed != 0.0f || hSpeed != 0.0f) && !invisible) {
				if (up && animate.GetInteger("PlayerState") != 1) {
					animate.SetInteger("PlayerState", 1);

				} else if (down && animate.GetInteger("PlayerState") != 2) {
					animate.SetInteger("PlayerState", 2);

				} else if (left && animate.GetInteger("PlayerState") != 3) {
					animate.SetInteger("PlayerState", 3);

				} else if (right && animate.GetInteger("PlayerState") != 4) {
					animate.SetInteger("PlayerState", 4);
				}
			} else if ((vSpeed != 0.0f || hSpeed != 0.0f) && invisible) {
				if (up && animate.GetInteger("PlayerState") != 5) {
					animate.SetInteger("PlayerState", 5);

				} else if (down && animate.GetInteger("PlayerState") != 6) {
					animate.SetInteger("PlayerState", 6);

				} else if (left && animate.GetInteger("PlayerState") != 7) {
					animate.SetInteger("PlayerState", 7);

				} else if (right && animate.GetInteger("PlayerState") != 8) {
					animate.SetInteger("PlayerState", 8);
				}
			} else {
				if (up && animate.GetInteger("PlayerState") != 14) {
					animate.SetInteger("PlayerState", 14);

				} else if (down && animate.GetInteger("PlayerState") != 0) {
					animate.SetInteger("PlayerState", 0);

				} else if (left && animate.GetInteger("PlayerState") != 15) {
					animate.SetInteger("PlayerState", 15);

				} else if (right && animate.GetInteger("PlayerState") != 16) {
					animate.SetInteger("PlayerState", 16);
				}
			}

		// Ability Casting
			if ((Input.GetKey(KeyCode.Alpha1) && !cast1) || (Input.GetKey(KeyCode.Alpha2) && !cast2) || (Input.GetKey(KeyCode.Alpha3) && !cast3)) {
				casting = true;
				if (Input.GetKey(KeyCode.Alpha1) && !cast1) {
					

				} else if (Input.GetKey(KeyCode.Alpha2) && !cast2) {


				} else if (Input.GetKey(KeyCode.Alpha3) && !cast3) {


				}

				if (up && animate.GetInteger("PlayerState") != 9) {
					animate.SetInteger("PlayerState", 9);

				} else if (down && animate.GetInteger("PlayerState") != 10) {
					animate.SetInteger("PlayerState", 10);

				} else if (left && animate.GetInteger("PlayerState") != 11) {
					animate.SetInteger("PlayerState", 11);

				} else if (right && animate.GetInteger("PlayerState") != 12) {
					animate.SetInteger("PlayerState", 12);
				}
			}

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

	IEnumerator AbilityCooldowns() {
		yield return new WaitForSeconds (0.25f);



		StartCoroutine(AbilityCooldowns());
	}

	IEnumerator DeadlineCounter() {
		yield return new WaitForSeconds (1.0f);

		if (deadline > 0) {
			if (deadline%60 > 9) {
				timerText.text = deadline/60 + ":" + deadline%60;
			
			} else {
				timerText.text = deadline/60 + ":0" + deadline%60;
			}
			deadline--;

		} else {
			Time.timeScale = 0.0f;
			failureCanvas.SetActive(true);
		}

		if (deadline > 0) {
			StartCoroutine(DeadlineCounter());
		}
	}
}