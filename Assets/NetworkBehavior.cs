using UnityEngine;
using System.Collections;

public class NetworkBehavior : Photon.MonoBehaviour {

	GameObject _player;

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

		bool whitePlayerExists = false;
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			if (player.customProperties["color"] != null && player.customProperties["color"].Equals("white")) {
				whitePlayerExists = true;
			}
		}

		if (!whitePlayerExists) {
			_player = PhotonNetwork.Instantiate ("OculusPlayer", new Vector3 (0, 8.85f, -14), Quaternion.identity, 0);
			_player.SetActive(true);

			ExitGames.Client.Photon.Hashtable settings = new ExitGames.Client.Photon.Hashtable(1);
			settings.Add("color","white");
			PhotonNetwork.player.SetCustomProperties(settings);

			object[] data = {Color.green.r,Color.green.g,Color.green.b};
			PhotonNetwork.Instantiate ("Pointer", new Vector3 (0, 9, -4), Quaternion.identity, 0, data);

		} else {
			_player = PhotonNetwork.Instantiate ("OculusPlayer", new Vector3 (0, 8.85f, 14), Quaternion.LookRotation(new Vector3 (0, 0, -1)), 0);
			_player.SetActive(true);

			ExitGames.Client.Photon.Hashtable settings = new ExitGames.Client.Photon.Hashtable(1);
			settings.Add("color","black");
			PhotonNetwork.player.SetCustomProperties(settings);

			object[] data = {Color.blue.r,Color.blue.g,Color.blue.b};
			PhotonNetwork.Instantiate ("Pointer", new Vector3 (0, 9, 4), Quaternion.identity, 0, data);
		}

		Camera.main.enabled = false;
	}

	void OnPhotonCreateRoomFailed () {
		Debug.Log ("Connection failed");
	}

	public GameObject GetPlayer() {
		return _player;
	}
}
