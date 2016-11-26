using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MouseOverEvents : MonoBehaviour, IPointerEnterHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (EventSystem.current.IsPointerOverGameObject ()) { // UI elements getting the hit/hover

		}
		
	}

	public void OnPointerEnter(PointerEventData dataName)
	{
	}
}
