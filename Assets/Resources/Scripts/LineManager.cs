using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class LineDescription{
	public List<GameObject> lineObjects = new List<GameObject>();
	List<Vector3> linePoints = new List<Vector3>();
	int numOfPoints = 0;

	public LineDescription(List<GameObject> lineRenderers, List<Vector3> linePoints, int numOfPoints){
		this.lineObjects = lineRenderers;
		this.linePoints = linePoints;
		this.numOfPoints = numOfPoints;
	
	}
}

public class LineManager{
	public List<LineDescription> lineDescriptions = new List<LineDescription>();
	public int drawCounter{ get; set; }

	public void addLine(List<GameObject> lineRenderers, List<Vector3> linePoints, int numOfPoints){
		LineDescription lineDescription = new LineDescription (lineRenderers, linePoints, numOfPoints);
		lineDescriptions.Add (lineDescription);
	}

	public int getStatus(){
		return lineDescriptions.Count;
	}

	public void pop(){
		lineDescriptions.RemoveAt (lineDescriptions.Count - 1);
	}

	public void clear(){
		Debug.Log ("Clearing");
		
		for (int i = 0; i < lineDescriptions.Count; i++) {
			for (int j = 0; i < lineDescriptions [i].lineObjects.Count; j++) {
			Debug.Log ("hey");
			Object.Destroy (lineDescriptions [i].lineObjects [j]);
			}
		}
	}
}


