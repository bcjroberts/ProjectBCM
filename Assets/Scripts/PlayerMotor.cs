using UnityEngine;
using System.Collections;

public class PlayerMotor : MonoBehaviour {
	
	private Rigidbody myRigidbody;
	
	private Vector3 myVelocity = new Vector3();
	private Vector3 rotation = new Vector3();
	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
	}
	
	public void setVelocity(Vector3 nVelocity){
		myVelocity = nVelocity;
	}
	public void rotate(Vector3 nRotation){
		rotation = nRotation;
	}
	// Update is called once per tick
	void FixedUpdate () {
		
		if(Input.GetKey(KeyCode.LeftControl)){
			myRigidbody.constraints = RigidbodyConstraints.FreezeAll;	
		}else{
			myRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
		}
		
		//Performes player movement taking collisions into account
		if(myVelocity != Vector3.zero){
			myRigidbody.MovePosition(myRigidbody.position + myVelocity*Time.deltaTime);
		}
		//performes player rotation
		myRigidbody.MoveRotation(myRigidbody.rotation * Quaternion.Euler(rotation));	
	}
}
