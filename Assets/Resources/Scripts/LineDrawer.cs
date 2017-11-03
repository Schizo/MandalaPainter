using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
	List<Vector3> linePoints = new List<Vector3>();
	//Vector3[] linePoints;
	public float startWidth = .5f;
	public float endWidth = .5f;
	public float threshold = .2f;
	public float brushsize = 0.5f;
	public int numOfLineRenderers = 32;
	public bool clickStarted = false;
	public bool enableVectorDrawer = false;
	public bool enableTextureDrawer = true;

	public List<List<Vector3>> allPoints; 


	public Vector3 mirror = new Vector3(-1.0f, 1.0f, 1.0f);



	bool commandPressed = false;
	bool mirrorSymmetry = true;
	private int maxNumSymmetries = 92;


	Vector3 lastPos = Vector3.one * float.MaxValue;

	private LineManager lineManager = new LineManager ();

	private bool drawStraight = false;
	public Vector3 centerPosition;
	private TexturePainter texturePainter;
	private VectorDrawer vectorDrawer;

	//UI Elements
	private Dropdown symmetryDropDown;
	public Slider myslider;


	void Start(){

		//Drawing to A texture
		texturePainter = new TexturePainter();

		GameObject plane = GameObject.FindGameObjectWithTag ("Player");
		if (!enableTextureDrawer)
			plane.SetActive (false);
		plane.GetComponent<Renderer> ().material.mainTexture = texturePainter.texture;

		centerPosition = new Vector3 (100.0f, 100.0f, 0.0f);
		myslider = GameObject.Find ("Slider").GetComponent<Slider>();

		symmetryDropDown = GameObject.Find ("Dropdown").GetComponent<Dropdown>();
		List<System.String> options = new List<System.String> ();

		//Populate UI
		for (int i = 0; i <  maxNumSymmetries; i++)
			options.Add (i.ToString ());
			
		symmetryDropDown.AddOptions (options);

		vectorDrawer = this.GetComponent<VectorDrawer> ();
		//vectorDrawer.linePoints = linePoints;

	}

	void Update()
	{
		
		if (isDrawArea ()) {
			InputInteractions ();
			//vectorDrawer.DrawInteractions ();
			DrawInteractions();
		}
	}

	void InputInteractions(){

		//Zoom Camera
		Camera.main.orthographicSize += Input.GetAxis ("Mouse ScrollWheel");


		if (Input.GetKeyDown ("x")) {
			Debug.Log(lineManager.getStatus());
		}
		if (Input.GetKeyDown (KeyCode.LeftCommand)) {
			commandPressed = true;
			Debug.Log ("Command Key Pressed");
		} else if (Input.GetKeyUp (KeyCode.LeftCommand))
			commandPressed = false;

		if (Input.GetKeyDown ("z")) {
			Debug.Log ("Performing undo");
			vectorDrawer.undo ();
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

	public void DrawInteractions(){

		//We start drawing
		var viewPortPosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);


		if (!clickStarted && Input.GetMouseButtonDown (0) && viewPortPosition.x > 0.1) {
			clickStarted = true;

			if (enableVectorDrawer)
				vectorDrawer.DrawInteractionVector (linePoints);

		} 
		else if (clickStarted) {
			DrawHandle ();
		}

		//Mouse down keep record of the curve



		if (Input.GetMouseButtonUp (0)) {
			MouseUp ();



		}
	}


	public void MouseUp(){
		clickStarted = false;

		List<Vector3> pong = new List<Vector3> (linePoints);
		VectorDrawer.smoothGauss (linePoints, pong);
		Draw (); 


		//vectorDrawer.undo ();


		//			if (enableTextureDrawer) {
		//				LateDraw ();
		//
		//				texturePainter.texture.Apply (false);
		//			}



		//colorPickerTint = GameObject.FindGameObjectWithTag ("colorPickerUI").GetComponent<ColorPickerAdvanced> ().RGBAColor;

		//allPoints.Add (linePoints);
		//linePoints = new List<Vector3> ();


		linePoints.Clear ();
		vectorDrawer.Clear ();


	}

	public void DrawHandle(){
		InputInteractions ();
		collectCurvePoints ();
		Draw ();
		//vectorDrawer.drawMandala ();
		//texturePainter.texture.Apply(false);

	
	}

	public void Draw(){
		int counter = 0;




		for (int i = 0; i < numOfLineRenderers; i++){

			calcSymmetricLine (counter, linePoints);

			if (enableVectorDrawer)
				vectorDrawer.drawMandala (vectorDrawer.lineRendererHolder[i].GetComponent<LineRenderer>(), linePoints);

			counter++;
		}		
	}

//	public void LateDraw(){
//		int counter = 0;
//
//		//var points = new Vector3[linePoints.Count];
//		for (int i = 0; i < numOfLineRenderers; i++){
//
//			calcSymmetricLine (counter, linePoints);
//			texturePainter.drawTexture (linePoints);
//			counter++;
//		}		
//		
//	}
//


	public  List<Vector3> calcSymmetricLine(int cycle, List<Vector3> points)
	{

		float scale = 1.0f;
		Vector3 vectorScale = new Vector3 (scale, 1.0f, 1.0f);


		for (int i = 0; i < points.Count; i++) {
			float angle = cycle*(360.0f/numOfLineRenderers);

			//Inner Symmetry on / off
			if (numOfLineRenderers % 2 == 0 && mirrorSymmetry){
				if (cycle % 2 != 0) 
				{
					scale = -1.0f;
					vectorScale.x = scale;
				}
			}
//			Debug.Log (angle);


			points[i] = Vector3.Scale(MathHelper.rotatePoint(linePoints[i], angle), vectorScale);


		}
		return points;

	}

//	public  Vector3 [] calcSymmetricLine(int cycle, Vector3[] points)
//	{
//
//		float scale = 1.0f;
//
//
//		for (int i = 0; i < linePoints.Count; i++) {
//			float angle = cycle*(360.0f/numOfLineRenderers);
//
//			//Inner Symmetry on / off
//			if (numOfLineRenderers % 2 == 0 && mirrorSymmetry){
//				if (cycle % 2 != 0) 
//				{
//					scale = -1.0f;
//				}
//			}
//
//			Vector3 scaleMe = new Vector3 (scale, 1.0f, 1.0f);
//			points[i] = Vector3.Scale(MathHelper.rotatePoint(linePoints[i], angle), scaleMe);
//
//
//		}
//		return points;
//
//	}
//



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

		mouseWorld.z=-1.0f;
		float dist = Vector3.Distance(lastPos, mouseWorld);
		if(dist <= threshold)
			return;

		if (drawStraight)
			return;

		lastPos = mouseWorld;
		if (linePoints == null) {
			Debug.Log ("LinePoints == Null");
			linePoints = new List<Vector3> ();
		}

		linePoints.Add(mouseWorld);
	}

	public void setBrushSize(float brushSize){

		vectorDrawer.setBrushSize (brushSize);
		texturePainter.setBrushSize (brushsize);
		Debug.Log ("CMD: Setting brushSize");
	}



}