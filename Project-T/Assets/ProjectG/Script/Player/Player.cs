using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Script;
using Script.Field;
using Script.God;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public int seed,product,money;
    [SerializeField] public int seed_inc, product_inc;
    [SerializeField] private float speed;
    [SerializeField] private float field_interaction_radius = 5.0f;
    public Action<List<(string, Action<Player>)>> onMeetInteraction;
 

    Interactable CheckNearbyFieldInteractable()
    {
        Vector2 detectionCenter = transform.position;
        // Returns an array of ALL colliders within the circle
        Collider2D hitColliders = Physics2D.OverlapCircle(detectionCenter, field_interaction_radius,interactable_layer_mask);
        if (hitColliders == null) return null;
        Interactable interactableObj = hitColliders.GetComponent<Interactable>();
        return interactableObj; 
    }
    public void Move(Vector2 move_value)
    {
        transform.position = transform.position + new Vector3(-move_value.x,move_value.y,0)  * speed * Time.deltaTime;
    }

    private Interactable oldInteractable;
    void meetInteractable()
    {
        // When player near interatable object
        Interactable i = CheckNearbyFieldInteractable();
        if (i == oldInteractable)
            return;
        onMeetInteraction?.Invoke(i ==null ? null : i.GetList());
        oldInteractable = i;
    }
    public void Interact(InputAction.CallbackContext ctx)
    {
        
    }
    void Start()
    {
        MessageBox.Show("Vua");
    }
    [SerializeField] LayerMask interactable_layer_mask;
    // Update is called once per frame
    void FixedUpdate()
    {
        meetInteractable();
    }
}
