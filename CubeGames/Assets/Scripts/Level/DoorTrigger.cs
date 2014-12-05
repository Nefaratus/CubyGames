using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

	public GameObject target;
	public bool active = true;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		target.SetActive (false);
	}

	void OnTriggerExit(Collider col) 
	{
		target.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
