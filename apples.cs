using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class apples : MonoBehaviour {

	public IdleChanger _idlechanger;

	// Use this for initialization
	void Start () {

		//_idlechanger.gameObject.transform.localScale += new Vector3(.1F, .1F, .1F);
		
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKeyDown(KeyCode.Q))
		{
			

		}


		// Update is called once per frame
			// PUMP UP SCALE
			if (Input.GetKeyDown(KeyCode.A))
			{
				_idlechanger.gameObject.transform.localScale += new Vector3(.1F, .1F, .1F);

			}
			// SHRINK SCALE
			if (Input.GetKeyDown(KeyCode.Z))
			{
				_idlechanger.gameObject.transform.localScale -= new Vector3(.1F, .1F, .1F);
			}


			// Rotate Clockwise
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				_idlechanger.transform.localEulerAngles += new Vector3(0,10,0);

			}
			// Rotate CounterClockwise
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				_idlechanger.transform.localEulerAngles -= new Vector3(0,10,0);

			}
			
	}
}
