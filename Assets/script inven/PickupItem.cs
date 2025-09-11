using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour
{
    public ItemSO item;
    public int amount = 1;
    public KeyCode pickupKey = KeyCode.F;   // <-- phím nh?t: F

    bool _inRange;

    void Reset() { GetComponent<Collider>().isTrigger = true; } // b?t Trigger cho collider 3D

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) _inRange = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) _inRange = false; }

    void Update()
    {
        if (_inRange && Input.GetKeyDown(pickupKey))
        {
            if (Inventory.I && Inventory.I.AddItem(item, amount))
                Destroy(gameObject);
        }
    }
}
