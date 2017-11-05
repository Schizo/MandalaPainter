using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
	GameObject fps;
	// Use this for initialization
	void Start () {
		fps = GameObject.FindGameObjectWithTag ("FPS");
	}
	
	// Update is called once per frame
	void Update () {

		fps.GetComponent<Text> ().text =  System.String.Format("{0:F2} FPS", 1.0f / Time.deltaTime);
		
	}
}
