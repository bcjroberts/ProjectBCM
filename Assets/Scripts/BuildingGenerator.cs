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
		constructLevel(currentLevel, 2f);
	}
	
	//Makes an actual level, 
	private void constructLevel(int levelNum, float baseHeight){
		
		if (levelNum < numberOfFloors) {
			FloorLayoutData fld = new FloorLayoutData(floorDimensions);
			fld.height = baseHeight;
			addStairs(fld);
			fillFloorSpace(fld);
			floorInformation.Add(fld);
			previousLayout = fld;
			fld.printLayoutData();
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
				int fx = Mathf.Min(sx+5, floor.floorData.GetLength(0));	
				int fy = Mathf.Min(sy+5, floor.floorData.GetLength(1));	
				
				int maxDimensions = Mathf.Min((fx-sx),(fy-sy));
				
				for(int j = sx;j<fx;j++){
					for(int k = sy;k<fy;k++){
						//if not empty at that position
						if(floor.floorData[j,k]!='e'){
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
				Instantiate(Resources.Load(name),new Vector3(startPos.x+offset,floor.height,startPos.y+offset),Quaternion.identity);
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
	//method will find room for an object, and return coordinates for instantiation
	private Vector3 addPiece(Vector2 dimensions, Vector2 buffer ,FloorLayoutData fld ,char value){
		
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
						if(fld.floorData[j+l,k+p]!='e'){//If not empty, end loops
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
		return new Vector3 (pos.x+(dimensions.x/2f),fld.height,pos.y+(dimensions.y/2f));

	}
	//method will look at the previous floor layout to decide where stairs can be added
	private void addStairs(FloorLayoutData fld){
		List<Vector2> potentialStairPositions = new List<Vector2> ();

		for(int j = 1;j<fld.getDimention(0)-1;j++){
			for(int k = 1;k<fld.getDimention(1)-1;k++){
				bool validPos = true;
				try{
					//Check for stair position validity
					for(int l = 0;l<2;l++){
						for(int p = 0;p<3;p++){
							if(previousLayout.floorData[j+l,k+p]!='f'){
								validPos = false;
								break;
							}
						}
						if(validPos==false)
							break;
					}
				}catch(IndexOutOfRangeException e){//Catches an exception essentially saying that the current stair location is invalid
					validPos = false;
				}
				if(validPos){
					potentialStairPositions.Add(new Vector2(j,k));
				}
			}
		}
		Debug.Log (potentialStairPositions.Count);
		int pos = UnityEngine.Random.Range (0,potentialStairPositions.Count);
		fld.editLayoutData(potentialStairPositions[pos],potentialStairPositions[pos]+new Vector2(3f,2f),'s');
	}
}
