using UnityEngine;
using System.Collections.Generic;

public class BulletHoleManager : MonoBehaviour {
	
	public static BulletHoleManager instance;
	
	public GameObject[] bulletHoles;
	public int maxBulletHoles = 10;
	
	private LinkedList<GameObject> currentHoles = new LinkedList<GameObject>();
	// Use this for initialization
	void Start () {
		instance = this;
	}
	//Instantiates a bullet hole, and removes extra if there are more than 100 holes.
	public void addBulletHole(RaycastHit hitInfo){
		GameObject b =  (GameObject)Instantiate(bulletHoles[UnityEngine.Random.Range(0,bulletHoles.Length)],hitInfo.point+hitInfo.normal*0.01f,Quaternion.FromToRotation(Vector3.back,hitInfo.normal));
		currentHoles.AddLast(b);
		b.transform.SetParent(hitInfo.transform);
		if(currentHoles.Count>maxBulletHoles){
			GameObject bd = currentHoles.First.Value;
			currentHoles.RemoveFirst();
			Destroy(bd);
		}
	}
}
