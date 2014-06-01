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
		RoomOptions roomOptions = new RoomOptions () { isVisible = false, maxPlayers = 2 };
		PhotonNetwork.JoinOrCreateRoom ("VRoom", roomOptions, TypedLobby.Default);
	}

	void OnCreatedRoom () {
		Debug.Log ("Room created");
	}

	void enablePlayer(GameObject player) {
		foreach (MonoBehaviour obj in player.GetComponents<MonoBehaviour>())
			obj.enabled = true;

		foreach (CharacterController obj in player.GetComponents<CharacterController>())
			obj.enabled = true;

	}
	void OnJoinedRoom () {
		Debug.Log ("Connected to Room");

		if (PhotonNetwork.player.ID == 1) {
			GameObject player = PhotonNetwork.Instantiate ("OculusPlayer", new Vector3 (0, 8.85f, -14), Quaternion.identity, 0);
			player.SetActive(true);

			PhotonNetwork.Instantiate ("Pointer", new Vector3 (0, 9, -4), Quaternion.identity, 0);

		} else {
			GameObject player = PhotonNetwork.Instantiate ("OculusPlayer", new Vector3 (0, 8.85f, 14), Quaternion.identity, 0);
			player.SetActive(true);

			PhotonNetwork.Instantiate ("Pointer", new Vector3 (0, 9, 4), Quaternion.LookRotation(new Vector3 (0, 0, -1)), 0);
		}

		Camera.main.enabled = false;
	}

	void OnPhotonCreateRoomFailed () {
		Debug.Log ("Connection failed");
	}
}
