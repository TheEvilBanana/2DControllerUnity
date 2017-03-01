using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	
	public float timeToJumpApex = 0.4f;
	float accelerationTimeAirborne = 0.2f;
	float accelerationTimeGrounded = 0.1f;
	float moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = 0.25f;
	public float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
	//Vector3 newScale;
	public GameObject bulletSpawner;
	public GameObject bulletPreFab;
	Animator playerAnim;
    Controller2D controller;
	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;
    void Start() {
		controller = GetComponent<Controller2D>();
		playerAnim = GetComponent<Animator>();
		gravity = -( 2 * maxJumpHeight ) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		
		
    }

	private void Update() {

		CalculateVelocity();
		HandleWallSliding();

		controller.Move(velocity * Time.deltaTime, directionalInput);

		if(directionalInput.x > 0 ) {
			GetComponent<SpriteRenderer>().flipX = false;
			bulletSpawner.transform.localPosition = new Vector3(0.9f, 0f, 0f);
			bulletPreFab.GetComponent<BulletDestroy>().bulletDirectionX = directionalInput.x;
			bulletPreFab.GetComponent<CircleCollider2D>().offset = new Vector2(0.35f, 0.0f);
			bulletPreFab.GetComponent<SpriteRenderer>().flipX = false;
			int i = 1;
			playerAnim.SetInteger("walking", i);
		}
		if( directionalInput.x < 0 ) {
			GetComponent<SpriteRenderer>().flipX = true;
			bulletSpawner.transform.localPosition = new Vector3(-1.0f, 0f, 0f);
			bulletPreFab.GetComponent<BulletDestroy>().bulletDirectionX = directionalInput.x;
			bulletPreFab.GetComponent<CircleCollider2D>().offset = new Vector2(-0.35f, 0.0f);
			bulletPreFab.GetComponent<SpriteRenderer>().flipX = true;
			int i = 1;
			playerAnim.SetInteger("walking", i);
		}
		if(directionalInput.x == 0 ) {
			int i = 0;
			playerAnim.SetInteger("walking", i);
		}

		if( controller.collisions.above || controller.collisions.below ) {
			if( controller.collisions.slidingDownMaxSlope ) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			}
			else {
				velocity.y = 0;
			}
		}
	}

	public void SetDirectionalInput(Vector2 input) {
		directionalInput = input;
	}

	public void OnJumpInputDown() {
		if( wallSliding ) {
			if( wallDirX == directionalInput.x ) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if( directionalInput.x == 0 ) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = -wallJumpOff.y;
			}
			else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}

		if( controller.collisions.below ) {
			if( controller.collisions.slidingDownMaxSlope ) {
				if( directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x) ) { // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}
			else {
				velocity.y = maxJumpVelocity;
			}
		} 
	}

	public void OnJumpInputUp() {
		if( velocity.y > minJumpVelocity ) {
			velocity.y = minJumpVelocity;
		}
	}

	void HandleWallSliding() {
		wallDirX = ( controller.collisions.left ) ? -1 : 1;
		wallSliding = false;

		if( ( controller.collisions.left || controller.collisions.right ) && !controller.collisions.below && velocity.y < 0 ) {
			wallSliding = true;

			if( velocity.y < -wallSlideSpeedMax ) {
				velocity.y = -wallSlideSpeedMax;
			}

			if( timeToWallUnstick > 0 ) {

				velocityXSmoothing = 0;
				velocity.x = 0;

				if( directionalInput.x != wallDirX && directionalInput.x != 0 ) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}
		}
	}
	void CalculateVelocity() {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, ( controller.collisions.below ) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}
}
