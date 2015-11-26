﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildingGenerator : MonoBehaviour {
	
	//These variables are self explanitory I believe
	public int numberOfFloors;
	public Vector2 floorDimensions;
	public GameObject buildingContainerObj;
	
	private List<FloorLayoutData> floorInformation = new List<FloorLayoutData>(); 
	private List<RoomDataConnection> specialCases = new List<RoomDataConnection> ();
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
			addRooms(fld);
			instantiateRooms(fld);
			previousLayout = fld;
			//fld.printObjectData();
			//fld.printLayoutData();
			constructLevel(++levelNum, baseHeight+=2);
		}else{//Now make the outside of the building
			for(int j = 0;j<=levelNum;j++)
				addOuterWalls(floorDimensions, j);
		}
		
	}
	//Filles all of the remaining floor space. Call this when all stairs, ladders, rooms etc have already been added.
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
	//Call this to add room walls and doorways
	private void instantiateRooms(FloorLayoutData floor){
		
		List<char> chars = new List<char>{'s','h','d','e'};
		//Debug.Log("Called");
		for (int j = 0; j<floor.floorObjectData.GetLength(0); j++) {
			for(int k = 0;k<floor.floorObjectData.GetLength(1);k++){
				
				char currentChar = floor.floorObjectData[j,k];
				//If the object is not a stair, hall or door
				if(!chars.Contains(currentChar)){
					GameObject rSegment = (GameObject)Instantiate(Resources.Load("WallSegment"),new Vector3(j+0.5f,floor.height,k+0.5f),Quaternion.identity);
					rSegment.transform.SetParent(buildingContainerObj.transform);
					RoomSegment cSeg = rSegment.GetComponent<RoomSegment>();
					bool w1 = false;
					bool w2 = false;
					bool w3 = false;
					bool w4 = false;
					if(!inBounds(floor,new Vector2(j-1,k)) || floor.floorObjectData[j-1,k]==currentChar || floor.floorObjectData[j-1,k]=='d'){
						w3 = true;
					}
					if(!inBounds(floor,new Vector2(j+1,k)) || floor.floorObjectData[j+1,k]==currentChar || floor.floorObjectData[j+1,k]=='d'){
						w1 = true;
					}
					if(!inBounds(floor,new Vector2(j,k-1)) || floor.floorObjectData[j,k-1]==currentChar || floor.floorObjectData[j,k-1]=='d'){
						w4 = true;
					}
					if(!inBounds(floor,new Vector2(j,k+1)) || floor.floorObjectData[j,k+1]==currentChar || floor.floorObjectData[j,k+1]=='d'){
						w2 = true;
					}
					
					cSeg.setupSegments(w1,w2,w3,w4);
					floor.addRoomSegment(cSeg, j, k);
				}
			}
		}
		
		foreach(RoomDataConnection rdc in specialCases){
			int xdif = Mathf.RoundToInt(rdc.inRoom.x-rdc.outRoom.x); 
			int ydif = Mathf.RoundToInt(rdc.inRoom.y-rdc.outRoom.y);
			RoomSegment cSeg1 = floor.getRoomSegment(Mathf.RoundToInt(rdc.inRoom.x),Mathf.RoundToInt(rdc.inRoom.y));
			RoomSegment cSeg2 = floor.getRoomSegment(Mathf.RoundToInt(rdc.outRoom.x),Mathf.RoundToInt(rdc.outRoom.y));
			if(xdif!=0){
				if(xdif<0){//Right
					cSeg1.setupSegments(true,false,false,false);	
					cSeg2.setupSegments(false,false,true,false);
				}else{//Left
					cSeg1.setupSegments(false,false,true,false);
					cSeg2.setupSegments(true,false,false,false);
				}
			}else if(ydif!=0){
				if(ydif<0){//down
					cSeg1.setupSegments(false,true,false,false);
					cSeg2.setupSegments(false,false,false,true);		
				}else{//up
					cSeg1.setupSegments(false,false,false,true);
					cSeg2.setupSegments(false,true,false,false);
				}
			}else{
				Debug.Log("The current room data connection goes nowhere.");
			}
		
		}
	}
	//Call this method to add rooms to the structure
	private void addRooms(FloorLayoutData floor){

		bool done = false;
		bool resetMod = false;
		int charValue = 65;
		int xmax = floor.floorObjectData.GetLength (0) / 2;
		int ymax = floor.floorObjectData.GetLength (1) / 2;
		int modV = -1;
		List<RoomData> createdRooms = new List<RoomData> ();

		while (!done) {
			int cx = UnityEngine.Random.Range(2,xmax);
			int cy = UnityEngine.Random.Range(2, ymax);
			RoomData nData = findRoomLocation(new Vector2(cx,cy),floor,Convert.ToChar(charValue));
			bool result = nData.created;
			//if a room was not found, decrement the dimensions randomely and look again
			if(result==false){
				if(modV==-1){
					modV = UnityEngine.Random.Range(0,2);
					resetMod = false;
				}else if(modV==1){
					modV = 0;
					resetMod = true;
				}else{
					modV = 1;
					resetMod = true;
				}
				switch(modV){
				case 1:
					xmax--;
					break;
				case 0:
					ymax--;
					break;
				}	
				if(resetMod==true)
					modV = -1;
			}else{
				createdRooms.Add(nData);
				charValue++;
			}
			if(xmax<2 || ymax <2)
				done = true;
			
		}
		joinRooms (floor, createdRooms);
	}
	//Call this once all of the rooms have been added. Ensures all of the rooms can be reached.
	private void joinRooms(FloorLayoutData fld, List<RoomData> rooms){

		List<char> invalidChars = new List<char>{'s','h','e'};
		//Go through each rooms and find its connections
		for(int j = 0;j<rooms.Count;j++){

			int ix = Mathf.RoundToInt(rooms[j].position.x);
			int iy = Mathf.RoundToInt(rooms[j].position.y);
			int dx = Mathf.RoundToInt(rooms[j].dimensions.x);
			int dy = Mathf.RoundToInt(rooms[j].dimensions.y);
			//initial values
			int x = ix;
			int y = iy;
			for(int k = 0;k<dx;k++){
				for(int i = 0;i<dy;i++){

					//Check all of the tiles surrounding the room to find which rooms border this room
					if(k==0 && inBounds(fld,new Vector2(x-1,y))){
						if(fld.floorObjectData[x-1,y]=='d'){
							rooms[j].connected = true;
						}else if(!invalidChars.Contains(fld.floorObjectData[x-1,y])){
							RoomDataConnection rdc = new RoomDataConnection(getDataWithChar(fld.floorObjectData[x-1,y],fld,rooms),new Vector2(x,y),new Vector2(x-1,y));
							rooms[j].addConnection(rdc);
						}
					}else if(k==dx-1 && inBounds(fld,new Vector2(x+1,y))){
						if(fld.floorObjectData[x+1,y]=='d'){
							rooms[j].connected = true;
						}else if(!invalidChars.Contains(fld.floorObjectData[x+1,y])){
							RoomDataConnection rdc = new RoomDataConnection(getDataWithChar(fld.floorObjectData[x+1,y],fld,rooms),new Vector2(x,y),new Vector2(x+1,y));
							rooms[j].addConnection(rdc);
						}
					}
					if(i==0 && inBounds(fld,new Vector2(x,y-1))){
						if(fld.floorObjectData[x,y-1]=='d'){
							rooms[j].connected = true;
						}else if(!invalidChars.Contains(fld.floorObjectData[x,y-1])){
							RoomDataConnection rdc = new RoomDataConnection(getDataWithChar(fld.floorObjectData[x,y-1],fld,rooms),new Vector2(x,y),new Vector2(x,y-1));
							rooms[j].addConnection(rdc);
						}
					}else if(i==dy-1 && inBounds(fld,new Vector2(x,y+1))){
						if(fld.floorObjectData[x,y+1]=='d'){
							rooms[j].connected = true;
						}else if(!invalidChars.Contains(fld.floorObjectData[x,y+1])){
							RoomDataConnection rdc = new RoomDataConnection(getDataWithChar(fld.floorObjectData[x,y+1],fld,rooms),new Vector2(x,y),new Vector2(x,y+1));
							rooms[j].addConnection(rdc);
						}
					}
					y++;
				}
				y = iy;
				x++;
			}

		}
		//Used for printing information about each room
		/*for(int j = 0;j<rooms.Count;j++){
			
			Debug.Log(fld.floorObjectData[Mathf.RoundToInt(rooms[j].position.x),Mathf.RoundToInt(rooms[j].position.y)] + ": Connected: " + rooms[j].connected + " Connections: " + rooms[j].roomConnections.Count);
			/*for(int k = 0;k<rooms[j].roomConnections.Count;k++){
				Debug.Log(fld.floorObjectData[Mathf.RoundToInt(rooms[j].roomConnections[k].roomData.position.x),Mathf.RoundToInt(rooms[j].roomConnections[k].roomData.position.y)]);
			}
		}*/
	
		
		List<RoomData> notConnected = new List<RoomData> (rooms);
		List<RoomData> checkNext = new List<RoomData> ();
		specialCases = new List<RoomDataConnection> ();
		bool allConnected = false;
		if (notConnected.Count > 0) {
			for(int j = 0;j<notConnected.Count; j++){
				if(notConnected[j].connected==true){
					checkNext.Add(notConnected[j]);
					notConnected.RemoveAt(j);
					j--;
				}
			}
		}else{
			allConnected = true;
		}
		int maxIter = 100;
		int cIter = 0;
		//allConnected = true;
		
		//Attempts to connect all of the rooms to eachother
		while (!allConnected && cIter<maxIter) {
			
			if(checkNext.Count>0){
				List<RoomData> tempData = new List<RoomData>();
				for(int j = 0;j<checkNext.Count;j++){
					//check to see if the current room has connections that need to be added
					for(int l = 0;l<checkNext[j].roomConnections.Count;l++){
						if(!checkNext[j].roomConnections[l].roomData.connected && 
						   !checkNext.Contains(checkNext[j].roomConnections[l].roomData)){
							checkNext.Add(checkNext[j].roomConnections[l].roomData);
						}
					}
					//Check if the current room is connected. If it is, remove it from the array
					if(checkNext[j].connected){
						notConnected.Remove(checkNext[j]);
						tempData.Add(checkNext[j]);
					}else{//If not connected, check to see if any of the connections are connected.
						for(int k = 0;k<checkNext[j].roomConnections.Count;k++){
							if(checkNext[j].roomConnections[k].roomData.connected){
								specialCases.Add(checkNext[j].roomConnections[k]);
								checkNext[j].connected = true;
								notConnected.Remove(checkNext[j]);
								tempData.Add(checkNext[j]);
								break;
							}
						}
					}
				}
				//remove from checknext here to ensure the loop is not messed up.
				foreach(RoomData d in tempData){
					checkNext.Remove(d);
				}
			}else if(notConnected.Count>0){//Need to connect these rooms to other rooms or hallways****************************
				//Based on the algorithm for placing rooms, we know that if a room is in this list still
				//It does not border any other rooms or doors, however it may border hallways, stairs or potentially other
				//objects. We also know that it will be at most 1 tile away from another room or hallway. So, just pick a tile
				//and add a door to that tile, then set this room as connected and place it in the checkNext array.
				//Find an 'e' and replace it with a d to connect this room.
				RoomData rd = notConnected[0];
				notConnected.RemoveAt(0);

				int ix = Mathf.RoundToInt(rd.position.x);
				int iy = Mathf.RoundToInt(rd.position.y);
				int dx = Mathf.RoundToInt(rd.dimensions.x);
				int dy = Mathf.RoundToInt(rd.dimensions.y);
				//initial values
				int x = ix;
				int y = iy;

				List<char> invalidChars2 = new List<char>{'s','e'};
				List<char> validChars2 = new List<char>{'h','e'};
				bool foundConnection = false;
				for(int k = 0;k<dx;k++){
					for(int i = 0;i<dy;i++){
						
						//Check all of the tiles surrounding the room to find which rooms border this room
						if(k==0 && inBounds(fld,new Vector2(x-2,y)) && !invalidChars2.Contains(fld.floorObjectData[x-2,y])){
							if(validChars2.Contains(fld.floorObjectData[x-1,y])){
								rd.connected = true;
								foundConnection = true;
								fld.floorObjectData[x-1,y] = 'd';
								break;
							}
						}else if(k==dx-1 && inBounds(fld,new Vector2(x+2,y)) && !invalidChars2.Contains(fld.floorObjectData[x+2,y])){
							if(validChars2.Contains(fld.floorObjectData[x+1,y])){
								rd.connected = true;
								foundConnection = true;
								fld.floorObjectData[x+1,y] = 'd';
								break;
							}
						}
						if(i==0 && inBounds(fld,new Vector2(x,y-2)) && !invalidChars2.Contains(fld.floorObjectData[x,y-2])){
							if(validChars2.Contains(fld.floorObjectData[x,y-1])){
								rd.connected = true;
								foundConnection = true;
								fld.floorObjectData[x,y-1] = 'd';
								break;
							}
						}else if(i==dy-1 && inBounds(fld,new Vector2(x,y+2)) && !invalidChars2.Contains(fld.floorObjectData[x,y+2])){
							if(validChars2.Contains(fld.floorObjectData[x,y+1])){
								rd.connected = true;
								foundConnection = true;
								fld.floorObjectData[x,y+1] = 'd';
								break;
							}
						}
						y++;
					}
					if(foundConnection)
						break;
					y = iy;
					x++;
				}
				if(foundConnection)
					checkNext.Add(rd);
			}else{
				allConnected = true;
			}
			cIter++;
		}
		if(cIter==maxIter)
			Debug.Log("WARNING: REACHED MAXIMUM ITERATIONS!!!");
		//Debug.Log (specialCases.Count);
	}
	//returna roomData with the same character that is passed in
	private RoomData getDataWithChar(char value, FloorLayoutData fld, List<RoomData> rooms){
		for (int j = 0; j<rooms.Count; j++) {
			if(fld.floorObjectData[Mathf.RoundToInt(rooms[j].position.x),Mathf.RoundToInt(rooms[j].position.y)]==value)
				return rooms[j];
		}
		return new RoomData ();
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
	//method will find room for an object, and return a roomData based on if the method found a spot for a room.
	private RoomData findRoomLocation(Vector2 dimensions,FloorLayoutData fld, char value){
		
		List<Vector2> potentialLocations = new List<Vector2> ();

		int r1 = fld.getDimention (0) - Mathf.RoundToInt (dimensions.x);
		int r2 = fld.getDimention (1) - Mathf.RoundToInt (dimensions.y);

		int xd = Mathf.RoundToInt (dimensions.x);
		int yd = Mathf.RoundToInt (dimensions.y);

		for (int j = 0; j<=r1; j++) {
			for(int k = 0;k<=r2;k++){

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
			return new RoomData();

		int choosePos = UnityEngine.Random.Range(0,potentialLocations.Count);
		Vector2 pos = potentialLocations [choosePos];
		fld.editObjectData(pos,pos+dimensions,value);
		RoomData nRoom = new RoomData(pos,dimensions);
		return nRoom;
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
	public void addHallways(FloorLayoutData fld){
		
		List<Vector2> entrances = findFloorEntrances(fld);
		
		//Debug.Log("Entrances: " + entrances.Count);
		if(entrances.Count<2)
			return;
		
		int entranceIndex = 1;
		Vector2 currentPos = entrances[0];
		Vector2 currentDestination = entrances[1];

		for (int j = 1; j<entrances.Count; j++) {

			List<Vector2> path = PathFinding.findPath (currentPos, currentDestination,fld.floorObjectData);

			//Run through each position in path and set that position to an 'h' to represent a hall
			foreach(Vector2 i in path){
				int x = Mathf.RoundToInt(i.x);
				int y = Mathf.RoundToInt(i.y);

				if(!new List<char> {'d','h'}.Contains(fld.floorObjectData[x,y])){
					fld.floorObjectData[x,y] = 'h';
				}
			}

			currentPos = currentDestination;
			entranceIndex++;
			if(j!=entrances.Count-1)
				currentDestination = entrances[entranceIndex];
		}
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
	//Adds the outer walls of the buildig. This includes Windows and such.
	public void addOuterWalls(Vector2 floorDimensions, float height){
		int x = Mathf.RoundToInt(floorDimensions.x);
		int y = Mathf.RoundToInt(floorDimensions.y);
		height *= 2;
		int wallCount1 = 0;
		int wallCount2 = 0;
		int wallCount3 = 0;
		int wallCount4 = 0;
		for(int j = 0;j<x;j++){
			for(int k = 0;k<y;k++){
				//Check for outside cases and place things accordingly
				GameObject outerPart = null;
				if(j==0){
					if(k==0){//add corner
						outerPart = (GameObject)Instantiate(Resources.Load("Corner1"),new Vector3(j,height+1f,k), Quaternion.identity);
						outerPart.transform.parent = buildingContainerObj.transform;
					}else if(k==y-1){//add corner
						outerPart = (GameObject)Instantiate(Resources.Load("Corner1"),new Vector3(j,height+1f,k+1), Quaternion.identity);
						outerPart.transform.parent = buildingContainerObj.transform;
					}
					if(wallCount1>=2){
						outerPart = (GameObject)Instantiate(Resources.Load("WallWindow1"),new Vector3(j,height+1,k+0.5f),Quaternion.identity);
						wallCount1 = 0;
					}else{
						outerPart = (GameObject)Instantiate(Resources.Load("Wall1"),new Vector3(j,height+1,k+0.5f),Quaternion.identity);
						wallCount1++;
					}
				}else if(j==x-1){
					if(k==0){//add corner
						outerPart = (GameObject)Instantiate(Resources.Load("Corner1"),new Vector3(j+1,height+1f,k), Quaternion.identity);
						outerPart.transform.parent = buildingContainerObj.transform;
					}else if(k==y-1){//add corner
						outerPart = (GameObject)Instantiate(Resources.Load("Corner1"),new Vector3(j+1,height+1f,k+1), Quaternion.identity);
						outerPart.transform.parent = buildingContainerObj.transform;
					}
					if(wallCount2>=2){
						outerPart = (GameObject)Instantiate(Resources.Load("WallWindow1"),new Vector3(j+1f,height+1,k+0.5f),new Quaternion(0,1,0,0));
						wallCount2 = 0;
					}else{
						outerPart = (GameObject)Instantiate(Resources.Load("Wall1"),new Vector3(j+1f,height+1,k+0.5f),new Quaternion(0,1,0,0));
						wallCount2++;
					}
				}
				if(k==0){
					if(wallCount3>=2){
						outerPart = (GameObject)Instantiate(Resources.Load("WallWindow1"),new Vector3(j+0.5f,height+1,k),new Quaternion(0,0.7f,0,-0.7f));
						wallCount3 = 0;
					}else{
						outerPart = (GameObject)Instantiate(Resources.Load("Wall1"),new Vector3(j+0.5f,height+1,k),new Quaternion(0,0.7f,0,-0.7f));
						wallCount3++;
					}
				}else if(k==y-1){
					if(wallCount4>=2){
						outerPart = (GameObject)Instantiate(Resources.Load("WallWindow1"),new Vector3(j+0.5f,height+1,k+1),new Quaternion(0,0.7f,0,0.7f));
						wallCount4 = 0;
					}else{
						outerPart = (GameObject)Instantiate(Resources.Load("Wall1"),new Vector3(j+0.5f,height+1,k+1),new Quaternion(0,0.7f,0,0.7f));
						wallCount4++;
					}
				}
				if(outerPart!=null){
					outerPart.transform.parent = buildingContainerObj.transform;
				}
			}
		}
	}
	//Returns true or false based on if the given bounds are in range or not
	public bool inBounds(FloorLayoutData fld, Vector2 bounds){
		if(bounds.x<0 || bounds.y<0 || bounds.x >= fld.getDimention(0) || bounds.y >= fld.getDimention(1))
			return false;
		return true;
	}
}
