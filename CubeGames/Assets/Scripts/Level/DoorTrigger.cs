using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

	public GameObject target;
	public bool active = true;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider col) {
			if (active == true)
			{	
				target.SetActive (false);
				active = false;
			}
						
			else if(active == false)
			{
				target.SetActive (true);
				active = true;
			}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
