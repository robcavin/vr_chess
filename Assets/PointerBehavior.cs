using UnityEngine;
using System.Collections;

public class PointerBehavior : Photon.MonoBehaviour {

	public NetworkBehavior network;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	private Vector3 lastPostion;

	Vector3 accumulatedMousePosition;
	GameObject grabbed = null;
	Vector3 grabbedPosition;
	//float grabbedBottom;

	// Use this for initialization
	void Start () {
		lastPostion = transform.position;
		accumulatedMousePosition = new Vector3 (Screen.width/2, Screen.height/2, 1);

		network = GameObject.Find ("Networking").GetComponent<NetworkBehavior> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (photonView.isMine) {
			float scale = 50;
			Vector3 mouseDelta = new Vector3 (scale * Input.GetAxis ("Mouse X"), scale * Input.GetAxis ("Mouse Y"), 0);
			accumulatedMousePosition += mouseDelta;
			accumulatedMousePosition.x = Mathf.Clamp (accumulatedMousePosition.x, 0, Screen.width);
			accumulatedMousePosition.y = Mathf.Clamp (accumulatedMousePosition.y, 0, Screen.height);

			Vector3 mousePosition = new Vector3 (accumulatedMousePosition.x / Screen.width - 0.5f, 0, accumulatedMousePosition.y / Screen.height - 0.5f);
			Quaternion cameraAboutY = Quaternion.Euler (new Vector3 (0, network.GetPlayer().transform.rotation.eulerAngles.y, 0));
			Vector3 relativePosition = cameraAboutY * mousePosition;
			transform.position = new Vector3 (10 * (relativePosition.x),
		                                  	  transform.position.y,
			                                  10 * (relativePosition.z));
	
			if (Input.GetMouseButtonDown (0)) 
				Grab ();
			else if (grabbed)
				Drag ();
		} else {
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp (syncStartPosition, syncEndPosition, syncTime / syncDelay);
		}
	}

	void Grab() {
		if (grabbed) {
			grabbed.GetPhotonView().RPC("released",PhotonTargets.All);
			grabbed = null;
		}
		else {
			RaycastHit hit;
			Ray ray = new Ray(transform.position, new Vector3(0,-1,0));
			//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				grabbed = hit.collider.gameObject;

				int newID = PhotonNetwork.AllocateViewID();
				grabbed.GetPhotonView().RPC("grabbed",PhotonTargets.All,newID);
				grabbedPosition = hit.transform.position;
				//grabbedBottom = hit.collider.bounds.min.y;
			}
		}
	}
	
	void Drag() {
		Vector3 newLocation = transform.position;
		newLocation.y = grabbedPosition.y + 2.0f;
		grabbed.transform.position = newLocation;
		
		//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		/*Vector3 sphereBase = sphere.transform.position;
		sphereBase.y += 5;
		Ray ray = new Ray(Camera.main.transform.position, sphereBase - Camera.main.transform.position);
		Vector3 pointOnPlane = new Vector3 (grabbedPosition.x, grabbedBottom, grabbedPosition.z);
		Plane plane = new Plane(new Vector3(0.0f, -1.0f, 0.0f), pointOnPlane);
		float distance;
		if (plane.Raycast(ray, out distance)) {
			Vector3 position = ray.GetPoint(distance);
			position.y += grabbedPosition.y - grabbedBottom + 2.0f;
			grabbed.position = position;
		}*/
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
		
		//if (stream.isWriting) {
		if (stream.isWriting) {
			stream.SendNext (transform.position);		
		} else {
			// Network player, receive data
			Vector3 syncPosition = (Vector3)stream.ReceiveNext();

			syncTime = 0f;
			syncDelay = Mathf.Min(1,Time.time - lastSynchronizationTime);
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition;
			syncStartPosition = transform.position;
		}
	}
		
	void OnPhotonInstantiate(PhotonMessageInfo info){
		renderer.material.color = new Color((float) photonView.instantiationData[0], 
		                                    (float) photonView.instantiationData[1], 
		                                    (float) photonView.instantiationData[2]);
	}
}
