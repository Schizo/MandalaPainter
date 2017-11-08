using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class LineManager{
	public List<LineDescription> lineDescriptions = new List<LineDescription>();
	public int drawCounter{ get; set; }
    int hash = 0;
    int count = 0 ;

    Stack<string> undo;
    public void addLine(List<GameObject> lineRenderers, List<Vector3> linePoints, int numOfPoints){
		LineDescription lineDescription = new LineDescription (lineRenderers, linePoints, numOfPoints);
		lineDescriptions.Add (lineDescription);
        hash += 1;
	}

    public void addToUndo(string containerName)
    {
        undo.Push(containerName);
    }

    public string  removeFromUndo()
    {
        return undo.Pop();
    }

	public int getHash(){
        return hash;

    }

    public int getCount()
    {
        return lineDescriptions.Count;
    }
    

	public void pop(){
		lineDescriptions.RemoveAt (lineDescriptions.Count - 1);
	}

	public void clear(){
		Debug.Log ("Clearing");
		
		for (int i = 0; i < lineDescriptions.Count; i++) {
			for (int j = 0; i < lineDescriptions [i].lineObjects.Count; j++) {

			Object.Destroy (lineDescriptions [i].lineObjects [j]);
			}
		}
	}
}


public class LineDescription
{
    public List<GameObject> lineObjects = new List<GameObject>();
    List<Vector3> linePoints = new List<Vector3>();
    int numOfPoints = 0;


    public LineDescription(List<GameObject> lineRenderers, List<Vector3> linePoints, int numOfPoints)
    {
        this.lineObjects = lineRenderers;
        this.linePoints = linePoints;
        this.numOfPoints = numOfPoints;

    }
}
