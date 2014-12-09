using UnityEngine;
using System;
using System.Collections;

public class Objective : MonoBehaviour {
	public int height;
	public Player player;
	public GameObject[] playerList;
	public string PlayerName;
	public int PlayerScore;
	public Color color;
	public ParticleSystem ps;

	// Use this for initialization
	void Start () {
		renderer.material.color = new Color (1, 0, 0);
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	/*
	 *When the player collides with an Objective add a point to the score of this player and explode using that player his color.
	 *After the particle is done destroy this object so a new one can spawn. The spawning of a new Objective is monitored in the NetworkManager.
	 */
	void OnTriggerEnter(Collider col)
	{
		player = (Player)col.gameObject.GetComponent ("Player");
		player.addScore (player.PlayerName,1);
		height += 10;
		networkView.RPC("Explosion",RPCMode.All, new object[]{player.playerColor, 325 });
		Destroy (gameObject,ps.duration);

	}

	/*
	 * Activates the Explosion for all players that are in the game.
	 */
	[RPC]
	void Explosion(Vector3 col,int count)
	{
		particleSystem.startColor = new Color (col.x,col.y,col.z);
		particleSystem.Emit (count);
		gameObject.renderer.enabled = false;
		gameObject.collider.enabled = false;
	}

	/*
	 *Create a basic UI for the score list where in all player names are noted and their scores behind it.		 *
	 */ 
	void OnGUI()
	{

		try{
		playerList = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject p in playerList) {
			PlayerName = p.gameObject.name;
			PlayerScore = p.GetComponent<Player> ().point;
			GUI.Label(new Rect (10, height, 100, 20), PlayerName + " : " + PlayerScore,"box");
			height+=25;

		}
		height = 20;
		}catch(Exception  e)
		{}
	}

}
