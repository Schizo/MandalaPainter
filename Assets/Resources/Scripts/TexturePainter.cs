using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

public class TexturePainter
	{

		public  int textureWidth;
		public  int textureHeight;
		public float brushSize = 1.0f;

		public Texture2D texture;
		public TexturePainter ()
		{
			this.textureWidth =  Screen.currentResolution.width;
			this.textureHeight =  Screen.currentResolution.height;
			texture = new Texture2D(this.textureWidth, this.textureHeight, TextureFormat.RGB24, false);
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

	public void setBrushSize(float brushSize){
		this.brushSize = brushSize;
		Debug.Log ("setting BrushSize");
	}


	public  void drawTexture( List<Vector3> drawPoints){

		for(int j = 0; j < drawPoints.Count()-1; j++){

			int x1 = (int)ExtensionMethods.Remap(drawPoints[j].x, -470, 470, 0, textureWidth);
			int y1 = (int)ExtensionMethods.Remap(drawPoints[j].y, -300, 300, 0, textureHeight);

			int x2 = (int)ExtensionMethods.Remap(drawPoints[j+1].x, -470, 470, 0, textureWidth);
			int y2 = (int)ExtensionMethods.Remap(drawPoints[j+1].y, -300, 300, 0, textureHeight);


			TexturePainter.rasterizeBresenham (x1, y1, x2, y2, texture);
			//TexturePainter.plotLineAA(x1, y1, x2, y2, brushSize, texture);

		}
	}




	public static void plotLineAA(int x0, int y0, int x1, int y1, float wd,  Texture2D texture)
	{
		int dx = Math.Abs(x1-x0), sx = x0<x1 ? 1 : -1;
		int dy = Math.Abs(y1-y0), sy = y0<y1 ? 1 : -1; 
		int err = dx-dy, e2, x2, y2;                       /* error value e_xy */


		Color tint = Color.black;

		float ed = dx+dy == 0 ? 1 : (float)Math.Sqrt((float)dx*dx+(float)dy*dy);

		for ( wd = (wd+1)/2; ;  ){                                         /* pixel loop */
			//setPixelAA(x0,y0, 255*abs(err-dx+dy)/ed);
			tint.a = (float) Math.Max(0.0, 1.0f * (float)(Math.Abs(err-dx+dy)/ed-wd+1));
			texture.SetPixel (x0, y0, tint);
			e2 = err; x2 = x0;
			if (2*e2 >= -dx) {                                    /* x step */
				for (e2 += dy, y2 = y0; e2 < ed*wd && (y1 != y2 || dx > dy); e2 += dx){
					tint.a = (float)Math.Max (0, 1.0f * (Math.Abs (e2 + dy) / ed - wd + 1));
						texture.SetPixel (x0, y2 += sy, tint ); //setPixelAA(x0,y0+sy, 255*(e2+dy)/ed);
				
				}
					if(x0==x1) break;
					e2 = err; err-= dy; x0 += sx;
			} 
			if (2*e2 <= dy) {                                     /* y step */
				for (e2 = dx-e2; e2 < ed*wd && (x1 != x2 || dx < dy); e2 += dy){
					tint.a = (float)Math.Max (0, 1.0f * (Math.Abs (e2) / ed - wd + 1));
						texture.SetPixel (x2 += sx, y0, tint); //setPixelAA(x2+sx,y0, 255*(dx-e2)/ed);
				}
				if (y0 == y1) break;
					err += dx; y0 += sy; 
			}
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