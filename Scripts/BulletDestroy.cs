using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : MonoBehaviour {

	public float bulletVelocity = 4;
	Animator bulletAnim;
	public float bulletDirectionX;
	
	void Update () {

		transform.Translate(new Vector3(bulletDirectionX, 0) * Time.deltaTime * bulletVelocity);
	}

	private void OnBecameInvisible() {
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Destroy(gameObject);
		if(collision.tag == "Enemy" ) {
			Destroy(collision.gameObject);
		}
	}
}

