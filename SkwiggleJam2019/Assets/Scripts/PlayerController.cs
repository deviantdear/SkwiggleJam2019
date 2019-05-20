using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
// Meta Variables: Things like game states and menus and the DEADLINE TIMER
	public int deadline;
	public Text timerText;
	public bool paused;
	public float viewDistance;
	public GameObject pauseCanvas;
	private GameObject failureCanvas, victoryCanvas;
	public AudioClip sound1, sound2, sound3;
	private AudioSource speaker;

// Sprite Handling Variables
	private GameObject deathSpriteObj;
	private Animator animate;
	private SpriteRenderer deathSprite;
	private bool up, down, left, right;
    private SpriteRenderer spriteRenderer;

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
	public Image cast1, cast2, cast3;
	private bool casting;
	private float coolDownMax, coolDown1, coolDown2, coolDown3, coolDownAlpha;
	private RaycastHit seen;
	private Vector3 facingDir;

// Start is called before the first frame update
	void Start() {
		speaker = this.GetComponent<AudioSource>();
		paused = casting = false;
		coolDown1 = coolDown2 = coolDown3 = coolDownMax = 100f;
		coolDownAlpha = 0.25f;

		pauseCanvas = GameObject.Find("PauseCanvas");
		failureCanvas = GameObject.Find("FailureCanvas");
		victoryCanvas = GameObject.Find("VictoryCanvas");

		pauseCanvas.SetActive(false);
		failureCanvas.SetActive(false);
		victoryCanvas.SetActive(false);

		up = left = right = false;
		down = true;
		Vector3 facingDir = -Vector3.forward;

		deathSpriteObj = GameObject.Find("DeathSprite");
		animate = deathSpriteObj.GetComponent<Animator>();
		deathSprite = deathSpriteObj.GetComponent<SpriteRenderer>();
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();


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

		Color temp = cast1.color;
		temp.a = coolDownAlpha;
		cast1.color = temp;

		temp = cast2.color;
		temp.a = coolDownAlpha;
		cast2.color = temp;

		temp = cast3.color;
		temp.a = coolDownAlpha;
		cast3.color = temp;

		StartCoroutine(InvisHandler());
		StartCoroutine(AbilityCooldowns());
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

	// Player Input/Controls
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
                    spriteRenderer.flipX = false; //toggles sprite left and right
                }

            } else if (!Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) {
			// Backward
				hSpeed = -moveSpeed;

				if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
					up = false;
					down = false;
					left = true;
					right = false;
                    spriteRenderer.flipX = true; //toggles sprite left and right
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

		// Ability RAYCasting hehe get it? casting but with a raycast!
			if (up) {
				facingDir = Vector3.forward;
			} else if (down) {
				facingDir = -Vector3.forward;
			} else if (left) {
				facingDir = -Vector3.right;
			} else if (right) {
				facingDir = Vector3.right;
			}

			Debug.DrawRay(this.transform.position, this.transform.TransformDirection(facingDir) * viewDistance, Color.white);

			if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(facingDir), out seen, viewDistance)) {
				GameObject seenObj = seen.collider.gameObject;

				if (seenObj.CompareTag("Patient")) {
					Patient patientScript = seenObj.GetComponent<Patient>();

					if (patientScript != null) {
						if (patientScript.illness == Illness.None) {
							if ((Input.GetKey(KeyCode.Alpha1) && coolDown1 == coolDownMax) || (Input.GetKey(KeyCode.Alpha2) && coolDown2 == coolDownMax) || (Input.GetKey(KeyCode.Alpha3) && coolDown3 == coolDownMax)) {
								casting = true;

								if (Input.GetKey(KeyCode.Alpha1) && coolDown1 == coolDownMax) {
									coolDown1 = 0f;
									Color temp = cast1.color;
									temp.a = coolDownAlpha;
									cast1.color = temp;
									speaker.PlayOneShot(sound1, 1.0f);
									patientScript.illness = Illness.Infection;

								} else if (Input.GetKey(KeyCode.Alpha2) && coolDown2 == coolDownMax) {
									coolDown2 = 0f;
									Color temp = cast2.color;
									temp.a = coolDownAlpha;
									cast2.color = temp;
									speaker.PlayOneShot(sound2, 1.0f);
									patientScript.illness = Illness.HeartAttack;

								} else if (Input.GetKey(KeyCode.Alpha3) && coolDown3 == coolDownMax) {
									coolDown3 = 0f;
									Color temp = cast3.color;
									temp.a = coolDownAlpha;
									cast3.color = temp;
									speaker.PlayOneShot(sound3, 1.0f);
									patientScript.illness = Illness.Stroke;
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

								StartCoroutine(AbilityUseCooldown());
							}
						}
					}
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

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("NurseSight")) {
			Transform t = other.gameObject.transform;

			while (t.parent != null) {
				if (t.parent.tag == "Nurse") {
					Nurse nurseScript = t.parent.GetComponent<Nurse>();
					nurseScript.fear = true;
				}

				t = t.parent.transform;
			}
		}
	}

// Ability refresher
	IEnumerator AbilityCooldowns() {
		yield return new WaitForSeconds (1.0f);

		if (coolDown1 < coolDownMax) {
			coolDown1 += coolDownMax/5f;

			if (coolDown1 > coolDownMax) {
				coolDown1 = coolDownMax;
			}
		}

		if (coolDown1 == coolDownMax) {
			Color temp = cast1.color;
			temp.a = 1.0f;
			cast1.color = temp;
		}

		if (coolDown2 < coolDownMax) {
			coolDown2 += coolDownMax/5f;

			if (coolDown2 > coolDownMax) {
				coolDown2 = coolDownMax;
			}
		}

		if (coolDown2 == coolDownMax) {
			Color temp = cast2.color;
			temp.a = 1.0f;
			cast2.color = temp;
		}

		if (coolDown3 < coolDownMax) {
			coolDown3 += coolDownMax/5f;

			if (coolDown3 > coolDownMax) {
				coolDown3 = coolDownMax;
			}
		}

		if (coolDown3 == coolDownMax) {

			Color temp = cast3.color;
			temp.a = 1.0f;
			cast3.color = temp;
		}

		StartCoroutine(AbilityCooldowns());
	}

	IEnumerator AbilityUseCooldown() {
		yield return new WaitForSeconds (0.5f);

		if (casting) {
			casting = false;
		}
	}

	IEnumerator DeadlineCounter() {
		yield return new WaitForSeconds (1.0f);

		if (deadline > 0) {
			if (deadline%60 > 9) {
				timerText.text = deadline/60 + ":" + deadline%60;
			
			} else {
				timerText.text = deadline/60 + ":0" + deadline%60;
			}

		} else {
			timerText.text = "0:00";
			Time.timeScale = 0.0f;
			failureCanvas.SetActive(true);
		}

		deadline--;
		if (deadline >= 0) {
			StartCoroutine(DeadlineCounter());
		}
	}
}