using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.FastLineRenderer;

public class VisualDebug : MonoBehaviour {

	Vector3 lastPos = Vector3.one * float.MaxValue;
	List<Vector3> linePoints = new List<Vector3>();
	GameObject lineRenderer;
	public float threshold = .5f;
	public bool clickStarted = false;
	FastLineRenderer fastLineRenderer;
	GameObject cached;
	FastLineRenderer cachedFastLineRenderer;
	List<FastLineRenderer> fastLineRenderers = new List<FastLineRenderer>();
	List<Vector3> points;

	// Use this for initialization
	void Start () {
		points =  new List<Vector3>();
		fastLineRenderer = this.gameObject.GetComponent<FastLineRenderer> ();
		drawFastLine ();
		lineRenderer = (GameObject)Instantiate (Resources.Load ("Prefabs/lineRendererObject"));
		cached = (GameObject)Instantiate (Resources.Load ("Prefabs/lineRendererObjectFastEmpty"));
		for (int i = 0; i < 10; i++) {
			cachedFastLineRenderer = FastLineRenderer.CreateWithParent (cached, fastLineRenderer);
			fastLineRenderers.Add (cachedFastLineRenderer);
		}
	}

	private void DoGrid()
	{
		FastLineRendererProperties props = new FastLineRendererProperties
		{
			Radius = 2.0f
		};
		Bounds gridBounds = new Bounds();

		// *** Note: For a 2D grid, pass a value of true for the fill parameter for optimization purposes ***

		// draw a grid cube without filling
		gridBounds.SetMinMax(new Vector3(-2200.0f, -1000.0f, 1000.0f), new Vector3(-200.0f, 1000.0f, 3000.0f));
		fastLineRenderer.AppendGrid(props, gridBounds, 250, false);

		// draw a grid cube with filling
		gridBounds.SetMinMax(new Vector3(200.0f, -1000.0f, 1000.0f), new Vector3(2200.0f, 1000.0f, 3000.0f));
		fastLineRenderer.AppendGrid(props, gridBounds, 250, true);

		// commit the changes
		fastLineRenderer.Apply(true);
	}

	private void drawFastLine(){
		//fastLineRenderer.SendToCache ();
		fastLineRenderer.Reset ();
		FastLineRendererProperties props = new FastLineRendererProperties
		{
			Radius = 5.0f
		};
		props.LineJoin = FastLineRendererLineJoin.Round;

		// generate zig zag
		List<Vector3> points = new List<Vector3>();


		// animation time per segment - set to 0 to have the whole thing appear at once
		const float animationTime = 0.1f;

		props.LineJoin = FastLineRendererLineJoin.Round;
		Bounds gridBounds = new Bounds();

		// *** Note: For a 2D grid, pass a value of true for the fill parameter for optimization purposes ***
		// add the list of line points



		fastLineRenderer.AddLine(props, linePoints, (FastLineRendererProperties _props) =>
			{
				// random color
				//props.Color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), byte.MaxValue);

				// animate this line segment in later
				//props.AddCreationTimeSeconds(animationTime);

				// increase the radius
				//props.Radius += 0.05f;
			}, true, true);


		fastLineRenderer.Apply(true);

	}

	private void CreateContinuousLineFromList()
	{
		// reset the line renderer - if you are appending to an existing line, don't call reset, as you would lost all the previous points
		fastLineRenderer.Reset();

		// generate zig zag
		List<Vector3> points = new List<Vector3>();
		for (int i = 0; i < 50; i++)
		{
			points.Add(new Vector3(-200 + (i * 10.0f), 100 + (150.0f * -(i % 2)), 100.0f));
		}

		// animation time per segment - set to 0 to have the whole thing appear at once
		const float animationTime = 0.1f;

		// create properties - do this once, before your loop
		FastLineRendererProperties props = new FastLineRendererProperties();
		props.Radius = 0.1f;
		props.LineJoin = FastLineRendererLineJoin.Round;

		// add the list of line points
		fastLineRenderer.AddLine(props, points, (FastLineRendererProperties _props) =>
			{
				// random color
				props.Color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), byte.MaxValue);

				// animate this line segment in later
				props.AddCreationTimeSeconds(animationTime);

				// increase the radius
				props.Radius += 0.05f;
			}, true, true);

		// must call apply to make changes permanent
		fastLineRenderer.Apply();
	}


	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			clickStarted = true;
			collectCurvePoints();

		}

	else if (clickStarted) {
			collectCurvePoints();
			//UpdateLineRenderer (lineRenderer.GetComponent<LineRenderer>());
			//drawFastLine ();
			Debug.Log(linePoints.Count);
			FastLineRendererProperties props = new FastLineRendererProperties();
			props.Radius = 0.1f;
			props.LineJoin = FastLineRendererLineJoin.Round;

			const float animationTime = 0.1f;

			for (int i = 1; i < fastLineRenderers.Count; i++) {
				fastLineRenderers[i].Reset ();
				fastLineRenderers[i].AddLine(props, linePoints, (FastLineRendererProperties _props) =>
					{
					}, true, true);
				
				fastLineRenderers [i].Apply ();
			}

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
		lineRenderer.positionCount = linePoints.Count;

		for (int i = 0; i < linePoints.Count; i++) {
			lineRenderer.SetPosition (i, linePoints[i]);
		}

	}
}
