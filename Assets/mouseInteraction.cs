using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class mouseInteraction : MonoBehaviour {

	bool clickStarted = false;
	public Texture2D texture;
	// Use this for initialization
	private HashSet<Vector3> drawPointsHash;

	public int width;
	public int height;
	private List<Vector3> drawPoints;

	private List<Vector3> drawPointsTexture;
	LineRenderer lr;


	void Start () {


		width =  Screen.currentResolution.width;
		height =  Screen.currentResolution.height;

		drawPoints = new List<Vector3> ();

		drawPointsTexture = new List<Vector3> ();

		drawPointsHash = new HashSet<Vector3>();
		//StartCoroutine(LateStart(3));
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		GameObject plane = GameObject.FindGameObjectWithTag ("Player");
		plane.GetComponent<Renderer> ().material.mainTexture = texture;
		plane.AddComponent<LineRenderer>();

		lr = plane.GetComponent<LineRenderer> ();




	}


	public void line(int x, int y, int x2, int y2, Texture2D texture) {


//
//		int x = (int)pointA.x;
//		int y = (int)pointA.y;
//
//		int x2 = (int)pointB.x;
//		int y2 = (int)pointB.y;


//		int x = (int)(viewPortPosition.x * Screen.currentResolution.width);
//		int y = (int)(viewPortPosition.y * Screen.currentResolution.height);
//
		int w = x2- x ;
		int h = y2 - y;
		int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
		if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
		if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
		if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
		int longest = Math.Abs(w) ;
		int shortest = Math.Abs(h) ;
		if (!(longest>shortest)) {
			longest = Math.Abs(h) ;
			shortest = Math.Abs(w) ;
			if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
			dx2 = 0 ;            
		}
		int numerator = longest >> 1 ;
		for (int i=0;i<=longest;i++) {
			texture.SetPixel (x, y, Color.red);

			numerator += shortest ;
			if (!(numerator<longest)) {
				numerator -= longest ;
				x += dx1 ;
				y += dy1 ;
			} else {
				x += dx2 ;
				y += dy2 ;
			}
		}
	}



	void rasterize(List<Vector3> rasterizePoints, Texture2D tex){

		int numberOfStrokes = 10;

		for (int i = 0; i < rasterizePoints.Count-1; i++) {
			for(int j = 0; j < numberOfStrokes; j++){
		
				line ((int)(rasterizePoints[i].x+j*0.25), (int)(rasterizePoints[i].y+j*0.25),
					(int)(rasterizePoints[i+1].x+j*0.25), (int)(rasterizePoints[i+1].y+j*0.25), tex);

			}

			for(int j = 0; j < 3; j++){

				line ((int)(rasterizePoints[i].x-j*0.25), (int)(rasterizePoints[i].y-j*0.25),
					(int)(rasterizePoints[i+1].x-j*0.25), (int)(rasterizePoints[i+1].y-j*0.25), tex);

			}
	}
		tex.Apply ();
	}

	IEnumerator LateStart(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		//Your Function You Want to Call

		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		GameObject plane = GameObject.FindGameObjectWithTag ("Player");
		plane.GetComponent<Renderer> ().material.mainTexture = texture;

		for (int y = 0; y < texture.height; y++)
		{
			for (int x = 0; x < texture.width; x++)
			{
				Color color = ((x & y) != 0 ? Color.white : Color.gray);
				texture.SetPixel(x, y, color);
			}
		}


		texture.Apply(false);
	}
		
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			clickStarted = true;




		} else if (clickStarted) {
			
			var viewPortPosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);

			if (viewPortPosition.x > 0 && viewPortPosition.x <= 1 &&
			    viewPortPosition.y > 0 && viewPortPosition.y <= 1) {


				Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//Debug.Log (mouseWorld);
				mouseWorld.z=-1.0f;


				int x = (int)(viewPortPosition.x * Screen.currentResolution.width);
				int y = (int)(viewPortPosition.y * Screen.currentResolution.height);
			

				drawPoints.Add (mouseWorld);
				drawPointsTexture.Add(new Vector3(x, y, -10.0f));
				drawPointsHash.Add (mouseWorld);


			}
		}


			if (Input.GetMouseButtonUp (0)) {

				foreach (var drawPoint in drawPoints) {
					texture.SetPixel((int)drawPoint.x, (int)drawPoint.y, Color.red);
				}

//				drawPoints.ForEach (delegate(Vector3 vec) {
//					Debug.Log (vec);
//				});


//				drawPointsHash.ForEach (delegate(Vector3 vec) {
//					Debug.Log (vec);
//				});
//				foreach(var point in drawPointsHash){
//					Debug.Log (point);
//				}

	
				lr.positionCount = drawPoints.Count;
				lr.SetPositions (drawPoints.ToArray ());


				texture.Apply (false);
				clickStarted = false;


			rasterize (drawPointsTexture, texture);

				Debug.Log ("texture applied");
			}


	}
}