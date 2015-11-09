using UnityEngine;
using System.Collections;

public class RoomDataConnection{

	public RoomData roomData;
	public Vector2 inRoom;
	public Vector2 outRoom;

	public RoomDataConnection(RoomData nroom, Vector2 nIn, Vector2 nOut){
		roomData = nroom;
		inRoom = nIn;
		outRoom = nOut;
	}
}
