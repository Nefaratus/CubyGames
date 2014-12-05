using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {
	public int height;
	public Player player;
	public GameObject[] coolList;
	public string PlayerName;
	public int PlayerScore;
	public string PlayerNewas;
	public Color color;

	// Use this for initialization
	void Start () {
		renderer.material.color = new Color (1, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider col)
	{
		player = (Player)col.gameObject.GetComponent ("Player");
		player.addScore (player.PlayerName,1);
		height += 10;
		networkView.RPC("Explosion",RPCMode.AllBuffered, new object[]{player.playerColor, 325 });
		//Destroy (gameObject);

	}


	[RPC]
	void Explosion(Vector3 col,int count)
	{
		particleSystem.startColor = new Color (col.x,col.y,col.z);
		particleSystem.Emit (count);
		gameObject.renderer.enabled = false;
		gameObject.collider.enabled = false;
	}


	void OnGUI()
	{
		coolList = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject p in coolList) {
			PlayerName = p.GetComponent<Player> ().PlayerName;
			PlayerScore = p.GetComponent<Player> ().point;
			GUI.Label(new Rect (10, height, 100, 20), PlayerName + " : " + PlayerScore,"box");
			height+=25;

		}
		height = 20;

	}

}
