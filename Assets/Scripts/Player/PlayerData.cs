using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

    public static int level = 0;
    private bool checkLevel = true;
	// Update is called once per frame
	void Update () {
        
        StartCoroutine("getLevel");
	}
    //Stop processing player levels
    public void stopPlayerLevelChecking() {
        StopCoroutine("getLevel");
        checkLevel = false;
    }
    //Uses less processing cycles
    IEnumerator getLevel() {
        while (checkLevel) {
            PlayerData.level = Mathf.RoundToInt(transform.position.y / 2);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
