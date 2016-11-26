using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper  {
	public static Vector3 rotatePoint(Vector3 position,float angle)
	{
		float s = Mathf.Sin(Mathf.Deg2Rad * angle) ;
		float c = Mathf.Cos(Mathf.Deg2Rad * angle) ;


		// rotate point
		float xnew = position.x * c - position.y * s;
		float ynew = position.x * s + position.y * c;

		// translate point back:
		position.x = xnew;
		position.y = ynew;
		return position;
	}
}
