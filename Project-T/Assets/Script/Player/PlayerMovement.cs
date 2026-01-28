using Script.Field;
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
    [SerializeField] LayerMask field_layer_mask;
    // Update is called once per frame
    void Update()
    {
        
    }

    FieldObject CheckNearbyField()
    {
        Vector2 detectionCenter = transform.position;
        // Returns an array of ALL colliders within the circle
        Collider2D hitColliders = Physics2D.OverlapCircle(detectionCenter, field_interaction_radius,field_layer_mask);


        FieldObject fieldObj = hitColliders.GetComponent<FieldObject>();
        return fieldObj; 
    }
    public void Plant()
    {
        if (seed <= 0)
        {
            Debug.Log("Khong du hat de trong");
            return;
        }

        FieldObject fieldObj = CheckNearbyField();
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

    public void harvest()
    {
        
        FieldObject fieldObj = CheckNearbyField();
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
        
    public void Move(Vector2 move_value)
    {
        transform.position = transform.position + new Vector3(-move_value.x,move_value.y,0)  * speed * Time.deltaTime;
    }
}
