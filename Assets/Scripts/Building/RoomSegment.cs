using UnityEngine;
using System.Collections;

public class RoomSegment : MonoBehaviour {

	public GameObject[] segments = new GameObject[4];
	private GameObject[] Asegments = new GameObject[4];
	
	void Awake(){
		for(int j = 0;j<4;j++){
			Asegments[j] = segments[j];
			segments[j] = null;
		}
	}
	//delete wall segments that will not be used
	public void setupSegments(bool removeWall1, bool removeWall2, bool removeWall3, bool removeWall4){
		
		if(removeWall1 && Asegments[0]!=null){
			Destroy(Asegments[0]);
			Asegments[0] = null;
		}
		if(removeWall2 && Asegments[1]!=null){
			Destroy(Asegments[1]);
			Asegments[1] = null;
		}
		if(removeWall3 && Asegments[2]!=null){
			Destroy(Asegments[2]);
			Asegments[2] = null;
		}
		if(removeWall4 && Asegments[3]!=null){
			Destroy(Asegments[3]);
			Asegments[3] = null;
		}
	}
}
