using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class controller : MonoBehaviour {

	public float speed = 0.5f;
	public float force = 200;

	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.LeftArrow) | Input.GetKeyDown (KeyCode.A) ) { this.GetComponent<Rigidbody> ().AddForce (new Vector3 (-force, 0, 0)); }
		if (Input.GetKeyDown (KeyCode.RightArrow) | Input.GetKeyDown (KeyCode.D) ) { this.GetComponent<Rigidbody> ().AddForce (new Vector3 (force, 0, 0)); }	
		if (Input.GetKeyDown (KeyCode.UpArrow) | Input.GetKeyDown (KeyCode.W) ) { this.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 0, force)); }
		if (Input.GetKeyDown (KeyCode.DownArrow) | Input.GetKeyDown (KeyCode.S) ) { this.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 0, -force)); }
		if (Input.GetKeyDown (KeyCode.Space)) { this.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, force, 0)); }
		if (Input.GetKeyDown (KeyCode.A)) { this.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 0, 0)); }
	}
}



// some basic code for mapping Emotive EEG brain signal detecter to keystrokes to input for my objects in unity
