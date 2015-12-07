using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

    public static int level = 0;
	
	// Update is called once per frame
	void Update () {
        PlayerData.level = Mathf.RoundToInt(transform.position.y);
	}
}
