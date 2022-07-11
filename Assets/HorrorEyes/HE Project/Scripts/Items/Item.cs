using UnityEngine;

public class Item : MonoBehaviour {

    [Tooltip("Item ID for inventory system")]
    public int itemID;
    [Tooltip("Item count")]
    public int itemCount;
    [Tooltip("Item name")]
    public string itemName;
    [Tooltip("Item pickup sound")]
    public AudioClip pickupSound;
}
