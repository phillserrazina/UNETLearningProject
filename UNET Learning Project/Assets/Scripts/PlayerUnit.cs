using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
 
// PlayerUnit is a unit controlled by the player

public class PlayerUnit : NetworkBehaviour {

	// VARIABLES

	Vector3 velocity;

	// Most correct position for this player
	Vector3 bestGuessPosition;	

	// This is a constantly updated value about our latency to the server.
	// This should probably be something we get from the PlayerConnectionObject.
	float ourLatency = 0f;	

	float latencySmoothingFactor = 10;

	// FUNCTIONS

	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () 
	{
		transform.Translate (velocity * Time.deltaTime);

		if(hasAuthority == false)
		{
			bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);

			transform.position = Vector3.Lerp (transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);

			return;
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			this.transform.Translate (0, 1, 0);
		}

		if (true)
		{
			velocity = new Vector3 (1, 0, 0);

			CmdUpdateVelocity (velocity, transform.position);
		}

	}

	// COMMANDS

	[Command]
	void CmdUpdateVelocity(Vector3 v, Vector3 position)
	{
		// I am on a server
		transform.position = position;
		velocity = v;

		// Knowing the latency:
		// transform.position = position + (v * thisPlayersLatencyToServer);

		RpcUpdateVelocity (velocity, transform.position);

	}

	[ClientRpc]
	void RpcUpdateVelocity(Vector3 v, Vector3 position)
	{
		// I am on a client

		if(hasAuthority == true)
		{
			return;
		}

		// transform.position = position;

		velocity = v;
		bestGuessPosition = position + (velocity * ourLatency);

		// Knowing the latency:
		// transform.position = position + (v * ourLatency);
	}
}
