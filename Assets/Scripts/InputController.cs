using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	private float jumpForce = 2000f;
	private bool canJump = false;
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
		
		//Gets the input and makes the camera rotate up and down. Limits rotation as well.
		transform.GetChild(0).Rotate(Vector3.left*Input.GetAxis("Mouse Y")*rotationSpeed);
		
		//Limit Camera viewing angles
		if(transform.GetChild(0).eulerAngles.x<300 && transform.GetChild(0).eulerAngles.x>260){
			transform.GetChild(0).eulerAngles = new Vector3(300,transform.GetChild(0).eulerAngles.y,transform.GetChild(0).eulerAngles.z);
		}else if(transform.GetChild(0).eulerAngles.x>60 && transform.GetChild(0).eulerAngles.x<100){
			transform.GetChild(0).eulerAngles = new Vector3(60,transform.GetChild(0).eulerAngles.y,transform.GetChild(0).eulerAngles.z);
		}
		Debug.DrawRay(transform.position,-transform.up*0.575f,Color.blue);
		if(Physics.Raycast(transform.position,-transform.up,0.575f)){
			canJump = true;
		}else{
			canJump = false;
		}
		if(Input.GetKeyDown(KeyCode.Space) && canJump){
			myMotor.jump(jumpForce);
		}
	}
}
