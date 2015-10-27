using UnityEngine;
using System.Collections;

public class WeaponData : MonoBehaviour {

	//weapon specific variables
	public int troundsPerClip;
	public bool tautomatic;
	public bool tcanSwitchRate;
	public float treloadTime;
	public float tkickbackMod;
	public float taccuracy;
	public float tfireRatePerSecond;

	private int roundsPerClip;
	private bool automatic;
	private bool canSwitchRate;
	private float reloadTime;
	private float kickbackMod;
	private float accuracy;
	private float fireRatePerSecond;
	//end weapon specific variables

	private int currentRoundsInClip;
	private float timeBetweenShots;
	private bool loaded = true;
	private bool shooting;
	private float currentReloadTime = 0;

	// Use this for initialization
	void Start () {
		roundsPerClip = troundsPerClip;
		automatic = tautomatic;
		canSwitchRate = tcanSwitchRate;
		reloadTime = treloadTime;
		kickbackMod = tkickbackMod;
		accuracy = taccuracy;

		if (automatic == false)
			timeBetweenShots = fireRatePerSecond / 50;

		currentRoundsInClip = roundsPerClip;

	}
	
	// Update is called once per frame
	void Update () {

		//On left click
		if (Input.GetMouseButton (0)) {
			shooting = true;
		}

	}
	//called at a fixed interval of time. Will be used for shooting because it is consistent.
	void FixedUpdate(){

		if (shooting == true && loaded == true) {

		}
		if(loaded==false){
			currentReloadTime+=Time.fixedDeltaTime;
			if(currentReloadTime>=reloadTime){
				loaded = true;
				currentReloadTime = 0;
				currentRoundsInClip = roundsPerClip;
			}
		}

	}
}
