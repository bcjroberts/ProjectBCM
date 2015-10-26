using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	private float speed = 3f;
	private float rotationSpeed = 3;
	private PlayerMotor myMotor;
	// Use this for initialization
	void Start () {
		myMotor = GetComponent<PlayerMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		
		//Gets input and calculates a velocity based on that input
		float xDif = Input.GetAxisRaw("Horizontal");
		float yDif = Input.GetAxisRaw("Vertical");
		
		Vector3 xMov = transform.right*xDif;
		Vector3 yMov = transform.forward*yDif;
		
		Vector3 fMov = (xMov+yMov).normalized*speed;
		
		myMotor.setVelocity(fMov);
		
		//Gets the input and makes the player rotate. Turns left and right.
		float yRot = Input.GetAxisRaw("Mouse X");
		
		Vector3 rotation = new Vector3(0,yRot,0)*rotationSpeed;
		
		myMotor.rotate(rotation);
	}
}
