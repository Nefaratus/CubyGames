using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	public Transform target;

	/*
	 * Sets the transform of this object to the same of his target.
	 */
	void FixedUpdate () {
		transform.position = new Vector3 (target.position.x, transform.position.y, target.position.z);
	}
}
