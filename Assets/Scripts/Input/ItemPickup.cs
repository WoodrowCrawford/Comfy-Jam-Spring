using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{

    public delegate void ItemPickupEvent(GameObject itemToPickup);
    public static event ItemPickupEvent OnItemPickup;   

    [SerializeField] float pickupDistance = 2f;
    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame && !DialogueUIBehavior.IsOpen)
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
        
        //check if the item can be picked up
        if(gameObject.GetComponent<ItemBehavior>().CanGoInInventory)
        {
            OnItemPickup?.Invoke(gameObject);
        }

        //if not then its an interactable object so call the event
        {
            gameObject.GetComponent<ItemBehavior>().OnItemUsed.Invoke();
        }
        
           
        //Destroy(gameObject);
    }
}