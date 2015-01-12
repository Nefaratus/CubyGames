using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {

	public GameObject[] Ports;
	public GameObject poort;
	public Vector3 Offset;
	// Use this for initialization
	void Start () {
		Offset = new Vector3 (15,0,0);
		Ports = GameObject.FindGameObjectsWithTag ("Poort");
		renderer.material.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		do{
			poort = Ports [Random.Range(0,Ports.Length)];
		}while(poort.name == gameObject.name);

		GameObject temp = col.gameObject;
		temp.transform.position = poort.transform.position - Offset;
	}
}
