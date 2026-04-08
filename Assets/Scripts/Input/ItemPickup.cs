using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{

    public delegate void ItemPickupEvent(GameObject itemToPickup);
    public static event ItemPickupEvent OnItemPickup;   

    [SerializeField] float pickupDistance = 2f;
    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                CheckDistanceAndPickUp();
            }
        }
    }

    void CheckDistanceAndPickUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance <= pickupDistance)
            {
                PickUp();
            }
            else
            {
                Debug.Log("You are too far away");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Player' found");
        }
    }

    void PickUp()
    {
        Debug.Log("picked up: " + gameObject.name);

        //we need to fire an event here that will tell the inventory system to add this item to the inventory
        OnItemPickup?.Invoke(gameObject);
        
        //Destroy(gameObject);
    }
}