using UnityEngine;
using UnityEngine.UI;

public class AccuracyReticuleLink : MonoBehaviour {

	private WeaponData wd;

	// Use this for initialization
	void Start () {
		wd = Camera.main.transform.GetComponentInChildren<WeaponData> ();
	}
	
	// Update is called once per frame
	void Update () {

		float accuracyMod = wd.getAccuracyMod ()*5f+1;
		this.GetComponent<RectTransform> ().localScale = new Vector3(accuracyMod,accuracyMod,accuracyMod);

	}
}
