using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VisualHelper : MonoBehaviour {

	public float cycle;
	public int numOfLineRenderers;
	public List<Vector3> linePoints;
	public Vector3 startPoint;
	public Vector3 endPoint;
	public float lineWidth = 0.1f;
	public Color lineColor;



	//Mirror Lines
	private GameObject DrawingLinesMirror;
	private Transform DrawingLinesMirrorTransform;
	private List<GameObject> DrawingLinesMirrorHolder = new List<GameObject>();
	public bool drawMirrorLines = false;

	//Start Lines
	private GameObject DrawingLinesStart;
	private Transform DrawingLinesStartTransform;
	private List<GameObject> DrawingLinesStartHolder = new List<GameObject>();
	public bool drawStartLines;


	private int totalLineRenderers;


	void Awake () {
	
	}


	void Start(){
		totalLineRenderers = 100;
		DrawingLinesMirror = GameObject.Find ("DrawingLinesMirror");
		DrawingLinesMirrorTransform =  DrawingLinesMirror.GetComponent<Transform>();

		DrawingLinesStart = GameObject.Find ("DrawingLinesStart");
		DrawingLinesStartTransform =  DrawingLinesStart.GetComponent<Transform>();


		startPoint = new Vector3 (0.0f, 0.0f, -10.0f);
		endPoint = new Vector3 (0.0f, 1000.0f, -10.1f);
		linePoints.Add (startPoint);
		linePoints.Add (endPoint);


		createStraightLines (DrawingLinesStartHolder, DrawingLinesStartTransform);
		updateSymmetryLines (DrawingLinesStartHolder, false);

		createStraightLines (DrawingLinesMirrorHolder, DrawingLinesMirrorTransform);
		updateSymmetryLines (DrawingLinesMirrorHolder, true);

		hideHelperLines ();


	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.H)){
			drawStartLines = !drawStartLines;
			hideHelperLines ();
		}

	}

	public void numOfLineRenderersChanged(){
		updateSymmetryLines (DrawingLinesMirrorHolder, true);
		updateSymmetryLines (DrawingLinesStartHolder, false);
	}



	public void hideHelperLines(){
		
		DrawingLinesMirror.SetActive (drawMirrorLines);
		DrawingLinesStart.SetActive (drawStartLines);
	}

	public void hideMirrorLines(){
		drawMirrorLines = !drawMirrorLines;
		DrawingLinesMirror.SetActive (drawMirrorLines);
	
	}

	public void hideStartLines(){
		drawStartLines = !drawStartLines;
	
		DrawingLinesStart.SetActive (drawStartLines);

	}


	public void updateSymmetryLines(List<GameObject> lineRendererHolders, bool midPoint){
		numOfLineRenderers = GameObject.Find("DrawingBoard").GetComponent<LineDrawer> ().numOfLineRenderers;
		var counter = 0;
		float angle = (360 / numOfLineRenderers);

		for (int i = 0; i < totalLineRenderers; i++) {
			float rotationDistance = angle*i;

			if (midPoint)
				rotationDistance = angle * i + (angle / 2);

			//Active Helper Lines
			if (counter < numOfLineRenderers){
				lineRendererHolders [i].SetActive (true);
				var holder =  lineRendererHolders [i].GetComponent<LineRenderer> ();
				for (int point = 0; point < linePoints.Count; point++) {
					holder.SetPosition (point, MathHelper.rotatePoint (linePoints [point], rotationDistance));	
					holder.startWidth = lineWidth;
					holder.endWidth = lineWidth;
					holder.material.color = lineColor;
				}
			}
			else
				lineRendererHolders [i].SetActive (false);
			counter += 1;
		}

	}

	void createStraightLines(List<GameObject> lineRendererHolders, Transform parent){

		for(int i = 0; i < totalLineRenderers; i++) {
			//Instance and Parent
			GameObject lineRenderer = ((GameObject)Instantiate (Resources.Load ("Prefabs/lineRendererObject")));
			lineRenderer.transform.parent = parent;
			lineRendererHolders.Add(lineRenderer);

			var holder = lineRenderer.GetComponent<LineRenderer> ();
			for (int point = 0; point < linePoints.Count; point++) {
				holder.SetPosition (point,linePoints[point]);
				holder.startWidth = lineWidth;
				holder.endWidth = lineWidth;
				holder.material.color = lineColor;
			}
		}

	}





}
