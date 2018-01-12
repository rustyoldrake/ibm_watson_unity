using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sizeobject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// PUMP UP SCALE
		if (Input.GetKeyDown(KeyCode.Period))
		{
			transform.localScale += new Vector3(0.1F, 0.1F, 0.1F);

		}
		// SHRINK SCALE
		if (Input.GetKeyDown(KeyCode.Comma))
		{
			transform.localScale -= new Vector3(0.1F, 0.1F, 0.1F);
		}


		
	}
}
