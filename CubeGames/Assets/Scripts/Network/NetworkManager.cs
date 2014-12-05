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

	void Start()
	{
		points = GameObject.FindGameObjectsWithTag("Respawn");
	}

	void Update()
	{
		totalObjective = GameObject.FindGameObjectsWithTag ("Objective");
		if(joinedServer == true)
		{
			if(totalObjective.Length != 5)
		   		SpawnObjective();
		}


	}

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
		GameObject objPoint = points [Random.Range (0, points.Length)];
		Network.Instantiate (Objective, objPoint.transform.position, Quaternion.identity,0);
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
		
		if(nameSet != true && joinedServer == true)
		{
			name = GUI.TextField(new Rect (Screen.width/2 - 100 , Screen.height/2, 100, 50), name);

			if(GUI.Button(new Rect(Screen.width/2 + 20, Screen.height/2, 100, 50),"Set Name"))
			{
				SpawnPlayer();
				SetPlayerName();
				nameSet = true;
			}
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
