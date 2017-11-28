using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTarget : MonoBehaviour {
	float mx = 0;
	float mz = 0;
	public GameObject myplayer;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 10000))
		{
			//Debug.Log("x: " + hit.point.x);
			//Debug.Log("y: " + hit.point.y);
			//Debug.Log("z: " + hit.point.z);
			Vector3 vecy = new Vector3(hit.point.x, myplayer.transform.position.y, hit.point.z);
			transform.position = vecy;
		}
	}
}
