using UnityEngine;
using System.Collections;

public class InventoryItem {

    private Item item;
    private int count;

	public InventoryItem(Item nitem, int ncount) {
        item = nitem;
        count = ncount;
    }
    public Item getItem() {
        return item;
    }
    public int getCount() {
        return count;
    }
    //Call this to add or remove item counts
    public void modifyCount(int num) {
        if(item.isStackable())
            count += num;
    }
}
