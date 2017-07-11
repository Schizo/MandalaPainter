using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

public class TexturePainter
	{

		public  int textureWidth;
		public  int textureHeight;

		public Texture2D texture;
		public TexturePainter ()
		{
			this.textureWidth =  Screen.currentResolution.width;
			this.textureHeight =  Screen.currentResolution.height;
			texture = new Texture2D(this.textureWidth, this.textureHeight, TextureFormat.ARGB32, false);
			fillColor (Color.white);
		}

	public void fillColor(Color color){

		for(var x = 0; x < this.textureWidth; ++x)
		{
			for(var y = 0; y < this.textureHeight; ++y) 
				this.texture.SetPixel (x, y, color);
		}
		this.texture.Apply ();
	
	}

	public  void drawTexture( Vector3[] drawPoints){

		for(int j = 0; j < drawPoints.Count()-1; j++){

			int x1 = (int)ExtensionMethods.Remap(drawPoints[j].x, -470, 470, 0, textureWidth);
			int y1 = (int)ExtensionMethods.Remap(drawPoints[j].y, -300, 300, 0, textureHeight);

			int x2 = (int)ExtensionMethods.Remap(drawPoints[j+1].x, -470, 470, 0, textureWidth);
			int y2 = (int)ExtensionMethods.Remap(drawPoints[j+1].y, -300, 300, 0, textureHeight);


			TexturePainter.rasterizeBresenham (x1, y1, x2, y2, texture);

		}
	}


	public static void rasterizeBresenham(int x, int y, int x2, int y2, Texture2D texture){
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
			//Write into the texture	
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
	}