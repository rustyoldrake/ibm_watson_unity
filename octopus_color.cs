using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octopus_color : MonoBehaviour {


	public MeshRenderer octopusMeshRenderer;

	public Material color_red;
	public Material color_green;
	public Material color_blue;
	public Material color_yellow;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


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

		
	}
}
