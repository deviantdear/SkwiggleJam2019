using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLock : MonoBehaviour {
	public GameObject nurse;
	private Rigidbody rigid;
	private float lockedX;

// Start is called before the first frame update
	void Start() {
		lockedX = 90f;
		rigid = this.gameObject.GetComponent<Rigidbody>();
	}

// Update is called once per frame
	void Update() {
		this.transform.up = rigid.velocity;
		transform.eulerAngles = new Vector3 (lockedX, transform.eulerAngles.y, transform.eulerAngles.z);
	}
}