using UnityEngine;
using System.Collections;

public class SoundOnCollide : Photon.MonoBehaviour {
	public bool currentOwner = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter () {
		audio.Play ();
	}

	[RPC]
	public void grabbed (int playerID) {
		currentOwner = PhotonNetwork.player.ID == playerID;
		rigidbody.useGravity = false;
		Debug.Log (name + " grabbed by " + playerID);
	}

	[RPC]
	public void released (int playerID) {
		currentOwner = false;
		rigidbody.useGravity = true;
		Debug.Log (name + " released by " + playerID);
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {

		//if (stream.isWriting) {
		if (stream.isWriting) {
			if (currentOwner) {
			// We own this player: send the others our data
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			}
		} else {
			// Network player, receive data
			this.transform.position = (Vector3)stream.ReceiveNext ();
			this.transform.rotation = (Quaternion)stream.ReceiveNext ();
			Debug.Log(PhotonNetwork.player.ID  + " updated position of " + name);

		}
	}

}
