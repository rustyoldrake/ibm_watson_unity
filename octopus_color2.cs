using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octopus_color : MonoBehaviour {


	public MeshRenderer octopusMeshRenderer;

	public Material color_red;
	public Material color_green;
	public Material color_blue;
	public Material color_yellow;
	public Material color_white;

	public float speed;
	public float roto;
	public float moto_x;
	public float moto_y;


	// Use this for initialization
	void Start () {



		transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		roto = 0.0f;
		moto_x = 0.0f;
		moto_y = 0.0f;
		Vector3 orginalPosition = octopusMeshRenderer.transform.position; // for reset

	}
	
	// Update is called once per frame
	void Update () {


		//float step = speed * Time.deltaTime;
		//transform.position = Vector3.MoveTowards(transform.position, target.position, step);

		/// COLOR
		if (Input.GetKeyDown (KeyCode.R)) {
			octopusMeshRenderer.material = color_red;
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			octopusMeshRenderer.material = color_green;
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			octopusMeshRenderer.material = color_blue;
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			octopusMeshRenderer.material = color_yellow;
		}


	
		/// MOVEMENT


		/// Navigation
		/// https://www.youtube.com/watch?v=mC9BfAqwU2Q
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		Vector3 direction = new Vector3 (vertical, 0, horizontal);
		octopusMeshRenderer.transform.Translate (direction.normalized * Time.deltaTime * speed);

		/// Rotataion Update
		transform.Rotate(Time.deltaTime, roto, 0);
		octopusMeshRenderer.transform.Translate(moto_y, 0,0);


		// Rotate the object around its local X axis at 1 degree per second
		//transform.Rotate(Time.deltaTime, 0, 0);
		// ...also rotate around the World's Y axis
		//transform.Rotate(0, Time.deltaTime, 0, Space.World);


		// Keystrokes to toggle on/off rotation
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			roto = 1.0f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			roto = 0.0f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			roto = -1.0f;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			moto_y = 0.1f;
		}		
		if (Input.GetKeyDown (KeyCode.C)) {
			moto_y = -0.1f;
		}

		/// REset octopus
		if (Input.GetKeyDown (KeyCode.Space)) {

		
			octopusMeshRenderer.transform.localPosition = new Vector3(0, 0, 0);

			//gameObject.transform.Rotate(0, -90, 0);  // works 
			octopusMeshRenderer.transform.Rotate(0, -90, 0);  // works (ish)

			roto = 0.0f;
			moto_x = 0.0f;
			moto_y = 0.0f;

		}



	} // update


} // class
