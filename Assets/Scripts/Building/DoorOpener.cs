using UnityEngine;
using System.Collections;

public class DoorOpener : MonoBehaviour, ObjectActionsInterface {

    private bool open = false;
    private bool closing = false;
    private bool opening = false;
    private float doorSpeed = 125f;
	
	// Update is called once per frame
	void Update () {
        if (opening) {
            if (transform.localEulerAngles.y + (doorSpeed * Time.deltaTime) > 140) {
                transform.Rotate(new Vector3(0, 140 - transform.localEulerAngles.y, 0) * Time.deltaTime);
                opening = false;
            }
            else {
                transform.Rotate(new Vector3(0, doorSpeed, 0) * Time.deltaTime);
            }
        }
        else if (closing) {
            if (transform.localEulerAngles.y - (doorSpeed * Time.deltaTime) < 0) {
                transform.Rotate(new Vector3(0, 0-transform.localEulerAngles.y, 0) * Time.deltaTime);
                closing = false;
            }
            else {
                transform.Rotate(new Vector3(0, -doorSpeed, 0) * Time.deltaTime);
            }
        }
	}
    //returns string describing action
    public string getAction() {
        if (open) {
            return ("Press [E] to close");
        }
        else {
            return ("Press [E] to open");
        }
    }
    //Does the action described by getAction
    public void doAction() {
        if (open) {
            open = false;
            closing = true;
            opening = false;
        }
        else {
            open = true;
            opening = true;
            closing = false;
        }
    }
}
