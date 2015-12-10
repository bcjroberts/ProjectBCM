using UnityEngine;
using System.Collections;

public class RoomSegment : MonoBehaviour {

	public GameObject[] segments = new GameObject[4];
	private GameObject[] Asegments = new GameObject[4];

    private bool wall1 = true;
    private bool wall2 = true;
    private bool wall3 = true;
    private bool wall4 = true;
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
            wall1 = false;
			Asegments[0] = null;
		}
		if(removeWall2 && Asegments[1]!=null){
			Destroy(Asegments[1]);
            wall2 = false;
			Asegments[1] = null;
		}
		if(removeWall3 && Asegments[2]!=null){
			Destroy(Asegments[2]);
            wall3 = false;
			Asegments[2] = null;
		}
		if(removeWall4 && Asegments[3]!=null){
			Destroy(Asegments[3]);
            wall4 = false;
			Asegments[3] = null;
		}
	}
    public bool getWall1() {
        return wall1;
    }
    public bool getWall2() {
        return wall2;
    }
    public bool getWall3() {
        return wall3;
    }
    public bool getWall4() {
        return wall4;
    }
}
