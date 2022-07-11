using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDatabase : MonoBehaviour {

    public List<Database> Items = new List<Database>();


    public int GetItemInDatabaseByID(int id)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if(Items[i].id == id)
            {
                return i;
            }
        }

        return -1;
    }

}
