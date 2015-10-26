using UnityEngine;
using System.Collections.Generic;

public class FloorLayoutData {

	public char[,] floorData;

	public FloorLayoutData(Vector2 floorDimensions){
		int xv = Mathf.RoundToInt(floorDimensions.x);
		int yv = Mathf.RoundToInt(floorDimensions.y);
		floorData = new char[xv,yv];
		for(int j = 0;j<xv;j++){
			for(int k = 0;k<yv;k++){
				floorData[j,k] = 'e';								//e means empty
			}
		}
	}
	public void editLayoutData(Vector2 startPos, Vector2 endPos, char value){
		int ixv = Mathf.RoundToInt(startPos.x);
		int fxv = Mathf.RoundToInt(endPos.x);
		int iyv = Mathf.RoundToInt(startPos.y);
		int fyv = Mathf.RoundToInt(endPos.y);
		for(int x = ixv;x<fxv;x++){ 
			for(int y = iyv;y<fyv;y++){
				floorData[x,y] = value;
			}
		}
	}
	public void fillLayoutData(Vector2 startPos, int dimension){
		int ixv = Mathf.RoundToInt(startPos.x);
		int iyv = Mathf.RoundToInt(startPos.y);
		//Debug.Log("Starting from: " + startPos.ToString() + " Dimension: " + dimension);
		for(int x = ixv;x<(ixv+dimension);x++){ 
			for(int y = iyv;y<(iyv+dimension);y++){
				floorData[x,y] = 'f';
			}
		}
	}
	public void printLayoutData(){
		string data = "";
		for(int j = 0;j<floorData.GetLength(0);j++){
			for(int k = 0;k<floorData.GetLength(1);k++){
				data += floorData[j,k].ToString();
			}
			data += "\n";
		}
		Debug.Log(data);
	}
}
