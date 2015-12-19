using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {

    //Overall inventory on top
    private List<InventoryItem> inventory = new List<InventoryItem>();
    //Inventory that is in the easily accessible section here.
    private InventoryItem[] diplayedInventory = new InventoryItem[5];

    private int maxWeight = 100;
    private int currentWeight = 0;
	// Use this for initialization
    //Could initialize some items here
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //Method checks to see if an item can be added, call this before addItem
    public bool canAddItem(Item item) {
        if (item.getWeight() + currentWeight > maxWeight) {
            return false;
        }
        else {
            return true;
        }
    }
    //Method adds item to inventory
    public void addItem(Item item) {
        
        InventoryItem iItem = null;
        for(int j = 0; j < inventory.Count; j++) {
            if (inventory[j].getItem().getName().Equals(item.getName())) {
                iItem = inventory[j];
                break;
            }
        }
        if (iItem==null) {//Object not found
            iItem = new InventoryItem(item, 1);
        }

        //Need to add code to either increase the item count or add the item to the list,
        //then update the current weight variable

    }
    //Method finds and removes entire item or a certain count from an item
    public void removeItem(Item rItem, int rCount) {

    }
}
