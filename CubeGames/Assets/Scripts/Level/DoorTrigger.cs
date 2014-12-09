using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

	public GameObject target;
	private bool active = true;

	/*
	 * When a Gameobject enters this trigger the target of the script will be deactivated. 
	 * The target is the door where the collider is placed.
	 * The collider will function as a trigger
	 */
	void OnTriggerEnter(Collider col)
	{
		target.SetActive (false);
	}

	/*
	 * When the player leaves the collider/trigger the target will be activated again.
	 */ 
	void OnTriggerExit(Collider col) 
	{
		target.SetActive (true);
	}
}
