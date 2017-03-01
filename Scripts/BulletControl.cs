using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : PlayerInput {

	public GameObject bulletPrefab;
	
	Vector2 directionalInput;

	//private List<GameObject> bullets = new List<GameObject>();

	//public float bulletVelocity = 4;

	void Update() {
		

		if( Input.GetButtonDown("Fire1") ) {
			
			Instantiate(bulletPrefab, transform.position, Quaternion.identity);
		}
	}

		public void SetDirectionalInput(Vector2 input) {
		directionalInput = input;
	}
}

