using UnityEngine;
using System.Collections;

public class FollowMouse : Photon.MonoBehaviour {

	Vector3 accumulatedMousePosition;
	GameObject grabbed = null;
	Vector3 grabbedPosition;
	float grabbedBottom;
	Rigidbody grabbedRigidBody;

	// Use this for initialization
	void Start () {
		accumulatedMousePosition = new Vector3 (Screen.width/2, Screen.height/2, 1);
	}
	
	// Update is called once per frame
	void Update () {
		float scale = 50;
		Vector3 mouseDelta = new Vector3 (scale * Input.GetAxis("Mouse X"), scale * Input.GetAxis("Mouse Y"), 0);
		accumulatedMousePosition += mouseDelta;
		accumulatedMousePosition.x = Mathf.Clamp (accumulatedMousePosition.x, 0, Screen.width);
		accumulatedMousePosition.y = Mathf.Clamp (accumulatedMousePosition.y, 0, Screen.height);

		transform.position = new Vector3 (-5 + 10 * (accumulatedMousePosition.x / Screen.width),
		                                  transform.position.y,
		                                  -5 + 10 * (accumulatedMousePosition.y / Screen.height));
	
		if (Input.GetMouseButtonDown(0)) 
			Grab();
		else if (grabbed)
			Drag();

	}

	void Grab() {
		if (grabbed) {
			grabbed.GetPhotonView().RPC("released",PhotonTargets.All,PhotonNetwork.player.ID);
			//grabbed.rigidbody.constraints = RigidbodyConstraints.None;
			grabbed = null;
		}
		else {
			RaycastHit hit;
			Ray ray = new Ray(transform.position, new Vector3(0,-1,0));
			//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				grabbed = hit.collider.gameObject;
				grabbed.GetPhotonView().RPC("grabbed",PhotonTargets.All,PhotonNetwork.player.ID);
				//grabbed.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
				grabbed.transform.rotation = Quaternion.AngleAxis(270,new Vector3(1,0,0));
				grabbedPosition = hit.transform.position;
				grabbedBottom = hit.collider.bounds.min.y;
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
}
