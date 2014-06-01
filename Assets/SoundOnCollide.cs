using UnityEngine;
using System.Collections;

public class SoundOnCollide : Photon.MonoBehaviour {

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private bool isGrabbed = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (isGrabbed && !photonView.isMine) {
			syncTime += Time.deltaTime;
			rigidbody.position = Vector3.Lerp (syncStartPosition, syncEndPosition, syncTime / syncDelay);
		}
	}

	void OnCollisionEnter () {
		audio.Play ();
	}

	[RPC]
	public void grabbed (int newID) {
		if (photonView.isMine) {
			PhotonNetwork.UnAllocateViewID(photonView.viewID); //Recycle this number (we "only" have 1000 views per default)
		}
		photonView.viewID = newID;

		transform.rotation = Quaternion.AngleAxis(270,new Vector3(1,0,0));
		rigidbody.isKinematic = true;
		isGrabbed = true;
	}

	[RPC]
	public void released () {
		rigidbody.isKinematic = false;
		isGrabbed = false;
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {

		//if (stream.isWriting) {
		if (stream.isWriting) {
			// We own this player: send the others our data
			stream.SendNext (rigidbody.position);
			stream.SendNext (rigidbody.velocity);
		} else {
			// Network player, receive data
			Vector3 syncPosition = (Vector3)stream.ReceiveNext();
			Vector3 syncVelocity = (Vector3)stream.ReceiveNext();
			
			syncTime = 0f;
			syncDelay = Mathf.Min(1,Time.time - lastSynchronizationTime);
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = rigidbody.position;
		}
	}

}
