using Script.Field;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    
    void Start()
    {
            
    }
    [SerializeField] LayerMask field_layer_mask;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Plant()
    {
        float radius = 5.0f;
        Vector2 detectionCenter = transform.position;

        // Returns an array of ALL colliders within the circle
        Collider2D hitColliders = Physics2D.OverlapCircle(detectionCenter, radius,field_layer_mask);


        Debug.Log("Found nearby field: " + hitColliders.name);
        FieldObject fieldObj = hitColliders.GetComponent<FieldObject>();
        // neu tim thay field object
        if (fieldObj)
            fieldObj.SetState(FieldObject.FieldStateEnum.Ripe);
        
    }
    public void Move(Vector2 move_value)
    {
        transform.position = transform.position + new Vector3(-move_value.x,move_value.y,0)  * speed * Time.deltaTime;
    }
}
