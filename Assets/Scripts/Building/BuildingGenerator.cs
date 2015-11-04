using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildingGenerator : MonoBehaviour {
	
	//These variables are self explanitory I believe
	public int numberOfFloors;
	public Vector2 floorDimensions;
	public GameObject buildingContainerObj;
	
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
		Vector3 instPos = findPartLocationWithInfluence(new Vector2(1,3),new Vector2(1,1), new Vector2(0,-1), new Vector2(0,3), fld, 's', 'd');
		GameObject tmp = (GameObject)Instantiate(Resources.Load("Stair1"),instPos,Quaternion.identity);
		tmp.transform.SetParent(buildingContainerObj.transform);
		
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
			
			//ensures no objects are created on the roof, and that it is empty.
			if(levelNum!=numberOfFloors-1){
				Vector3 instPos = findPartLocationWithInfluence(new Vector2(1,3),new Vector2(1,1), new Vector2(0,-1), new Vector2(0,3), fld, 's', 'd');
				GameObject tmp = (GameObject)Instantiate(Resources.Load("Stair1"),instPos,Quaternion.identity);
				tmp.transform.SetParent(buildingContainerObj.transform); 
				Vector3 instPos2 = findPartLocationWithInfluence(new Vector2(1,3),new Vector2(1,1), new Vector2(0,-1), new Vector2(0,3), fld, 's', 'd');
				GameObject tmp2 = (GameObject)Instantiate(Resources.Load("Stair1"),instPos2,Quaternion.identity);
				tmp2.transform.SetParent(buildingContainerObj.transform);
			}
			addHallways(fld);
			previousLayout = fld;
			fld.printObjectData();
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
				GameObject tmp = (GameObject)Instantiate(Resources.Load(name),new Vector3(startPos.x+offset,floor.height,startPos.y+offset),Quaternion.identity);
				tmp.transform.SetParent(buildingContainerObj.transform);
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
	//method will find room for an object, and return the vector3 coordinates for instantiation. It will also set the influence layer, which will influence the next layer that is added.
	private Vector3 findPartLocationWithInfluence(Vector2 dimensions, Vector2 buffer, Vector2 baseValue, Vector2 influenceValue, FloorLayoutData fld ,char value ,char value2){
		
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
		fld.editObjectData(pos+baseValue,pos+baseValue+new Vector2(1,1),value2);
		fld.editInfluenceData(pos+influenceValue,pos+influenceValue+new Vector2(1,1), value2);
		return new Vector3 (pos.x+(dimensions.x/2f),fld.height+1,pos.y+(dimensions.y/2f));
	}
	//Call this method once stairs/other up methods have been introduced.
	
	
	int minStraightLength = 3;
	int maxStraightLength = 5;
	//Going to keep some variables for this method by this method for ease of use.
	public void addHallways(FloorLayoutData fld){
		
		int avgDimension = Mathf.RoundToInt((fld.getDimention(0)+fld.getDimention(1))/2f);
		int minStraightLength = avgDimension/5+1;
		int maxStraightLength = avgDimension/2;
		
		List<Vector2> entrances = findFloorEntrances(fld);
		bool linked = false;
		
		Debug.Log(entrances.Count);
		if(entrances.Count<2)
			return;
		
		int entranceIndex = 1;
		int maxIterations = 200;
		int cIterations = 0;
		Vector2 currentPos = entrances[0];
		Vector2 currentDestination = entrances[1];
		Vector2 prevPos = new Vector2(-1,-1);
		List<char> validChars = new List<char>{'e','h'};
		
		while(linked==false && cIterations<maxIterations){
			
			//Check if go right
			if(currentPos.x-currentDestination.x<0 && inBounds(fld,currentPos+new Vector2(1,0)) && validChars.Contains(fld.floorObjectData[Mathf.RoundToInt(currentPos.x+1),Mathf.RoundToInt(currentPos.y)]) && !(prevPos.x==(currentPos.x+1) && prevPos.y==currentPos.y)){
				prevPos = currentPos;
				currentPos+=new Vector2(1,0);
				fld.floorObjectData[Mathf.RoundToInt(currentPos.x),Mathf.RoundToInt(currentPos.y)] = 'h';
				//Debug.Log("Going right");
			//Check if go left
			}else if(currentPos.x-currentDestination.x>0 && inBounds(fld,currentPos+new Vector2(-1,0)) && validChars.Contains(fld.floorObjectData[Mathf.RoundToInt(currentPos.x-1),Mathf.RoundToInt(currentPos.y)]) && !(prevPos.x==(currentPos.x-1) && prevPos.y==currentPos.y)){
				prevPos = currentPos;
				currentPos+=new Vector2(-1,0);
				fld.floorObjectData[Mathf.RoundToInt(currentPos.x),Mathf.RoundToInt(currentPos.y)] = 'h';	
				//Debug.Log("Going left");
			//Check if go up
			}else if(currentPos.y-currentDestination.y>0 && inBounds(fld,currentPos+new Vector2(0,-1)) && validChars.Contains(fld.floorObjectData[Mathf.RoundToInt(currentPos.x),Mathf.RoundToInt(currentPos.y-1)]) && !(prevPos.x==currentPos.x && prevPos.y==(currentPos.y-1))){
				prevPos = currentPos;
				currentPos+=new Vector2(0,-1);
				fld.floorObjectData[Mathf.RoundToInt(currentPos.x),Mathf.RoundToInt(currentPos.y)] = 'h';
				//Debug.Log("Going up");
			//Check if go down
			}else if(currentPos.y-currentDestination.y<0 && inBounds(fld, currentPos+new Vector2(0,1)) && validChars.Contains(fld.floorObjectData[Mathf.RoundToInt(currentPos.x),Mathf.RoundToInt(currentPos.y+1)]) && !(prevPos.x==currentPos.x && prevPos.y==(currentPos.y+1))){
			 	prevPos = currentPos;
				currentPos+=new Vector2(0,1);
				fld.floorObjectData[Mathf.RoundToInt(currentPos.x),Mathf.RoundToInt(currentPos.y)] = 'h';
				//Debug.Log("Going down");
			}	
			Debug.Log(Vector2.Distance(currentPos,currentDestination));
			//check to see if we are at the destination	
			if(Vector2.Distance(currentPos,currentDestination)<=1){
				currentPos = currentDestination;
				entranceIndex++;
				if(entranceIndex==entrances.Count){
					linked = true;
				}else{
					currentDestination = entrances[entranceIndex];
					prevPos = new Vector2(-1,-1);
				}
			}
			fld.printObjectData();
			cIterations++;
		}
		Debug.Log(cIterations);
	}
	//Call this to find all of the ways/spots people can get onto the floor
	public List<Vector2> findFloorEntrances(FloorLayoutData fld){
	
		List<Vector2> locations = new List<Vector2>();
	
		for(int j = 0;j<fld.getDimention(0);j++){
			for(int k = 0;k<fld.getDimention(1);k++){
				if(fld.floorObjectData[j,k]=='d'){
					locations.Add(new Vector2(j,k));
				}
			}
		}
		return locations;
	}
	//Returns true or false based on if the given bounds are in range or not
	public bool inBounds(FloorLayoutData fld, Vector2 bounds){
		if(bounds.x<0 || bounds.y<0 || bounds.x >= fld.getDimention(0) || bounds.y >= fld.getDimention(1))
			return false;
		return true;
	}
}
