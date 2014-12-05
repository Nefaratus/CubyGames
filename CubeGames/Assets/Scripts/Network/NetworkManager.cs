using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	private const string gameName = "CubeWars";
	private const string roomName = "CubiesPlayGround_V1";
	private HostData[] hostList;
	public GameObject playerPrefab,standbyCamera,Objective;
	public string name,currentMessage;
	GameObject[] points;	

	void Start()
	{
		points = GameObject.FindGameObjectsWithTag("Respawn");
	}

	private void StartServer()
	{
		Network.InitializeServer(10, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, roomName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		SpawnPlayer();
		SpawnObjective ();
	}

	private void RefreshHostList()
	{
		MasterServer.RequestHostList(gameName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		SpawnPlayer();
	}


	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	private void SpawnPlayer()
	{
		GameObject myPlayer = (GameObject)Network.Instantiate (playerPrefab, new Vector3 (0,1,0), Quaternion.identity, 0);
		standbyCamera.SetActive (false);
		
		//Setting on the objects of the player so that only the player has them in his client
		myPlayer.GetComponent<Player>().enabled = true;		
		myPlayer.transform.FindChild ("CubyCamera").gameObject.SetActive (true);
		Debug.Log ("All Active");
	}

	void SpawnObjective()
	{
		foreach (GameObject item in points) {
			//GameObject objPoint = points [Random.Range (0, points.Length)];
			//Debug.Log (item.ToString());
			Network.Instantiate (Objective, item.transform.position, Quaternion.identity,0);
				}


	}
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}

		name = GUI.TextField(new Rect (10, Screen.height-50, 100, 20), name);

		if(GUI.Button(new Rect(120, Screen.height-50, 100, 20),"Send"))
		{
			SetPlayerName();
		}

		if (GUI.Button (new Rect (Screen.width - 100,Screen.height - 50,100,50), "Exit"))
		{
			Application.Quit();
		}



		
	}

	void SetPlayerName()
	{
		if(!string.IsNullOrEmpty(name.Trim()))
		{
			GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetName(name);
			name = string.Empty;
		}
	}
}
