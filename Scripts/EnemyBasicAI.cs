using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicAI : MonoBehaviour {

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public float waitTime;
	[Range(0, 5)]
	public float easeAmount;

	int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;

	private void Start() {
		globalWaypoints = new Vector3[localWaypoints.Length];
		for( int i = 0 ; i < localWaypoints.Length ; i++ ) {
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	private void Update() {
		Vector3 velocity = CalculateEnemyMovement();

		transform.Translate(velocity);
	}

	Vector3 CalculateEnemyMovement() {
		if( Time.time < nextMoveTime ) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = ( fromWaypointIndex + 1 ) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
		float easedPercentBetweenWayPoints = Ease(percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWayPoints);

		if( percentBetweenWaypoints >= 1 ) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;

			
			if( fromWaypointIndex >= globalWaypoints.Length - 1 ) {
				fromWaypointIndex = 0;
				System.Array.Reverse(globalWaypoints);
			}
			
			nextMoveTime = Time.time + waitTime;
		}
		return newPos - transform.position;
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / ( Mathf.Pow(x, a) + Mathf.Pow(1 - x, a) );
	}


	void OnDrawGizmos() {
		if( localWaypoints != null ) {
			Gizmos.color = Color.red;
			float size = .3f;
			for( int i = 0 ; i < localWaypoints.Length ; i++ ) {
				Vector3 globalWayPointsPos = ( Application.isPlaying ) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWayPointsPos - Vector3.up * size, globalWayPointsPos + Vector3.up * size);
				Gizmos.DrawLine(globalWayPointsPos - Vector3.left * size, globalWayPointsPos + Vector3.left * size);
			}
		}
	}
}
