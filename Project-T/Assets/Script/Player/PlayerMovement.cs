using System.Threading.Tasks;
using Script;
using Script.Field;
using Script.God;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int seed,product,money;
    [SerializeField] private int seed_inc, product_inc;
    [SerializeField] private float speed;
    [SerializeField] private float field_interaction_radius = 5.0f;
    void Start()
    {
        
    }
    [SerializeField] LayerMask field_layer_mask,god_layer_mask;
    // Update is called once per frame
    void Update()
    {
        
    }

    T CheckNearbyFieldInteractable<T>(LayerMask mask) where T : Interactable
    {
        Vector2 detectionCenter = transform.position;
        // Returns an array of ALL colliders within the circle
        Collider2D hitColliders = Physics2D.OverlapCircle(detectionCenter, field_interaction_radius,mask);
        T interactableObj = hitColliders.GetComponent<T>();
        return interactableObj; 
    }
    public void Plant()
    {
        if (seed <= 0)
        {
            Debug.Log("Khong du hat de trong");
            return;
        }

        FieldObject fieldObj = CheckNearbyFieldInteractable<FieldObject>(field_layer_mask);
        if (fieldObj.GetState() != FieldObject.FieldStateEnum.EMPTY)
        {
            Debug.Log("Cannot Plant field that not EMPTY:");
            return;
        }
        seed--;
        // neu tim thay field object
        if (fieldObj)
        {
            Debug.Log("Found nearby field: " + fieldObj.name);
            fieldObj.SetState(FieldObject.FieldStateEnum.Planted);
        }
        
    }

    public void Harvest()
    {
        
        FieldObject fieldObj = CheckNearbyFieldInteractable<FieldObject>(field_layer_mask);
        if (!fieldObj)
        {
            Debug.Log("Cannot found nearby field:");
            return;
        }

        if (fieldObj.GetState() != FieldObject.FieldStateEnum.Ripe)
        {
            Debug.Log("Cannot harvest field that not ripe:");
            return;
        }
        Debug.Log("Harvest sucessfullly: " + fieldObj.name);
        seed += seed_inc;
        product += product_inc;
        fieldObj.SetState(FieldObject.FieldStateEnum.EMPTY);
    }

    public bool? AccptOffer = null;
    public async void Offer()
    {
        GodObject god = CheckNearbyFieldInteractable<GodObject>(god_layer_mask);
        int offerProductInt = god.RandomOfferList();
        if (product - offerProductInt <= 0)
        {
            Debug.Log("Cannot Offer");
            return;
        }
        Debug.Log($"Offer cost {offerProductInt}");
        while (AccptOffer == null)
        {
            await Task.Delay(100);
        }

        if (AccptOffer == true)
        {
            product -= offerProductInt; 
            Debug.Log("offer sucessfully");
            money += god.moneyForSucessful;
        }
        else
        {
            Debug.Log("offer rejected");
        }

        AccptOffer = null;
    }
    public void Move(Vector2 move_value)
    {
        transform.position = transform.position + new Vector3(-move_value.x,move_value.y,0)  * speed * Time.deltaTime;
    }
}
