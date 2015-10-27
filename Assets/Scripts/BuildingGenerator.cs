using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildingGenerator : MonoBehaviour {
	
	//These variables are self explanitory I believe
	public int numberOfFloors;
	public Vector2 floorDimensions;
	
	private List<FloorLayoutData> floorInformation = new List<FloorLayoutData>(); 
	private FloorLayoutData previousLayout;
	private int currentLevel = 0;
	// Use this for initialization 
	void Start () {
		UnityEngine.Random.seed = 101;
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
		Vector3 instPos = findPartLocationWithInfluence(new Vector2(2,3),new Vector2(1,1),fld,'s');
		Instantiate(Resources.Load("Stair1"),instPos,Quaternion.identity);
		
		
		previousLayout = fld;
		constructLevel(currentLevel, 2f);
	}
	
	//Makes an actual level, 
	private void constructLevel(int levelNum, float baseHeight){
		
		if (levelNum < numberOfFloors) {
			FloorLayoutData fld = new FloorLayoutData(floorDimensions,previousLayout.floorInfluenceData);
			fld.height = baseHeight;
			//previousLayout.printInfluenceData();
			//fld.printBaseData();
			fillFloorSpace(fld);
			floorInformation.Add(fld);
			Vector3 instPos = findPartLocationWithInfluence(new Vector2(2,3),new Vector2(1,1),fld,'s');
			Instantiate(Resources.Load("Stair1"),instPos,Quaternion.identity);
			
			previousLayout = fld;
			//fld.printLayoutData();
			constructLevel(++levelNum, baseHeight+=2);
		}
		
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
				int fx = Mathf.Min(sx+5, floor.getDimention(0));	
				int fy = Mathf.Min(sy+5, floor.getDimention(1));	
				
				int maxDimension = Mathf.Min((fx-sx),(fy-sy));
				
				int maxX = 1;
				int maxY = maxDimension;
				for(int j = sx;j<fx;j++){
					for(int k = sy;k<fy;k++){
						//if not empty at that position
						if(floor.floorBaseData[j,k]!='e'){
							if(maxY>k-sy){
								maxY = k-sy;
								break;
							}
						}
					}
					if(maxY<=(j-sx+1)){
						break;
					}else{
						maxX++;
					}
				}
				maxDimension = Mathf.Max((maxX-1),maxY);
				//instantiate the floor tile that was just calculated
				string name = maxDimension+"x"+maxDimension+"tile";
				float offset = maxDimension/2f;
				Instantiate(Resources.Load(name),new Vector3(startPos.x+offset,floor.height,startPos.y+offset),Quaternion.identity);
				//floor.height+=0.1f;
				floor.fillLayoutData(startPos,maxDimension);
			}
		}
		//floor.printLayoutData();
	}
	//gets the next empty space going through the x axis, then down a line, then through the x axis again. Returns -1,-1 if floor is full.
	private Vector2 getNextEmptyTile(FloorLayoutData floor){
		
		Vector2 position = new Vector2(-1,-1);
		for(int j = 0;j<floor.getDimention(0);j++){
			for(int k = 0;k<floor.getDimention(1);k++){
				if(floor.floorBaseData[j,k]=='e'){//Look for an empty slot
					return new Vector2(j,k);
				}
			}
		}
		
		return position;
	}
	//method will find room for an object, and return coordinates for instantiation. Use this for object on the floor passed in
	private Vector3 findPartLocation(Vector2 dimensions, Vector2 buffer ,FloorLayoutData fld ,char value){
		
		List<Vector2> potentialLocations = new List<Vector2> ();

		int r1 = fld.getDimention (0) - Mathf.RoundToInt (dimensions.x - buffer.x);
		int r2 = fld.getDimention (1) - Mathf.RoundToInt (dimensions.y - buffer.y);

		int xd = Mathf.RoundToInt (dimensions.x);
		int yd = Mathf.RoundToInt (dimensions.y);

		for (int j = Mathf.RoundToInt(buffer.x); j<=r1; j++) {
			for(int k = Mathf.RoundToInt(buffer.y);k<=r2;k++){

				bool validPos = true;
				//Now we can check for the area designated by the dimensions to see if we found a start position
				for(int l = 0;l<xd;l++){ 
					for(int p = 0;p<yd;p++){
						if(fld.floorObjectData[j+l,k+p]!='e'){//If not empty, end loops
							validPos = false;
							break;
						}
					}
					if(!validPos){
						break;
					}
				}
				if(validPos){
					potentialLocations.Add(new Vector2(j,k));
				}
			}
		}
		if (potentialLocations.Count == 0)
			return new Vector3 (-1, -1, -1);

		int choosePos = UnityEngine.Random.Range(0,potentialLocations.Count);
		Vector2 pos = potentialLocations [choosePos];
		fld.editBaseData(pos,pos+dimensions,value);
		return new Vector3 (pos.x+(dimensions.x/2f),fld.height,pos.y+(dimensions.y/2f));
	}
	//method will find room for an object, and return the vector3 coordinates for instantiation. It will also set the influence layer, which will influence the next layer that is added.
	private Vector3 findPartLocationWithInfluence(Vector2 dimensions, Vector2 buffer ,FloorLayoutData fld ,char value){
		
		List<Vector2> potentialLocations = new List<Vector2> ();
		
		int r1 = fld.getDimention (0) - Mathf.RoundToInt (dimensions.x + buffer.x);
		int r2 = fld.getDimention (1) - Mathf.RoundToInt (dimensions.y + buffer.y);
		//Debug.Log(r1 + " " + r2);
		
		int xd = Mathf.RoundToInt (dimensions.x);
		int yd = Mathf.RoundToInt (dimensions.y);
		
		for (int j = Mathf.RoundToInt(buffer.x); j<=r1; j++) {
			for(int k = Mathf.RoundToInt(buffer.y);k<=r2;k++){
				
				bool validPos = true;
				//Now we can check for the area designated by the dimensions to see if we found a start position
				for(int l = 0;l<xd;l++){ 
					for(int p = 0;p<yd;p++){
						//Debug.Log("[ " + (j+l) + ", "+ (k+p) +"] :" + fld.floorObjectData[j+l,k+p].ToString());
						if(fld.floorObjectData[j+l,k+p]!='e'){//If not empty, end loops
							validPos = false;
							break;
						}
					}
					if(!validPos){
						break;
					}
				}
				if(validPos){
					potentialLocations.Add(new Vector2(j,k));
				}
			}
		}
		//Debug.Log(potentialLocations.Count);
		if (potentialLocations.Count == 0)
			return new Vector3 (-1, -1, -1);
		
		int choosePos = UnityEngine.Random.Range(0,potentialLocations.Count);
		Vector2 pos = potentialLocations [choosePos];
		//Debug.Log(pos);
		
		fld.editObjectData(pos,pos+dimensions,value);
		fld.editInfluenceData(pos,pos+dimensions,value);
		return new Vector3 (pos.x+(dimensions.x/2f),fld.height+1,pos.y+(dimensions.y/2f));
	}
}
