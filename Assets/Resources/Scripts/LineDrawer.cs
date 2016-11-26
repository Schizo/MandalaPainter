using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
	List<Vector3> linePoints = new List<Vector3>();
	public float startWidth = .5f;
	public float endWidth = .5f;
	public float threshold = .5f;


	public Vector3 mirror = new Vector3(-1.0f, 1.0f, 1.0f);
	List<GameObject> lineRendererHolder = new List<GameObject> ();

	public int numOfLineRenderers = 2;
	private bool clickStarted;

	public bool setRandomColor;
	public int currentIndex = 0;
	private GameObject drawingLines;
	public Color lineColor;
	Material testMaterial;
	bool commandPressed = false;
	bool mirrorSymmetry = true;
	private int maxNumSymmetries = 92;


	Vector3 lastPos = Vector3.one * float.MaxValue;

	private LineManager lineManager = new LineManager ();
	private Dictionary<int, int> divisorMap;
	private Transform drawingLinesParent;
	private Dropdown symmetryDropDown;
	private bool drawStraight = false;

	public Slider myslider;

	void Awake()
	{
		divisorMap = new Dictionary<int, int> () {
			{1, 1}, {2, 2}, {3, 3}, {4, 4}, {5, 5}, {6, 6}, {7, 8}, 
			{8, 9}, {9, 10}, {10, 12}, {11, 15}, {12, 18}, {13, 20}, 
			{14, 24}, {15, 30}, {16, 36}, {17, 40}, {18, 45}, {19, 60},
			{20, 72}, {21, 90}, {22, 120}, {23, 180}, {24, 360}

//			{1, 1},{2, 2},{3, 3},{4, 5},{5, 8}, {6, 13},{7, 21}, {8, 34}, {9, 55}, {10, 89}
		};
	}



	void Start(){
		myslider = GameObject.Find ("Slider").GetComponent<Slider>();

		symmetryDropDown = GameObject.Find ("Dropdown").GetComponent<Dropdown>();
		List<System.String> options = new List<System.String> ();
		for (int i = 0; i <  maxNumSymmetries; i++)
			options.Add (i.ToString ());
			
		symmetryDropDown.AddOptions (options);
		drawingLines = GameObject.FindGameObjectWithTag("drawingLinesParent");
		drawingLinesParent =  drawingLines.GetComponent<Transform>();


	}

	void Update()
	{
		
		//Debug.Log(myslider.transform.localPosition);
		//if (isDrawArea ()) {
			InputInteractions ();
			DrawInteractions ();
		//}
	}

	public void setBrushSize(float brushSize){
		startWidth = brushSize;
		endWidth = brushSize;
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

	public void pushUndoStack(List<GameObject> lineRendererHolder)
	{
		lineManager.addLine (lineRendererHolder, linePoints, linePoints.Count);
	}

	public void setNumberOfLines(int n){
		numOfLineRenderers = n;
		Debug.Log ("Setting Number of Lines...");
	}

	void DrawInteractions(){

		//We start drawing
		var viewPortPosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);

		if (Input.GetMouseButtonDown(0))
			Debug.Log (Camera.main.ScreenToWorldPoint (Input.mousePosition));


		if (Input.GetMouseButtonDown (0) && viewPortPosition.y < 0.92) {
			

			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
				Debug.Log (hit.collider);




			clickStarted = true;

			for(int index = 0; index < numOfLineRenderers; index++){
				var lineRendererInstance = (GameObject)Instantiate (Resources.Load ("Prefabs/lineRendererObject"));
				lineRendererInstance.GetComponent<LineRenderer> ().material = testMaterial;
				lineRendererInstance.transform.parent = drawingLinesParent;
				lineRendererHolder.Add(lineRendererInstance);
			}

			pushUndoStack (lineRendererHolder);
			createDrawContainer(lineRendererHolder);
			DrawHandle();

		//We are still drawing
		} else if (clickStarted) {
			DrawHandle ();
		}

		//Mouse down keep record of the curves
		if (Input.GetMouseButtonUp (0)) {
			clickStarted = false;
			linePoints.Clear ();
			lineRendererHolder.Clear ();
		}
	}


	void DrawHandle(){
		InputInteractions ();
		collectCurvePoints ();
		drawMandala ();
	
	}

	void InputInteractions(){
		Camera.main.orthographicSize += Input.GetAxis ("Mouse ScrollWheel");



		Vector2 test = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);



		if (Input.GetKeyDown ("x")) {
			Debug.Log(lineManager.getStatus());
		}
		if (Input.GetKeyDown (KeyCode.LeftCommand)) {
			commandPressed = true;
			Debug.Log ("Command Key Pressed");
		} else if (Input.GetKeyUp (KeyCode.LeftCommand))
			commandPressed = false;

		//if (Input.GetKeyDown ("z") && commandPressed) {
		if (Input.GetKeyDown ("z")) {
			Debug.Log ("Performing undo");
			undo ();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
			symmetryDropDown.value += 1;
		if (Input.GetKeyDown(KeyCode.DownArrow))
			symmetryDropDown.value -= 1;
		if (Input.GetKeyDown (KeyCode.M)) {
			mirrorSymmetry = !mirrorSymmetry;
		}
		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)) {
			drawStraight = true;
		}
		else if (Input.GetKeyUp (KeyCode.LeftShift) || Input.GetKeyUp (KeyCode.RightShift)) {
			drawStraight = false;
		}
	}

	bool isDrawArea(){

		Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		screenPos = new Vector2(screenPos.x, screenPos.y);

		if (screenPos.x > -470 && screenPos.x < 470 && screenPos.y > -300 && screenPos.y < 300)
			return true;
		return false;
	}



	void collectCurvePoints(){
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = Camera.main.nearClipPlane;
		Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);
		//Debug.Log (mouseWorld);
		mouseWorld.z=-5.0f;
		float dist = Vector3.Distance(lastPos, mouseWorld);
		if(dist <= threshold)
			return;

		if (drawStraight)
			return;

		lastPos = mouseWorld;
		if(linePoints == null)
			linePoints = new List<Vector3>();

		//mouseWorld = new Vector3 ((int)mouseWorld.x, (int)mouseWorld.y, (int)mouseWorld.z); 
		linePoints.Add(mouseWorld);
	}



	void drawMandala(){
		int counter = 0;
		foreach (var lineRenderer in lineRendererHolder) {
			var holder = lineRenderer.GetComponent<LineRenderer>();
			holder.useWorldSpace = false;
			UpdateLineRenderer(holder, counter);
			holder.material.color = lineColor;
			holder.startWidth = startWidth;
			holder.endWidth = endWidth;
			holder.numCornerVertices = 4;
			holder.numCapVertices = 2;
			counter++;
		}
	}

	void UpdateLineRenderer(LineRenderer lineRenderer, int cycle)
	{
		lineRenderer.numPositions = linePoints.Count;
		float scale = 1.0f;


		for (int i = 0; i < linePoints.Count; i++) {
			float angle = cycle*(360.0f/numOfLineRenderers);

			//Inner Symmetry on / off
			if (numOfLineRenderers % 2 == 0 && mirrorSymmetry){
				if (cycle % 2 != 0) 
				{
					scale = -1.0f;
				}
			}




			Vector3 scaleMe = new Vector3 (scale, 1.0f, 1.0f);
			lineRenderer.SetPosition (i, Vector3.Scale(MathHelper.rotatePoint(linePoints[i], angle), scaleMe));
		}

	}


}