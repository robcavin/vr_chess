using UnityEngine;
using System.Collections;

public class NewBehaviourScript : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("Hellow!");

		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.ConnectUsingSettings ("0.1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;

	void OnConnectedToMaster () {
		RoomOptions roomOptions = new RoomOptions () { isVisible = false, maxPlayers = 4 };
		PhotonNetwork.JoinOrCreateRoom ("VRoom", roomOptions, TypedLobby.Default);
	}

	void OnCreatedRoom () {
		Debug.Log ("Room created");
	}

	void OnJoinedRoom () {
		Debug.Log ("Connected to Room");
		PhotonNetwork.Instantiate ("Pointer", Vector3.zero, Quaternion.identity, 0);
	}

	void OnPhotonCreateRoomFailed () {
		Debug.Log ("Connection failed");
	}
}
