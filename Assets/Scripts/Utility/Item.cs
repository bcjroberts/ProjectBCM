using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    public ItemTypes types = ItemTypes.WEAPON;
    public int pweight;
    public string pname;
    public bool pstackable;

    private bool stackable;
    private string itemName = "";
    private int weight = 0;
	// Use this for initialization
	void Start () {
        weight = pweight;
        itemName = pname;
        stackable = pstackable;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public int getWeight() {
        return weight;
    }
    public string getName() {
        return itemName;
    }
    public bool isStackable() {
        return stackable;
    }
}
//enum to keep track of the types of items that this could be. Will be used to see
//how an item can be used.
public enum ItemTypes {
    WEAPON, AMMO,
}