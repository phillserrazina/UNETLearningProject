using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

	// VARIABLES

	// SyncVars are variables that transmits data changes from the server to the clients.
	[SerializeField, SyncVar(hook="OnPlayerNameChange")]
	private string PlayerName = "Default";

	public GameObject PlayerUnitPrefab;

	// FUNCTIONS

	// Use this for initialization
	void Start () 
	{
		// Check Owner
		if(isLocalPlayer == false)
		{
			return;
		}

		// Command the server to spawn unit
		CmdSpawnMyUnit();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isLocalPlayer == false)
		{
			return;
		}

		if(Input.GetKeyDown(KeyCode.S))
		{
			CmdSpawnMyUnit ();
		}

		if(Input.GetKeyDown(KeyCode.N))
		{
			string s = "Player" + Random.Range (1, 100);

			CmdChangePlayerName (s);
		}
	}

	void OnPlayerNameChange(string newName)
	{
		Debug.Log ("OnPlayerNameChange: " + newName);

		// If a hook is used on a SyncVar, the local value does NOT get
		// automatically update.
		PlayerName = newName;

		gameObject.name = newName;
	}

	// COMANDS (Special functions that ONLY get executed on the server)

	private GameObject myPlayerUnit;

	[Command] // "Command" executes on the server, not the client
	void CmdSpawnMyUnit()
	{
		GameObject gameObject = Instantiate (PlayerUnitPrefab);

		myPlayerUnit = gameObject;

//		gameObject.GetComponent<NetworkIdentity> ().AssignClientAuthority (connectionToClient);

		NetworkServer.SpawnWithClientAuthority (gameObject, connectionToClient);
	}

	[Command]
	void CmdMoveUnitUp()
	{
		if(myPlayerUnit == null)
		{
			return;
		}

		myPlayerUnit.transform.Translate (0, 1, 0);
	}

	[Command]
	void CmdChangePlayerName(string s)
	{
		PlayerName = s;
//		RpcChangePlayerName (PlayerName);
	}

	// RPCs (Special functions that ONLY get executed on the clients.

//	[ClientRpc]
//	void RpcChangePlayerName(string s)
//	{
//		PlayerName = s;
//	}
}