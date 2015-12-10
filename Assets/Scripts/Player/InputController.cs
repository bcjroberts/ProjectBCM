using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	private float jumpForce = 2000f;
	private bool canJump = false;
	private float speed = 3f;
	private float runMod = 1.5f;
	private float rotationSpeed = 3;
	private float rotMod = 1;
	private float aimingRotationMod = 1f/3f;
	private float aimingSpeedMod = 0.5f;
	private PlayerMotor myMotor;
	// Use this for initialization
	void Start () {
		myMotor = GetComponent<PlayerMotor>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Gets input and calculates a velocity based on that input
		float xDif = Input.GetAxisRaw("Horizontal");
		float yDif = Input.GetAxisRaw("Vertical");
		
		Vector3 xMov = transform.right*xDif;
		Vector3 yMov = transform.forward*yDif;
		
		Vector3 fMov = (xMov+yMov).normalized*speed;
		
		
		//Gets the input and makes the player rotate. Turns left and right.
		float yRot = Input.GetAxisRaw("Mouse X");
		
		Vector3 rotation = new Vector3(0,yRot,0)*rotationSpeed;
		
		//Handles more precise aiming on right click
		if(Input.GetMouseButton(1)){
			Camera.main.fieldOfView = 40f;
			rotation = rotation*aimingRotationMod;
			rotMod = rotationSpeed*aimingRotationMod;
			fMov = fMov*aimingSpeedMod;
		}else{
			rotMod = rotationSpeed;
			Camera.main.fieldOfView = 60f;
		}
		//running on right shift
		if(!Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)){
			fMov = fMov*runMod;
		}
		//Final set of movement here
		if(canJump){
			myMotor.setVelocity(fMov);
		}
		//Final set of rotation
		myMotor.rotate(rotation);
		
		//Gets the input and makes the camera rotate up and down. Limits rotation as well.
		transform.GetChild(0).Rotate(Vector3.left*Input.GetAxis("Mouse Y")*rotMod);
		
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

        bool performAction = false;
        if (Input.GetKeyDown(KeyCode.E)) {
            performAction = true;
        }
        //How to interact with objects
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward*1.5f);
        RaycastHit hitInfo;
        if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward, out hitInfo, 1.5f)) {
            GameObject obj = hitInfo.transform.gameObject;
            if (obj.GetComponentInParent<ObjectActionsInterface>()!=null && performAction) {
                obj.GetComponentInParent<ObjectActionsInterface>().doAction();
            }else if (obj.GetComponent<ObjectActionsInterface>()!=null && performAction) {
                obj.GetComponent<ObjectActionsInterface>().doAction();
            }
        }
	}
}
