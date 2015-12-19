using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text descriptorText;
    public Text ammoText;
    public static UIManager instance;
	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //Sets descriptor text. Used for things that can be interacted with.
    public void describeAction(string text) {
        descriptorText.text = text;
    }
    //Sets the clip left
    public void setClips(int clips) {
        string[] temp = ammoText.text.Split('/');
        int bullets = int.Parse(temp[0]);
        ammoText.text = bullets + " / " + clips;
    }
    //Sets the bullets left
    public void setBullets(int bullets) {
        string[] temp = ammoText.text.Split('/');
        int clips = int.Parse(temp[1]);
        ammoText.text = bullets + " / " + clips;
    }
}
