using UnityEngine;
namespace ProjectG.Script.Item
{
    public enum item_type 
    { 
        fruit, 
        leaf, 
        branch, 
        stone 
    }
    public class Item : MonoBehaviour
    {
        [Header("Item Configuration")]
        [SerializeField] public item_type type;

        // Optional: You can add a unique ID or value if needed for scoring later
        [SerializeField] private int item_id;

        private void Awake()
        {
            // Ensure the object has a collider set to Trigger
            if (GetComponent<Collider2D>() != null)
            {
                GetComponent<Collider2D>().isTrigger = true;
            }
        }
    }
}