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
	public float tfireRatePerMinute;
	public AudioClip firingSoundEffect;
	public AudioClip reloadSoundEffect;
	public GameObject fireEffectStartPos;
	public GameObject fireEffect;
	
	private int roundsPerClip;
	private bool automatic;
	private bool canSwitchRate;
	private float reloadTime;
	private float kickbackMod;
	private float accuracy;
	private float fireRatePerMinute;
	//end weapon specific variables

	private AudioSource myAudioSource;
	private int currentRoundsInClip;
	private float timePerShot;
	private float timeSinceShot;
	private bool loaded = true;
	private bool shooting;
	private bool singleFireClick;
	private bool doShootEffect;
	private float currentReloadTime = 0;
	private float weaponKickModifier = 0;

	private float timeSinceLastEffect;
	private float effectTimer = 0.05f;
	// Use this for initialization
	void Start () {
		roundsPerClip = troundsPerClip;
		automatic = tautomatic;
		canSwitchRate = tcanSwitchRate;
		reloadTime = treloadTime;
		kickbackMod = tkickbackMod;
		accuracy = taccuracy;
		fireRatePerMinute = tfireRatePerMinute;

		timePerShot = 1f / (fireRatePerMinute / 60f);
		
		Debug.Log(timePerShot);
		currentRoundsInClip = roundsPerClip;
		myAudioSource = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0) && !automatic){
			singleFireClick = true;
		}
		if(Input.GetKeyDown(KeyCode.F) && canSwitchRate)
			automatic = !automatic;
		
		//On left click
		if (Input.GetMouseButton (0)) {
			shooting = true;
			//Debug.Log("Shots left in clip: " + currentRoundsInClip);
		}else{
			shooting = false;
			if(weaponKickModifier>0){
				weaponKickModifier-=kickbackMod*5;
				if(weaponKickModifier<0)
					weaponKickModifier = 0;
			}
		}
		if(doShootEffect && shooting==true){
			AudioSource.PlayClipAtPoint(firingSoundEffect,transform.position);
			GameObject temp =  (GameObject)Instantiate(fireEffect,fireEffectStartPos.transform.position,fireEffectStartPos.transform.rotation);
			temp.transform.SetParent(fireEffectStartPos.transform);
			doShootEffect = false;
		}else{
			doShootEffect = false;
		}

	}
	//called at a fixed interval of time. Will be used for shooting because it is consistent.
	void FixedUpdate(){
		
		//Fire the gun. This is where all raycasts and such will be done/taken into account
		if (shooting == true && loaded == true && timeSinceShot>=timePerShot && (automatic||singleFireClick)) {
			timeSinceShot = 0;
			//calculate rounds that need to be processed
			if(currentRoundsInClip>0){
				currentRoundsInClip--;
			}else{
				loaded = false;
			}
			doShootEffect = true;
			singleFireClick = false;
			//Do calculations for accuracy and modify the second argument of raycast. Actually, just use kickback. Add if necessary.
			
			//Now we calculate the rayCast
			RaycastHit hitInfo;
			if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward,out hitInfo,100f)){
				//Debug.Log(hitInfo.transform.tag);
				if(hitInfo.transform.tag.Equals("Untagged")){
					BulletHoleManager.instance.addBulletHole(hitInfo);
				}else{//Check for damage to that object
				
				}
			}
			//Now account for weapon kick
			weaponKickModifier = (weaponKickModifier*weaponKickModifier)+kickbackMod;
			if(weaponKickModifier>3f){
				weaponKickModifier = 3f;
			}
			//Debug.Log(weaponKickModifier);
			Camera.main.transform.Rotate(Vector3.left*(weaponKickModifier+UnityEngine.Random.Range(-kickbackMod/2f,kickbackMod/2f)));
			Camera.main.transform.parent.GetComponent<PlayerMotor>().rotate(Vector3.up*(weaponKickModifier+UnityEngine.Random.Range(-kickbackMod/4f,kickbackMod/4f)/2f));
			
		}else if(loaded==false){
			currentReloadTime+=Time.fixedDeltaTime;
			if(currentReloadTime>=reloadTime){
				loaded = true;
				currentReloadTime = 0;
				currentRoundsInClip = roundsPerClip;
			}
			//Debug.Log("Reloading: " + currentReloadTime);
		}else{
			timeSinceShot+=Time.fixedDeltaTime;
		}

	}
}
