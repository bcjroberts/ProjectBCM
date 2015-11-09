using UnityEngine;
using System.Collections;

public class RoomSegment : MonoBehaviour {

	public GameObject[] segments = new GameObject[4];
	
	//delete wall segments that will not be used
	public void setupSegments(bool removeWall1, bool removeWall2, bool removeWall3, bool removeWall4){
		
		if(removeWall1){
			Destroy(segments[0]);
		}
		if(removeWall2){
			Destroy(segments[1]);
		}
		if(removeWall3){
			Destroy(segments[2]);
		}
		if(removeWall4){
			Destroy(segments[3]);
		}
	}
}
