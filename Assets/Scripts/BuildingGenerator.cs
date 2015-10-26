using UnityEngine;
using System.Collections.Generic;

public class BuildingGenerator : MonoBehaviour {
	
	//These variables are self explanitory I believe
	public int numberOfFloors;
	public Vector2 floorDimensions;
	
	private List<FloorLayoutData> floorInformation = new List<FloorLayoutData>(); 
	private FloorLayoutData previousLayout;
	private int currentLevel = 0;
	// Use this for initialization 
	void Start () {
		constructBase();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	//constructs the base of the building. Checks to see which size is the best fit
	private void constructBase(){
		FloorLayoutData fld = new FloorLayoutData(floorDimensions);
		floorInformation.Add(fld);
		fillFloorSpace(fld);
		
		
		
		previousLayout = fld;
		//constructLevel(currentLevel, 2f);
	}
	
	//Makes an actual level, 
	private void constructLevel(int levelNum, float baseHeight){
		
		
		
	}
	//Filles all of the remaining floor space. Call this when all stairs, ladders, etc have already been added.
	private void fillFloorSpace(FloorLayoutData floor){
		//floor.printLayoutData();
		bool done = false;
		while(!done){
			//get next empty tile
			Vector2 startPos = getNextEmptyTile(floor);
			//no empty slots
			if(startPos.x == -1f || startPos.y == -1f){
				done = true;
				continue;
			}else{//found an emtpy slot
				int sx = Mathf.RoundToInt(startPos.x);	
				int sy = Mathf.RoundToInt(startPos.y);
				int fx = Mathf.Min(sx+5, floor.floorData.GetLength(0));	
				int fy = Mathf.Min(sy+5, floor.floorData.GetLength(1));	
				
				int maxDimensions = Mathf.Min((fx-sx),(fy-sy));
				
				for(int j = sx;j<fx;j++){
					for(int k = sy;k<fy;k++){
						//if full at that position
						if(floor.floorData[j,k]=='f'){
							maxDimensions = k-sy+1;
							break;
						}
					}
					if(maxDimensions<=(j-sx+1)){
						break;
					}
				}
				floor.fillLayoutData(startPos,maxDimensions);
				//instantiate the floor tile that was just calculated
				string name = maxDimensions+"x"+maxDimensions+"tile";
				float offset = maxDimensions/2f;
				Instantiate(Resources.Load(name),new Vector3(startPos.x+offset,0,startPos.y+offset),Quaternion.identity);
				//Debug.Log(maxDimensions);
			}
		}
		//floor.printLayoutData();
	}
	//gets the next empty space going through the x axis, then down a line, then through the x axis again. Returns -1,-1 if floor is full.
	private Vector2 getNextEmptyTile(FloorLayoutData floor){
		
		Vector2 position = new Vector2(-1,-1);
		for(int j = 0;j<floor.floorData.GetLength(0);j++){
			for(int k = 0;k<floor.floorData.GetLength(1);k++){
				if(floor.floorData[j,k]=='e'){//Look for an empty slot
					return new Vector2(j,k);
				}
			}
		}
		
		return position;
	}
}
