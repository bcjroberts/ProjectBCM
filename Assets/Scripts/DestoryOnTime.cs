using UnityEngine;
using System.Collections;

public class DestoryOnTime : MonoBehaviour {
	
	
	public float destoryTimer;
	public bool hasLight;
	public float lightOffTimer;
	private float ctime = 0;
	// Update is called once per frame
	void Update () {
		if(ctime>=destoryTimer){
			Destroy(this.gameObject);
		}else{
			ctime+=Time.deltaTime;
		}
		if(hasLight && ctime>=lightOffTimer){
			GetComponent<Light>().enabled = false;
		}
	}
}
