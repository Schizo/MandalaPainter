using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebug : MonoBehaviour {

	Vector3 lastPos = Vector3.one * float.MaxValue;
	List<Vector3> linePoints = new List<Vector3>();
	GameObject lineRenderer;
	public float threshold = .5f;
	public bool clickStarted = false;
	// Use this for initialization
	void Start () {
		lineRenderer = (GameObject)Instantiate (Resources.Load ("Prefabs/lineRendererObject"));

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			clickStarted = true;
			collectCurvePoints();
			UpdateLineRenderer (lineRenderer.GetComponent<LineRenderer>());




		}

	else if (clickStarted) {
			collectCurvePoints();
			UpdateLineRenderer (lineRenderer.GetComponent<LineRenderer>());
	}


		if (Input.GetMouseButtonUp (0)) {
			clickStarted = false;

		}

		if (Input.GetKeyDown ("space")) {
			Debug.Log ("key down");
			linePoints.Clear ();
		}
		if (Input.GetKeyDown ("x")) {
			Debug.Log ("Smooth curve");
		}
		if (Input.GetKeyDown ("a")) {
			Component.Instantiate (lineRenderer.GetComponent<LineRenderer>());
		}
		
	}

	void collectCurvePoints(){

		Vector3 mousePos = Input.mousePosition;
		mousePos.z = Camera.main.nearClipPlane;
		Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);
		mouseWorld.z=-5.0f;
		float dist = Vector3.Distance(lastPos, mouseWorld);
		if(dist <= threshold)
			return;

		lastPos = mouseWorld;
		if(linePoints == null)
			linePoints = new List<Vector3>();

		linePoints.Add(mouseWorld);
	}

	void UpdateLineRenderer(LineRenderer lineRenderer)
	{
		lineRenderer.numPositions = linePoints.Count;

		for (int i = 0; i < linePoints.Count; i++) {
			lineRenderer.SetPosition (i, linePoints[i]);
		}

	}
}
