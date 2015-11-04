using UnityEngine;
using System.Collections.Generic;

public class FloorLayoutData {

	public char[,] floorBaseData;
	public char[,] floorObjectData;
	public char[,] floorInfluenceData;
	public float height = 0;
	
	//Called if the base
	public FloorLayoutData(Vector2 floorDimensions){
		int xv = Mathf.RoundToInt(floorDimensions.x);
		int yv = Mathf.RoundToInt(floorDimensions.y);
		floorBaseData = new char[xv,yv];
		floorObjectData = new char[xv,yv];
		floorInfluenceData = new char[xv,yv];
		for(int j = 0;j<xv;j++){
			for(int k = 0;k<yv;k++){
				floorBaseData[j,k] = 'e';							 //e means empty
				floorObjectData[j,k] = 'e';
				floorInfluenceData[j,k] = 'e';
			}
		}
	}
	//Called if not the base
	public FloorLayoutData(Vector2 floorDimensions, char[,] previousFloorData){
		int xv = Mathf.RoundToInt(floorDimensions.x);
		int yv = Mathf.RoundToInt(floorDimensions.y);
		floorBaseData = new char[xv,yv];
		floorObjectData = new char[xv,yv];
		floorInfluenceData = new char[xv,yv];
		for(int j = 0;j<xv;j++){
			for(int k = 0;k<yv;k++){
				if(previousFloorData[j,k]!='d')
					floorBaseData[j,k] = previousFloorData[j,k];			//takes into account previous floor elements, such as stair cases and elevators
				else
					floorBaseData[j,k] = 'e';
				floorObjectData[j,k] = previousFloorData[j,k];
				floorInfluenceData[j,k] = 'e';
			}
		}
	}
	//Call this to edit the floor layout data. Used more for on floor additions.
	public void editBaseData(Vector2 startPos, Vector2 endPos, char value){
		int ixv = Mathf.RoundToInt(startPos.x);
		int fxv = Mathf.RoundToInt(endPos.x);
		int iyv = Mathf.RoundToInt(startPos.y);
		int fyv = Mathf.RoundToInt(endPos.y);
		//Debug.Log (ixv + ":" + fxv + ":" + iyv + ":" + fyv);
		for(int x = ixv;x<fxv;x++){ 
			for(int y = iyv;y<fyv;y++){
				floorBaseData[x,y] = value;
			}
		}
	}
	public void editObjectData(Vector2 startPos, Vector2 endPos, char value){
		int ixv = Mathf.RoundToInt(startPos.x);
		int fxv = Mathf.RoundToInt(endPos.x);
		int iyv = Mathf.RoundToInt(startPos.y);
		int fyv = Mathf.RoundToInt(endPos.y);
		//Debug.Log (ixv + ":" + fxv + ":" + iyv + ":" + fyv);
		for(int x = ixv;x<fxv;x++){ 
			for(int y = iyv;y<fyv;y++){
				floorObjectData[x,y] = value;
			}
		}
	}
	//Call this to edit the floor influence data. Used for things that will effect other floors above them. I.E. Stairs, elevators, etc.
	public void editInfluenceData(Vector2 startPos, Vector2 endPos, char value){
		int ixv = Mathf.RoundToInt(startPos.x);
		int fxv = Mathf.RoundToInt(endPos.x);
		int iyv = Mathf.RoundToInt(startPos.y);
		int fyv = Mathf.RoundToInt(endPos.y);
		//Debug.Log (ixv + ":" + fxv + ":" + iyv + ":" + fyv);
		for(int x = ixv;x<fxv;x++){ 
			for(int y = iyv;y<fyv;y++){
				floorInfluenceData[x,y] = value;
			}
		}
	}
	public void fillLayoutData(Vector2 startPos, int dimension){
		int ixv = Mathf.RoundToInt(startPos.x);
		int iyv = Mathf.RoundToInt(startPos.y);
		//Debug.Log("Starting from: " + startPos.ToString() + " Dimension: " + dimension);
		for(int x = ixv;x<(ixv+dimension);x++){ 
			for(int y = iyv;y<(iyv+dimension);y++){
				floorBaseData[x,y] = 'f';
			}
		}
	}
	public void printBaseData(){
		string data = "";
		for(int j = 0;j<floorBaseData.GetLength(0);j++){
			for(int k = 0;k<floorBaseData.GetLength(1);k++){
				data += floorBaseData[j,k].ToString();
			}
			data += "\n";
		}
		Debug.Log(data);
	}
	public void printObjectData(){
		string data = "";
		for(int j = 0;j<floorObjectData.GetLength(0);j++){
			for(int k = 0;k<floorObjectData.GetLength(1);k++){
				data = floorObjectData[j,k].ToString() + data;
			}
			data = "\n" + data;
		}
		Debug.Log(data);
	}
	public void printInfluenceData(){
		string data = "";
		for(int j = 0;j<floorInfluenceData.GetLength(0);j++){
			for(int k = 0;k<floorInfluenceData.GetLength(1);k++){
				data += floorInfluenceData[j,k].ToString();
			}
			data += "\n";
		}
		Debug.Log(data);
	}
	public int getDimention(int d){
		if (d == 0) {
			return floorBaseData.GetLength(0);
		} else {
			return floorBaseData.GetLength(1);
		}
	}
}
