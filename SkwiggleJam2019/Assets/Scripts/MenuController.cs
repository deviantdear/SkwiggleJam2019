using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	private int nextScene;
	private GameObject player;
	private PlayerController playerScript;

// Start is called before the first frame update
	void Start() {
		nextScene = SceneManager.GetActiveScene().buildIndex + 1;

		if (nextScene > 5) { // Change "5" to whatever the max number of scenes is - 1
			nextScene = 0;
		}

		player = GameObject.FindGameObjectWithTag("Player");

		if (player != null) {
			playerScript = player.GetComponent<PlayerController>();
		}
	}

// Custom menu button functions
	public void StartGame() {
		SceneManager.LoadScene(1);
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void Resume() {
		playerScript.paused = false;
		Time.timeScale = 1.0f;
		playerScript.pauseCanvas.SetActive(false);
	}

	public void ExitToTitle() {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(0);
	}

	public void Retry() {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void NextLevel() {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(nextScene);
	}
}