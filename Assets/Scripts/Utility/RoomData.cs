using UnityEngine;
using System.Collections.Generic;

public class RoomData{

	//All variables can be publicly accessed for ease of use.
	public List<RoomDataConnection> roomConnections = new List<RoomDataConnection>();
	public Vector2 position;
	public Vector2 dimensions;
	public bool created;
	public bool connected;

	//used if the room exists
	public RoomData(Vector2 npos, Vector2 ndim){
		position = npos;
		dimensions = ndim;
		created = true;
	}
	//used if the room is not created
	public RoomData(){
		created = false;
	}
	//used to add a connection to the room.
	public void addConnection(RoomDataConnection rdc){
		roomConnections.Add(rdc);
	}
}
