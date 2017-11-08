using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DigitalRuby.FastLineRenderer;

public class VectorDrawer:MonoBehaviour
	{

		public int numOfLineRenderers = 32;
		public bool mirrorSymmetry = true;
		public List<GameObject> lineRendererHolder = new List<GameObject> ();
		public TexturePainter texturePainter;


		public float startWidth = .5f;
		public float endWidth = .5f;
		public float threshold = .2f;
		public float brushsize = 0.5f;
		public Color lineColor;


		private LineManager lineManager = new LineManager ();
		public Transform drawingLinesParent;
		public GameObject drawingLines;
		//private Dropdown symmetryDropDown;
		private bool drawStraight = false;
		public Collider drawingBoard;
		public Color colorPickerTint = Color.black;
		public Vector3 centerPosition;

		//Status Holders
		private bool clickStarted;


		Material testMaterial;


		public VectorDrawer ()
		{
			lineColor = Color.black;
		}

		public void Start()
		{
			drawingLines = GameObject.FindGameObjectWithTag("drawingLinesParent");
			drawingLinesParent =  drawingLines.GetComponent<Transform>();
			drawingBoard = GameObject.FindGameObjectWithTag ("drawingBoard").GetComponent<Collider>();
			colorPickerTint = GameObject.FindGameObjectWithTag ("colorPickerUI").GetComponent<ColorPickerAdvanced> ().RGBAColor;

		
		}
		public static void smoothGauss(List<Vector3> outLine, List<Vector3> line){
			Debug.Log ("smoooth");


			Vector3 l1, l2, l3, l4, r1, r2, r3, r4, selfVal;

			for (int i = 4; i < line.Count ()-4; i++) {

				l1 = line[i - 1] * 0.05f;
				l2 = line[i - 2] * 0.09f;
				l3 = line[i - 3] * 0.12f;
				l4 = line[i - 4] * 0.15f;

				r1 = line[i + 1] * 0.05f;
				r2 = line[i + 2] * 0.09f;
				r3 = line[i + 3] * 0.12f;
				r4 = line[i + 4] * 0.15f;

				selfVal = line [i] * 0.18f;

				outLine[i] = l1 + l2 + l3 + l4 + r1 + r2 + r3 + r4 + selfVal;


			}

		}



	public static void smoothCurve(List<Vector3> ping, List<Vector3> pong, int times){
		Debug.Log ("smoooth");

		if (times < 1)
			return;

		for (int i = 1; i < ping.Count ()-1; i++) {

			pong [i] = (ping [i - 1] + ping [i + 1]) / 2; //average neighbours

		}
		smoothCurve (pong, ping, times - 1);

	}

	public void setBrushSize(float brushSize){
		startWidth = brushSize;
		endWidth = brushSize;
		brushsize = brushSize;
	}
		

	public void drawMandala(LineRenderer holder, List<Vector3> points){
			holder.useWorldSpace = false;
			holder.positionCount = points.Count();
			holder.SetPositions(points.ToArray());
			holder.material.color = lineColor;
			holder.startWidth = startWidth;
			holder.endWidth = endWidth;
			holder.numCornerVertices = 4;
			holder.numCapVertices = 2;


		holder.SetPositions (points.ToArray());
	
			}

	public void createDrawContainer(List<GameObject> lineRenderers){
		var drawCanvas = new GameObject ();
		drawCanvas.name = System.String.Format("LineDrawContainer{0}", lineManager.getStatus());
		drawCanvas.transform.parent = drawingLinesParent;
		drawingLinesParent =  drawingLines.GetComponent<Transform>();
		foreach (var linerenderer in lineRenderers)
			linerenderer.transform.parent = drawCanvas.transform;
		Debug.Log ("CreateDrawContainer...");
	}

	public void deleteDrawing(){
		Object.Destroy (GameObject.FindGameObjectWithTag("drawingLinesParent"));
		drawingLines = new GameObject ();
		drawingLines.name = "DrawingLines";
		drawingLines.tag = "drawingLinesParent";
		drawingLinesParent =  drawingLines.GetComponent<Transform>();
		Debug.Log ("Clearing...");
	}

	public void undo(){
		Debug.Log ("Undoing...");
		var drawCanvas = System.String.Format("LineDrawContainer{0}", lineManager.getStatus());
		Object.Destroy (GameObject.Find (drawCanvas));
		lineManager.pop ();

	}

	public void pushUndoStack(List<GameObject> lineRendererHolder, List<Vector3> points)
	{
		lineManager.addLine (lineRendererHolder, points, points.Count);
	}

	public void setNumberOfLines(int n){
		numOfLineRenderers = n;
		Debug.Log ("Setting Number of Lines...");
	}

	public void Clear(){
		lineRendererHolder.Clear();
		Debug.Log ("Clearning All Lines...");
		
	}

	public void DrawInteractionVector(List<Vector3> points){
		Debug.Log ("DrawInteractionVector");

		for(int index = 0; index < numOfLineRenderers; index++){
			var lineRendererInstance = (GameObject)Instantiate (Resources.Load ("Prefabs/lineRendererObject"));
			lineRendererInstance.GetComponent<LineRenderer> ().material = testMaterial;


			lineRendererInstance.transform.parent = drawingLinesParent;
			lineRendererHolder.Add(lineRendererInstance);
		}

		pushUndoStack (lineRendererHolder, points);
		createDrawContainer(lineRendererHolder);
	}



}


