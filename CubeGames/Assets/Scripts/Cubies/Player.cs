using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float speed = 10f;
	public int point,totalPoints;	
	public string PlayerName;
	public Vector3 playerColor;
	public GameObject victoryCamera;

	void Start()
	{
		totalPoints = 10;
		playerColor =  new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		SetColor (playerColor);
	}

	void Update()
	{
		if(point >= totalPoints)
		{
			networkView.RPC("Victory",RPCMode.All);
		}

		if (networkView.isMine)
		{
			InputMovement();
			InputColorChange();
		}
		else
		{
		//	SyncedMovement();
		}
	}
	
	void InputMovement()
	{
		if (Input.GetKey(KeyCode.W))
			rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.S))
			rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.D))
			rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.A))
			rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
	}

	private void InputColorChange()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			//Debug.Log("Hallo");
			playerColor =  new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			SetColor (playerColor);

		}

	}


	public int getScore()
	{
		return point;
	}

	/*
	 * Changes the colour of the player's gameObject
	 */ 

	[RPC]
	public void SetColor(Vector3 col)
	{
		renderer.material.color = new Color(col.x,col.y,col.z); //C#
		playerColor = col;
		if(networkView.isMine)
		{
			networkView.RPC ("SetColor", RPCMode.OthersBuffered, col);
		}
	}


	/*
	 * Sets the name of the player within the Game and Network
	 * Also sets the name of the game object so that when creators run the game in the editor they can see which object is who
	 */
	[RPC]
	public void SetName(string name)
	{
		gameObject.name = name;
	}

	/*
	 * When a player is Victorious they will be removed from the game
	 * This methods makes sure that the player gets removed across the server
	 */
	[RPC]
	public void Victory()
	{
		gameObject.transform.position = new Vector3(0,30,0) ;
		gameObject.transform.FindChild ("CubyCamera").gameObject.SetActive (true);
		renderer.enabled = false;
	}


	/*
	 * This adds the score towards the player that has scored and updates it across the network
	 */
	[RPC]
	public void addScore(string PlayerName,int Points)
	{
		point += Points;
		if(networkView.isMine)
			{
				networkView.RPC ("addScore", RPCMode.OthersBuffered,PlayerName, Points);
			}
	}


	/*
	 * Creates a respawn button for when a player wants to respawn 
	 */
	void OnGUI()
	{


		if (GUI.Button (new Rect (Screen.width - 250,Screen.height - 50,100,50), "Respawn"))
		{
			gameObject.transform.position = new Vector3 (0,1,0);
			point = 0;
			if(renderer.enabled == false)
				renderer.enabled = true;
		}
	}
}
