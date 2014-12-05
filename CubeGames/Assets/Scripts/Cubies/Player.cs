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

	[RPC]
	public void SetName(string name)
	{
		PlayerName = name;

		if(networkView.isMine)
		{
			networkView.RPC ("SetName", RPCMode.OthersBuffered, name);
		}
		
	}

	[RPC]
	public void Victory()
	{
		gameObject.transform.position = new Vector3(0,30,0) ;
		gameObject.transform.FindChild ("CubyCamera").gameObject.SetActive (true);
		renderer.enabled = false;
	}

	[RPC]
	public void addScore(string PlayerName,int Points)
	{
		point += Points;
		if(networkView.isMine)
			{
				networkView.RPC ("addScore", RPCMode.OthersBuffered,PlayerName, Points);
			}

	}

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
