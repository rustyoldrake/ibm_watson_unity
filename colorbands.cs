using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class color_bands : MonoBehaviour {


	// declare 
	public MeshRenderer sphereMeshRenderer;

	public Material original_material;
	public Material color_black;
	public Material color_brown;
	public Material color_red;
	public Material color_orange;
	public Material color_yellow;
	public Material color_green;
	public Material color_blue;
	public Material color_violet;
	public Material color_grey;
	public Material color_white;

	//private int _numbertest = 1;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// press a Key - get a color
		// this maps to RESISTOR LOOKUP in IEEE / Electronics
		//  
		//  -----(=|=\=|=|)-----
		//
		//Black	0 //Brown	1
		//Red	2 //Orange	3
		//Yellow	4 //Green	5
		//Blue	6 //Violet	7
		//Grey	8 //White	9

		// later you can do a CASE or SWITCH - but for now....

		if (Input.GetKeyDown(KeyCode.Alpha0))
		{ sphereMeshRenderer.material = color_black; }

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{ sphereMeshRenderer.material = color_brown; }

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{ sphereMeshRenderer.material = color_red; }

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{ sphereMeshRenderer.material = color_orange; }

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{ sphereMeshRenderer.material = color_yellow; }

		if (Input.GetKeyDown(KeyCode.Alpha5))
		{ sphereMeshRenderer.material = color_green; }

		if (Input.GetKeyDown(KeyCode.Alpha6))
		{ sphereMeshRenderer.material = color_blue; }
			
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{ sphereMeshRenderer.material = color_violet; }
			
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{ sphereMeshRenderer.material = color_grey; }

		if (Input.GetKeyDown(KeyCode.Alpha9))
		{ sphereMeshRenderer.material = color_white; }
	

		
	}
}
