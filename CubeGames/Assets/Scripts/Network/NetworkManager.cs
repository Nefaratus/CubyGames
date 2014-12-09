using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	private const string gameName = "CubeWars";
	private const string roomName = "CubiesPlayGround_V1";
	private HostData[] hostList;
	public GameObject playerPrefab,standbyCamera,Objective;
	public string name,currentMessage;
	GameObject[] points, totalObjective;
	bool joinedServer, nameSet;


	/*
	 * Find all spawn points within the map 
	 */
	void Start()
	{
		points = GameObject.FindGameObjectsWithTag("Respawn");
	}

	/*
	 * Keeps checking if the total amount of objectives in the game is smaller than five.
	 */
	void Update()
	{
		totalObjective = GameObject.FindGameObjectsWithTag ("Objective");
		if(joinedServer == true)
		{
			if(totalObjective.Length < 5)
		   		SpawnObjective();
		}
	}

	/*
	 * Automatic function of Unity Networking on what to do when creating a server
	 */
	private void StartServer()
	{
		Network.InitializeServer(10, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, roomName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");		
		joinedServer = true;
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

	/*
	 * When the user tries to join a server connect to the server by using the HostData of that server
	 */
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	/*
	 * Sets joined server on true when the user is connected to the server.
	 */
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		joinedServer = true;
	}

	/*
	 * Cleaning up the mess when a person leaves the game.
	 */
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	/*
	 * Spawning the player and activating the scripts that only he needs in his client.
	 * Deactivating the standbye camera since its not needed anymore in this client since the player uses his own camera that follows him.
	 */
	private void SpawnPlayer(string playerName)
	{
		GameObject myPlayer = (GameObject)Network.Instantiate (playerPrefab, new Vector3 (0,1,0), Quaternion.identity, 0);
		standbyCamera.SetActive (false);
		myPlayer.networkView.RPC ("SetName", RPCMode.AllBuffered, name);

		//Setting on the objects of the player so that only the player has them in his client
		myPlayer.GetComponent<Player>().enabled = true;		
		myPlayer.transform.FindChild ("CubyCamera").gameObject.SetActive (true);
		Debug.Log ("All Active");
	}

	/*
	 * Spawn one Objective over the network.
	 */
	void SpawnObjective()
	{
		GameObject objPoint = points [Random.Range (0, points.Length)];
		Network.Instantiate (Objective, objPoint.transform.position, Quaternion.identity,0);
	}

	/*
	 * Adds basic buttons to start a server, refresh the serverlist and to join a server when one is available 
	 */
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

		/*
		 * Forces the player to set a name before spawning him.
		 */
		if(nameSet != true && joinedServer == true)
		{
			name = GUI.TextField(new Rect (Screen.width/2 - 100 , Screen.height/2, 100, 25), name);

			if(GUI.Button(new Rect(Screen.width/2 + 20, Screen.height/2, 100, 25),"Set Name"))
			{
				SetPlayerName();
				nameSet = true;
			}
		}
			/*
			 * Adding a Exit button so that the player can exit the game.
		 	 */
			if (GUI.Button (new Rect (Screen.width - 100,Screen.height - 50,100,50), "Exit"))
			{
				Application.Quit();
			}
		
	}

	/*
	 * Forces the player to set a name before spawning him.
	 */
	void SetPlayerName()
	{
		if(!string.IsNullOrEmpty(name.Trim()))
		{			
			SpawnPlayer(name);
			name = string.Empty;
		}
	}



}
